namespace Figlut.Spread.Web.Site.Controllers
{
    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Csv;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class CountryController : SpreadController
    {
        #region Constants

        private const string COUNTRY_GRID_PARTIAL_VIEW_NAME = "_CountryGrid";
        private const string EDIT_COUNTRY_PARTIAL_VIEW_NAME = "_EditCountryDialog";
        private const string CREATE_COUNTRY_PARTIAL_VIEW_NAME = "_CreateCountryDialog";

        public int CountryView { get; private set; }

        #endregion //Constants

        #region Methods

        public FilterModel<CountryModel> GetCountryFilterModel(
            SpreadEntityContext context,
            FilterModel<CountryModel> model)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<Country> countryList = context.GetCountriesByFilter(model.SearchText);
            List<CountryModel> modelList = new List<CountryModel>();
            foreach (Country c in countryList)
            {
                CountryModel m = new CountryModel();
                m.CopyPropertiesFromCountry(c);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllCountryCount();
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index()
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<CountryModel> model = GetCountryFilterModel(
                    context,
                    new FilterModel<CountryModel>());
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
        public ActionResult Index(FilterModel<CountryModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<CountryModel> resultModel = GetCountryFilterModel(context, model);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(COUNTRY_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsCampaignModel>());
                }
                return PartialView(COUNTRY_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid countryId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                Country country = context.GetCountry(countryId, true);
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                context.Delete<Country>(country);
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
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                Country country = context.GetCountry(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (country != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete Country '{0}'?", country.CountryName);
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
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                Country country = context.GetCountry(model.Identifier, true);
                context.Delete<Country>(country);
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
                if (!Request.IsAuthenticated)
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

                string organizationIdString = searchParameters[searchParameters.Length - 1];
                Guid organizationId = Guid.Parse(organizationIdString);
                Organization organization = context.GetOrganization(organizationId, true);
                if (!CurrentUserHasAccessToOrganization(organization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                model.ParentId = organization.OrganizationId;
                model.ParentCaption = organization.Name;
                model.SearchText = searchText;
                model.ConfirmationMessage = string.Format("Delete all SMS Message Templates currently loaded for {0} '{1}'?", typeof(Organization).Name, organization.Name);
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
                        EntityReader<SmsMessageTemplate>.GetPropertyName(p => p.OrganizationId, false)));
                }
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(model.ParentId, context))
                {
                    return RedirectToHome();
                }
                context.DeleteSmsMessageTemplatesByFilter(model.SearchText, model.ParentId);
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

                string organizationIdString = searchParameters[searchParameters.Length - 1];
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<Country> countryList = context.GetCountriesByFilter(searchText);
                EntityCache<Guid, CountryCsv> cache = new EntityCache<Guid, CountryCsv>();
                foreach (Country c in countryList)
                {
                    CountryCsv csv = new CountryCsv();
                    csv.CopyPropertiesToCountry(c);
                    cache.Add(csv.CountryId, csv);
                }
                return GetCsvFileResult<CountryCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> smsMessageTemplateId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!smsMessageTemplateId.HasValue)
                {
                    return PartialView(EDIT_COUNTRY_PARTIAL_VIEW_NAME, new SmsMessageTemplateModel());
                }
                Country country = context.GetCountry(smsMessageTemplateId.Value, true);
                CountryModel model = new CountryModel();
                model.CopyPropertiesToCountry(country);
                PartialViewResult result = PartialView(EDIT_COUNTRY_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(CountryModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                Country country = context.GetCountry(model.CountryId, true);
                model.CopyPropertiesToCountry(country);
                context.Save<Country>(country, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult CreateDialog()
        {
            try
            {
                return PartialView(CREATE_COUNTRY_PARTIAL_VIEW_NAME, new SmsMessageTemplateModel());
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(true);
            }
        }

        [HttpPost]
        public ActionResult CreateDialog(CountryModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                model.CountryId = Guid.NewGuid();
                model.DateCreated = DateTime.Now;
                Country country = new Country();
                model.CopyPropertiesToCountry(country);
                context.Save<Country>(country, false);
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
