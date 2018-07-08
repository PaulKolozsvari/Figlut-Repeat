namespace Figlut.Repeat.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Csv;
    using Figlut.Repeat.ORM.Helpers;
    using Figlut.Repeat.ORM.Views;
    using Figlut.Repeat.Web.Site.Configuration;
    using Figlut.Repeat.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class ScheduleController : RepeatController
    {
        #region Constants

        private const string SCHEDULE_GRID_PARTIAL_VIEW_NAME = "_ScheduleGrid";
        private const string EDIT_SCHEDULE_PARTIAL_VIEW_NAME = "_EditScheduleDialog";
        private const string EXTEND_SCHEDULE_PARTIAL_VIEW_NAME = "_ExtendScheduleDialog";
        private const string CREATE_SCHEDULE_PARTIAL_VIEW_NAME = "_CreateScheduleDialog";

        #endregion //Constants

        #region Methods

        public FilterModel<ScheduleModel> GetScheduleFilterModel(
            RepeatEntityContext context,
            FilterModel<ScheduleModel> model,
            Nullable<Guid> subscriptionId)
        {
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<ScheduleView> scheduleViewList = context.GetScheduleViewsByFilter(model.SearchText, subscriptionId);
            List<ScheduleModel> modelList = new List<ScheduleModel>();
            foreach (ScheduleView v in scheduleViewList)
            {
                ScheduleModel m = new ScheduleModel();
                m.CopyPropertiesFromScheduleView(v);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllScheduleCount();
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
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<ScheduleModel> model = GetScheduleFilterModel(
                    context,
                    new FilterModel<ScheduleModel>(),
                    subscriptionId);
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                return View(model);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Index(FilterModel<ScheduleModel> model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<ScheduleModel> resultModel = model.ParentId != Guid.Empty ?
                    GetScheduleFilterModel(context, model, model.ParentId) :
                    GetScheduleFilterModel(context, model, null);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(SCHEDULE_GRID_PARTIAL_VIEW_NAME, new FilterModel<PublicHolidayModel>());
                }
                return PartialView(SCHEDULE_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid scheduleId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                Schedule schedule = context.GetSchedule(scheduleId, true);
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                context.Delete<Schedule>(schedule);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult ConfirmDeleteDialog(Guid identifier)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                Schedule schedule = context.GetSchedule(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (schedule != null)
                {
                    OrganizationSubscriptionView subscription = context.GetOrganizationSubscriptionView(schedule.SubscriptionId, true);
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete Schedule '{0}' for {1}?", 
                        schedule.ScheduleName, 
                        subscription.SubscriberCellPhoneNumber);
                }
                PartialViewResult result = PartialView(CONFIRMATION_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ConfirmDeleteDialog(ConfirmationModel model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                context.DeleteScheduleAndEntries(model.Identifier, true);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult ConfirmDeleteAllDialog(string searchParametersString)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
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
                model.ConfirmationMessage = string.Format("Delete all Schedules currently loaded for {0} {1}?", 
                    subscription.CustomerFullName,
                    subscription.SubscriberCellPhoneNumber);
                PartialViewResult result = PartialView(CONFIRMATION_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ConfirmDeleteAllDialog(ConfirmationModel model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (model.ParentId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<Country>.GetPropertyName(p => p.CountryId, false)));
                }
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                context.DeleteSchedulesByFilter(model.SearchText, model.ParentId);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult DownloadCsvFile(string searchParametersString)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();

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
                List<ScheduleView> scheduleViewList = context.GetScheduleViewsByFilter(searchText, subscriptionId);
                EntityCache<Guid, ScheduleCsv> cache = new EntityCache<Guid, ScheduleCsv>();
                foreach (ScheduleView v in scheduleViewList)
                {
                    ScheduleCsv csv = new ScheduleCsv();
                    csv.CopyPropertiesFromScheduleView(v);
                    cache.Add(csv.ScheduleId, csv);
                }
                return GetCsvFileResult<ScheduleCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult ExtendDialog(Nullable<Guid> scheduleId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!scheduleId.HasValue)
                {
                    return PartialView(EXTEND_SCHEDULE_PARTIAL_VIEW_NAME, new ScheduleModel());
                }
                ScheduleView scheduleView = context.GetScheduleView(scheduleId.Value, true);
                ScheduleModel model = new ScheduleModel();
                model.CopyPropertiesFromScheduleView(scheduleView);
                model.ExtendDate = model.EndDate.HasValue ? model.EndDate.Value.AddDays(365) : (DateTime.Now).AddDays(365);
                PartialViewResult result = PartialView(EXTEND_SCHEDULE_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ExtendDialog(ScheduleModel model)
        {
            try
            {
                int maxSmsSendMessageLength = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                string errorMessage = null;
                if (!model.IsValid(out errorMessage, maxSmsSendMessageLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                ScheduleEntry lastScheduleEntry = context.GetLastScheduleEntry(model.ScheduleId);
                if (model.ExtendDate.Date < lastScheduleEntry.EntryDate)
                {
                    return GetJsonResult(false, string.Format("{0} may not be less than the last entry's date of {0}.",
                        EntityReader<ScheduleModel>.GetPropertyName(p => p.ExtendDate, true),
                        DataShaper.GetDefaultDateString(lastScheduleEntry.EntryDate)));
                }
                int extraDays = model.ExtendDate.Subtract(lastScheduleEntry.EntryDate).Days;
                context.ShiftScheduleEntry(lastScheduleEntry.ScheduleEntryId, lastScheduleEntry.EntryDate, "zaf", extraDays);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> scheduleId)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!scheduleId.HasValue)
                {
                    return PartialView(EDIT_SCHEDULE_PARTIAL_VIEW_NAME, new ScheduleModel());
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                RefreshSmsMessageTemplatesList(context);
                ScheduleView scheduleView = context.GetScheduleView(scheduleId.Value, true);
                ScheduleModel model = new ScheduleModel();
                model.CopyPropertiesFromScheduleView(scheduleView);
                model.NotificationMessageEdit = model.NotificationMessage;
                model.MaxSmsSendMessageLength = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                PartialViewResult result = PartialView(EDIT_SCHEDULE_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult EditDialog(ScheduleModel model)
        {
            try
            {
                model.NotificationMessage = model.NotificationMessageEdit;
                int maxSmsSendMessageLength = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                string errorMessage = null;
                if (!model.IsValid(out errorMessage, maxSmsSendMessageLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                RefreshSmsMessageTemplatesList(context);
                Schedule schedule = context.GetSchedule(model.ScheduleId, true);
                model.CopyPropertiesToSchedule(schedule);
                context.Save<Schedule>(schedule, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult CreateDialog(Nullable<Guid> subscriptionId)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!subscriptionId.HasValue || subscriptionId.Value == Guid.Empty)
                {
                    return PartialView(CREATE_SCHEDULE_PARTIAL_VIEW_NAME, new CreateScheduleModel());
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                RefreshSmsMessageTemplatesList(context);
                CreateScheduleView view = context.GetCreateScheduleModelView(subscriptionId.Value, true);
                Country country = context.GetCountryByCountryCode("zaf", true); //TODO Use IP address lookup to determine the country that the user is in, but for now we're only working with South Africa.
                CreateScheduleModel model = new CreateScheduleModel();
                model.CopyPropertiesFromCreateScheduleView(view);
                model.EntriesTime = new TimeSpan(9, 0, 0);
                model.DaysRepeatInterval = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[ORM.Helpers.GlobalSettingName.DefaultRepeatDaysInterval].SettingValue);
                model.CountryId = country.CountryId;
                model.StartDateCreate = DateTime.Now;
                model.EndDateCreate = DateTime.Now.AddDays(365);
                model.CreateScheduleEntries = true;
                model.ExcludeNonWorkingDays = true;
                model.ExcludePublicHolidays = true;
                model.MaxSmsSendMessageLength = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                PartialViewResult result = PartialView(CREATE_SCHEDULE_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult CreateDialog(CreateScheduleModel model)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                model.NotificationMessage = model.NotificationMessageCreate;
                RepeatEntityContext context = RepeatEntityContext.Create();
                RefreshSmsMessageTemplatesList(context);
                int maxSmsSendMessageLength = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                string errorMessage = null;
                if (!model.IsValid(out errorMessage, maxSmsSendMessageLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                CreateScheduleView view = new CreateScheduleView();
                model.CopyPropertiesToCreateScheduleView(view);
                context.CreateSchedule(view);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        #endregion //Actions
    }
}