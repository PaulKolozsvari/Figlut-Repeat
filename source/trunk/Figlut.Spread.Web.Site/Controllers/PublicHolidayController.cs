namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Csv;
    using Figlut.Spread.ORM.Views;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Data.iCalendar;

    #endregion //Using Directives

    public class PublicHolidayController : SpreadController
    {
        #region Constants

        private const string PUBLIC_HOLIDAY_GRID_PARTIAL_VIEW_NAME = "_PublicHolidayGrid";
        private const string EDIT_PUBLIC_HOLIDAY_PARTIAL_VIEW_NAME = "_EditPublicHolidayDialog";
        private const string CREATE_PUBLIC_HOLIDAY_PARTIAL_VIEW_NAME = "_CreatePublicHolidayDialog";
        private const string GENERATE_COUNTRY_PUBLIC_HOLIDAYS_VIEW_NAME = "_GenerateCountryPublicHolidaysDialog";

        #endregion //Constants

        #region Methods

        public FilterModel<PublicHolidayModel> GetPublicHolidayFilterModel(
            SpreadEntityContext context,
            FilterModel<PublicHolidayModel> model,
            Nullable<Guid> countryId)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<PublicHolidayView> publicHolidayViewList = context.GetPublicHolidaysViewByFilter(model.SearchText, countryId);
            List<PublicHolidayModel> modelList = new List<PublicHolidayModel>();
            foreach (PublicHolidayView v in publicHolidayViewList)
            {
                PublicHolidayModel m = new PublicHolidayModel();
                m.CopyPropertiesFromPublicHolidayView(v);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllPublicHolidayCount();
            if (countryId.HasValue)
            {
                Country country = context.GetCountry(countryId.Value, false);
                if (country != null)
                {
                    model.ParentId = country.CountryId;
                    model.ParentCaption = country.CountryName;
                }
            }
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index(Guid countryId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<PublicHolidayModel> model = GetPublicHolidayFilterModel(
                    context,
                    new FilterModel<PublicHolidayModel>(),
                    countryId);
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                return View(model);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Index(FilterModel<PublicHolidayModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(model.ParentId, context))
                {
                    return RedirectToHome();
                }
                FilterModel<PublicHolidayModel> resultModel = model.ParentId != Guid.Empty ?
                    GetPublicHolidayFilterModel(context, model, model.ParentId) :
                    GetPublicHolidayFilterModel(context, model, null);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(PUBLIC_HOLIDAY_GRID_PARTIAL_VIEW_NAME, new FilterModel<PublicHolidayModel>());
                }
                return PartialView(PUBLIC_HOLIDAY_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid publicHolidayId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                PublicHoliday publicHoliday = context.GetPublicHoliday(publicHolidayId, true);
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                context.Delete<PublicHoliday>(publicHoliday);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult ConfirmDeleteDialog(Guid identifier)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                PublicHoliday publicHoliday = context.GetPublicHoliday(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (publicHoliday != null)
                {
                    Country country = context.GetCountry(publicHoliday.CountryId, true);
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete Public Holiday '{0}' for {1} '{2}'?", publicHoliday.EventName, typeof(Country).Name, country.CountryName);
                }
                PartialViewResult result = PartialView(CONFIRMATION_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ConfirmDeleteDialog(ConfirmationModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                PublicHoliday publicHoliday = context.GetPublicHoliday(model.Identifier, true);
                context.Delete<PublicHoliday>(publicHoliday);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult ConfirmDeleteAllDialog(string searchParametersString)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;

                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);

                string countryIdString = searchParameters[searchParameters.Length - 1];
                Guid countryId = Guid.Parse(countryIdString);
                Country country = context.GetCountry(countryId, true);
                model.ParentId = country.CountryId;
                model.ParentCaption = country.CountryName;
                model.SearchText = searchText;
                model.ConfirmationMessage = string.Format("Delete all Public Holidays currently loaded for {0} '{1}'?", typeof(Country).Name, country.CountryName);
                PartialViewResult result = PartialView(CONFIRMATION_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ConfirmDeleteAllDialog(ConfirmationModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (model.ParentId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<Country>.GetPropertyName(p => p.CountryId, false)));
                }
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                context.DeletePublicHolidaysByFilter(model.SearchText, model.ParentId);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult DownloadCsvFile(string searchParametersString)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();

                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);

                string countryIdString = searchParameters[searchParameters.Length - 1];
                Guid countryId = Guid.Parse(countryIdString);
                Country country = context.GetCountry(countryId, true);
                if (countryId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<PublicHoliday>.GetPropertyName(p => p.CountryId, false)));
                }
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<PublicHolidayView> publicHolidayViewList = context.GetPublicHolidaysViewByFilter(searchText, countryId);
                EntityCache<Guid, PublicHolidayCsv> cache = new EntityCache<Guid, PublicHolidayCsv>();
                foreach (PublicHolidayView v in publicHolidayViewList)
                {
                    PublicHolidayCsv csv = new PublicHolidayCsv();
                    csv.CopyPropertiesFromPublicHolidayView(v);
                    cache.Add(csv.PublicHolidayId, csv);
                }
                return GetCsvFileResult<PublicHolidayCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> publicHolidayId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                if (!publicHolidayId.HasValue || publicHolidayId.Value == Guid.Empty)
                {
                    return PartialView(EDIT_PUBLIC_HOLIDAY_PARTIAL_VIEW_NAME, new PublicHolidayModel());
                }
                PublicHolidayView publicHolidayView = context.GetPublicHolidayView(publicHolidayId.Value, true);
                PublicHolidayModel model = new PublicHolidayModel();
                model.CopyPropertiesFromPublicHolidayView(publicHolidayView);
                PartialViewResult result = PartialView(EDIT_PUBLIC_HOLIDAY_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult EditDialog(PublicHolidayModel model)
        {
            try
            {
                model.DateIdentifier = DataShaper.GetDefaultDateString(model.HolidayDate);
                model.Year = model.HolidayDate.Year;
                model.Month = model.HolidayDate.Month;
                model.Day = model.HolidayDate.Day;
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                PublicHoliday publicHoliday = context.GetPublicHoliday(model.PublicHolidayId, true);
                model.CopyPropertiesToPublicHoliday(publicHoliday);
                context.Save<PublicHoliday>(publicHoliday, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult CreateDialog(Nullable<Guid> countryId)
        {
            try
            {
                if (!countryId.HasValue || countryId.Value == Guid.Empty)
                {
                    throw new NullReferenceException(string.Format("{0} not specified for creating a {1}.",
                        EntityReader<PublicHolidayModel>.GetPropertyName(p => p.CountryId, true),
                        DataShaper.ShapeCamelCaseString(typeof(PublicHoliday).Name)));
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                return PartialView(CREATE_PUBLIC_HOLIDAY_PARTIAL_VIEW_NAME, new PublicHolidayModel()
                {
                    CountryId = countryId.Value,
                    HolidayDateCreate = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult CreateDialog(PublicHolidayModel model)
        {
            try
            {
                model.PublicHolidayId = Guid.NewGuid();
                model.HolidayDate = model.HolidayDateCreate;
                model.DateIdentifier = DataShaper.GetDefaultDateString(model.HolidayDate);
                model.Year = model.HolidayDate.Year;
                model.Month = model.HolidayDate.Month;
                model.Day = model.HolidayDate.Day;
                model.DateCreated = DateTime.Now;
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                PublicHoliday publicHoliday = new PublicHoliday();
                model.CopyPropertiesToPublicHoliday(publicHoliday);
                context.Save<PublicHoliday>(publicHoliday, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult GenerateCountryPublicHolidays(Nullable<Guid> countryId)
        {
            try
            {
                if (!countryId.HasValue || countryId.Value == Guid.Empty)
                {
                    throw new NullReferenceException(string.Format("{0} not specified for downloading {1}s.",
                        EntityReader<GenerateCountryPublicHolidaysModel>.GetPropertyName(p => p.CountryId, true),
                        DataShaper.ShapeCamelCaseString(typeof(PublicHoliday).Name)));
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                Country country = context.GetCountry(countryId.Value, true);
                return PartialView(GENERATE_COUNTRY_PUBLIC_HOLIDAYS_VIEW_NAME, new GenerateCountryPublicHolidaysModel()
                {
                    CountryId = country.CountryId,
                    CountryCode = country.CountryCode,
                    CountryName = country.CountryName,
                    Year = DateTime.Now.Year
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(true);
            }
        }

        [HttpPost]
        public ActionResult GenerateCountryPublicHolidays(GenerateCountryPublicHolidaysModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                Country country = context.GetCountry(model.CountryId, true);
                ICalCalendar calendar = SpreadWebApp.Instance.CalendarDownloader.DownloadICalCalendar(
                    country.CountryCode, 
                    country.CountryName, 
                    model.Year, 
                    null,
                    true);
                context.SavePublicHolidaysFromICalCalendar(calendar);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        #endregion //Actions
    }
}
