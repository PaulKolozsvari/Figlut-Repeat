namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Csv;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.ORM.Views;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class RepeatScheduleController : SpreadController
    {
        #region Constants

        private const string REPEAT_SCHEDULE_GRID_PARTIAL_VIEW_NAME = "_RepeatScheduleGrid";
        private const string EDIT_REPEAT_SCHEDULE_PARTIAL_VIEW_NAME = "_EditRepeatScheduleDialog";
        private const string EXTEND_REPEAT_SCHEDULE_PARTIAL_VIEW_NAME = "_ExtendRepeatScheduleDialog";
        private const string CREATE_REPEAT_SCHEDULE_PARTIAL_VIEW_NAME = "_CreateRepeatScheduleDialog";

        #endregion //Constants

        #region Methods

        public FilterModel<RepeatScheduleModel> GetRepeatScheduleFilterModel(
            SpreadEntityContext context,
            FilterModel<RepeatScheduleModel> model,
            Nullable<Guid> subscriptionId)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<RepeatScheduleView> repeatScheduleViewList = context.GetRepeatScheduleViewsByFilter(model.SearchText, subscriptionId);
            List<RepeatScheduleModel> modelList = new List<RepeatScheduleModel>();
            foreach (RepeatScheduleView v in repeatScheduleViewList)
            {
                RepeatScheduleModel m = new RepeatScheduleModel();
                m.CopyPropertiesFromRepeatScheduleView(v);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllRepeatScheduleCount();
            if (subscriptionId.HasValue)
            {
                OrganizationSubscriptionView subscription = context.GetOrganizationSubscriptionView(subscriptionId.Value, false);
                if (subscription != null)
                {
                    model.ParentId = subscription.SubscriptionId;
                    model.ParentCaption = string.Format("{0} {1}", subscription.CustomerFullName, subscription.SubscriberCellPhoneNumber);
                }
            }
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index(Guid subscriptionId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<RepeatScheduleModel> model = GetRepeatScheduleFilterModel(
                    context,
                    new FilterModel<RepeatScheduleModel>(),
                    subscriptionId);
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
        public ActionResult Index(FilterModel<RepeatScheduleModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<RepeatScheduleModel> resultModel = model.ParentId != Guid.Empty ?
                    GetRepeatScheduleFilterModel(context, model, model.ParentId) :
                    GetRepeatScheduleFilterModel(context, model, null);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(REPEAT_SCHEDULE_GRID_PARTIAL_VIEW_NAME, new FilterModel<PublicHolidayModel>());
                }
                return PartialView(REPEAT_SCHEDULE_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid repeatScheduleId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                RepeatSchedule repeatSchedule = context.GetRepeatSchedule(repeatScheduleId, true);
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                context.Delete<RepeatSchedule>(repeatSchedule);
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
                RepeatSchedule repeatSchedule = context.GetRepeatSchedule(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (repeatSchedule != null)
                {
                    OrganizationSubscriptionView subscription = context.GetOrganizationSubscriptionView(repeatSchedule.SubscriptionId, true);
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete Repeat Schedule '{0}' for {1}?", 
                        repeatSchedule.ScheduleName, 
                        subscription.SubscriberCellPhoneNumber);
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
                context.DeleteRepeatScheduleAndEntries(model.Identifier, true);
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

                string subscriptionIdString = searchParameters[searchParameters.Length - 1];
                Guid subscriptionId = Guid.Parse(subscriptionIdString);
                OrganizationSubscriptionView subscription = context.GetOrganizationSubscriptionView(subscriptionId, true);
                model.ParentId = subscription.SubscriptionId;
                model.ParentCaption = string.Format("{0} {1}", subscription.CustomerFullName, subscription.SubscriberCellPhoneNumber);
                model.SearchText = searchText;
                model.ConfirmationMessage = string.Format("Delete all Repeat Schedules currently loaded for {0} {1}?", 
                    subscription.CustomerFullName,
                    subscription.SubscriberCellPhoneNumber);
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
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                context.DeleteRepeatSchedulesByFilter(model.SearchText, model.ParentId);
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

                string subscriptionIdString = searchParameters[searchParameters.Length - 1];
                Guid subscriptionId = Guid.Parse(subscriptionIdString);
                Subscription subscription = context.GetSubscription(subscriptionId, true);
                if (subscriptionId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<PublicHoliday>.GetPropertyName(p => p.CountryId, false)));
                }
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<RepeatScheduleView> repeatScheduleViewList = context.GetRepeatScheduleViewsByFilter(searchText, subscriptionId);
                EntityCache<Guid, RepeatScheduleCsv> cache = new EntityCache<Guid, RepeatScheduleCsv>();
                foreach (RepeatScheduleView v in repeatScheduleViewList)
                {
                    RepeatScheduleCsv csv = new RepeatScheduleCsv();
                    csv.CopyPropertiesFromRepeatScheduleView(v);
                    cache.Add(csv.RepeatScheduleId, csv);
                }
                return GetCsvFileResult<RepeatScheduleCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult ExtendDialog(Nullable<Guid> repeatScheduleId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!repeatScheduleId.HasValue)
                {
                    return PartialView(EXTEND_REPEAT_SCHEDULE_PARTIAL_VIEW_NAME, new RepeatScheduleModel());
                }
                RepeatScheduleView repeatScheduleView = context.GetRepeatScheduleView(repeatScheduleId.Value, true);
                RepeatScheduleModel model = new RepeatScheduleModel();
                model.CopyPropertiesFromRepeatScheduleView(repeatScheduleView);
                model.ExtendDate = model.EndDate.HasValue ? model.EndDate.Value.AddDays(365) : (DateTime.Now).AddDays(365);
                PartialViewResult result = PartialView(EXTEND_REPEAT_SCHEDULE_PARTIAL_VIEW_NAME, model);
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
        public ActionResult ExtendDialog(RepeatScheduleModel model)
        {
            try
            {
                int maxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                string errorMessage = null;
                if (!model.IsValid(out errorMessage, maxSmsSendMessageLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                RepeatScheduleEntry lastRepeatScheduleEntry = context.GetLastRepeatScheduleEntry(model.RepeatScheduleId);
                if (model.ExtendDate.Date < lastRepeatScheduleEntry.RepeatDate)
                {
                    return GetJsonResult(false, string.Format("{0} may not be less than the last entry's date of {0}.",
                        EntityReader<RepeatScheduleModel>.GetPropertyName(p => p.ExtendDate, true),
                        DataShaper.GetDefaultDateString(lastRepeatScheduleEntry.RepeatDate)));
                }
                int extraDays = model.ExtendDate.Subtract(lastRepeatScheduleEntry.RepeatDate).Days;
                context.ShiftRepeatScheduleEntry(lastRepeatScheduleEntry.RepeatScheduleEntryId, lastRepeatScheduleEntry.RepeatDate, "zaf", extraDays);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> repeatScheduleId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!repeatScheduleId.HasValue)
                {
                    return PartialView(EDIT_REPEAT_SCHEDULE_PARTIAL_VIEW_NAME, new RepeatScheduleModel());
                }
                RepeatScheduleView repeatScheduleView = context.GetRepeatScheduleView(repeatScheduleId.Value, true);
                RepeatScheduleModel model = new RepeatScheduleModel();
                model.CopyPropertiesFromRepeatScheduleView(repeatScheduleView);
                model.MaxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                PartialViewResult result = PartialView(EDIT_REPEAT_SCHEDULE_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(RepeatScheduleModel model)
        {
            try
            {
                int maxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                string errorMessage = null;
                if (!model.IsValid(out errorMessage, maxSmsSendMessageLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                RepeatSchedule repeatSchedule = context.GetRepeatSchedule(model.RepeatScheduleId, true);
                model.CopyPropertiesToRepeatSchedule(repeatSchedule);
                context.Save<RepeatSchedule>(repeatSchedule, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult CreateDialog(Nullable<Guid> subscriptionId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!subscriptionId.HasValue || subscriptionId.Value == Guid.Empty)
                {
                    return PartialView(CREATE_REPEAT_SCHEDULE_PARTIAL_VIEW_NAME, new CreateRepeatScheduleModel());
                }
                CreateRepeatScheduleView view = context.GetCreateRepeatScheduleModelView(subscriptionId.Value, true);
                Country country = context.GetCountryByCountryCode("zaf", true); //TODO Use IP address lookup to determine the country that the user is in, but for now we're only working with South Africa.
                CreateRepeatScheduleModel model = new CreateRepeatScheduleModel();
                model.CopyPropertiesFromCreateRepeatScheduleView(view);
                model.DaysRepeatInterval = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[ORM.Helpers.GlobalSettingName.DefaultRepeatDaysInterval].SettingValue);
                model.CountryId = country.CountryId;
                model.StartDateCreate = DateTime.Now;
                model.EndDateCreate = DateTime.Now.AddDays(365);
                model.MaxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                PartialViewResult result = PartialView(CREATE_REPEAT_SCHEDULE_PARTIAL_VIEW_NAME, model);
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
        public ActionResult CreateDialog(CreateRepeatScheduleModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                int maxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                string errorMessage = null;
                if (!model.IsValid(out errorMessage, maxSmsSendMessageLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                CreateRepeatScheduleView view = new CreateRepeatScheduleView();
                model.CopyPropertiesToCreateRepeatScheduleView(view);
                context.CreateRepeatSchedule(view);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        //public ActionResult CreateDialog()
        //{
        //    try
        //    {
        //        return PartialView(CREATE_REPEAT_SCHEDULE_PARTIAL_VIEW_NAME, new RepeatScheduleModel());
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler.HandleException(ex);
        //        SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
        //        return GetJsonResult(true);
        //    }
        //}

        //[HttpPost]
        //public ActionResult CreateDialog(RepeatScheduleModel model)
        //{
        //    try
        //    {
        //        string errorMessage = null;
        //        if (!model.IsValid(out errorMessage))
        //        {
        //            return GetJsonResult(false, errorMessage);
        //        }
        //        SpreadEntityContext context = SpreadEntityContext.Create();
        //        model.RepeatScheduleId = Guid.NewGuid();
        //        model.DateCreated = DateTime.Now;
        //        RepeatSchedule repeatSchedule = new RepeatSchedule();
        //        model.CopyPropertiesToRepeatSchedule(repeatSchedule);
        //        context.Save<RepeatSchedule>(repeatSchedule, false);
        //        return GetJsonResult(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler.HandleException(ex);
        //        SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
        //        return GetJsonResult(false, ex.Message);
        //    }
        //}

        #endregion //Actions
    }
}