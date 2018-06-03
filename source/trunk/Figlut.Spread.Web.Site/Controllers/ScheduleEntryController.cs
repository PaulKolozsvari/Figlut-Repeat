namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Figlut.Spread.Web.Site.Models;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM.Views;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.ORM;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM.Csv;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.SMS;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using System.Text;

    #endregion //Using Directives

    public class ScheduleEntryController : SpreadController
    {
        #region Constants

        private const string SCHEDULE_ENTRY_GRID_PARTIAL_VIEW_NAME = "_ScheduleEntryGrid";
        private const string EDIT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME = "_EditScheduleEntryDialog";
        private const string SHIFT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME = "_ShiftScheduleEntryDialog";
        private const string CREATE_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME = "_CreateScheduleEntryDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<ScheduleEntryModel> GetScheduleEntryFilterModelForSchedule(
            SpreadEntityContext context,
            FilterModel<ScheduleEntryModel> model,
            Nullable<Guid> scheduleId)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<ScheduleEntryView> scheduleEntryViewList = context.GetScheduleEntryViewsForScheduleByFilter(model.SearchText, scheduleId);
            List<ScheduleEntryModel> modelList = new List<ScheduleEntryModel>();
            foreach (ScheduleEntryView v in scheduleEntryViewList)
            {
                ScheduleEntryModel m = new ScheduleEntryModel();
                m.CopyPropertiesFromScheduleEntryView(v);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllScheduleEntryCount();
            if (scheduleId.HasValue)
            {
                ScheduleView schedule = context.GetScheduleView(scheduleId.Value, false);
                if (schedule != null)
                {
                    model.ParentId = schedule.ScheduleId;
                    model.ParentCaption = string.Format("{0} {1} {2}", schedule.ScheduleName, schedule.CustomerFullName, schedule.CellPhoneNumber);
                }
            }
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index(Guid scheduleId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<ScheduleEntryModel> model = GetScheduleEntryFilterModelForSchedule(
                    context,
                    new FilterModel<ScheduleEntryModel>(),
                    scheduleId);
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
        public ActionResult Index(FilterModel<ScheduleEntryModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<ScheduleEntryModel> resultModel = model.ParentId != Guid.Empty ?
                    GetScheduleEntryFilterModelForSchedule(context, model, model.ParentId) :
                    GetScheduleEntryFilterModelForSchedule(context, model, null);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(SCHEDULE_ENTRY_GRID_PARTIAL_VIEW_NAME, new FilterModel<ScheduleEntryModel>());
                }
                return PartialView(SCHEDULE_ENTRY_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult SendSms(Nullable<Guid> scheduleEntryId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!scheduleEntryId.HasValue || scheduleEntryId.Value == Guid.Empty)
                {
                    return GetJsonResult(false, string.Format("{0} not specified for sending an SMS.", EntityReader<ScheduleEntry>.GetPropertyName(p => p.ScheduleEntryId, false)));
                }
                string errorMessage = null;
                int maxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                string organizationIdentifierIndicator = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.OrganizationIdentifierIndicator].SettingValue;
                string smsSendMessageSuffix = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.SmsSendMessageSuffix].SettingValue;
                User currentUser = GetCurrentUser(context);
                Organization currentOrganization = GetCurrentOrganization(context, true);
                if (!CurrentUserHasAccessToOrganization(currentOrganization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                ScheduleEntry scheduleEntry = context.GetScheduleEntry(scheduleEntryId.Value, true);
                ScheduleView scheduleView = context.GetScheduleView(scheduleEntry.ScheduleId, true);
                if (currentOrganization.SmsCreditsBalance < 1 && !currentOrganization.AllowSmsCreditsDebt)
                {
                    return GetJsonResult(false, string.Format("{0} '{1}' has insufficient SMS credits to send an SMS.", typeof(Organization).Name, currentOrganization.Name));
                }
                SmsResponse response;
                try
                {
                    response = SpreadWebApp.Instance.SmsSender.SendSms(new SmsRequest(
                        scheduleView.CellPhoneNumber, scheduleEntry.NotificationMessage, maxSmsSendMessageLength, smsSendMessageSuffix, currentOrganization.Identifier, organizationIdentifierIndicator));
                }
                catch (Exception exFailed) //Failed to send the SMS Web Request to the provider.
                {
                    int smsProviderCode = (int)SpreadWebApp.Instance.Settings.SmsProvider;
                    context.LogFailedSmsSent(
                        scheduleView.CellPhoneNumber, scheduleView.NotificationMessage, smsProviderCode, exFailed, currentUser, out errorMessage);
                    return GetJsonResult(false, errorMessage);
                }
                SmsSentLog smsSentLog = SpreadWebApp.Instance.LogSmsSentToDB(
                    scheduleView.CellPhoneNumber, scheduleView.NotificationMessage, response, currentUser, true); //If this line throws an exception then we don't havea record of the SMS having been sent, therefore cannot deduct credits from the Organization. Therefore there's no point in wrapping this call in a try catch and swallowing any exception i.e. if we don't have a record of the SMS having been sent we cannot charge for it.
                if (response.success)
                {
                    long smsCredits = context.DecrementSmsCreditFromOrganization(currentOrganization.OrganizationId).SmsCreditsBalance;
                    GOC.Instance.Logger.LogMessage(new LogMessage(
                        string.Format("{0} '{1}' has sent an SMS. Credits remaining: {2}.",
                        typeof(Organization).Name,
                        currentOrganization.Name,
                        smsCredits),
                        LogMessageType.SuccessAudit,
                        LoggingLevel.Normal));

                    scheduleEntry.SMSNotificationSent = true;
                    scheduleEntry.SMSMessageId = response.messageId;
                    scheduleEntry.SMSDateSent = DateTime.Now;
                    if (smsSentLog != null)
                    {
                        scheduleEntry.SmsSentLogId = smsSentLog.SmsSentLogId;
                    }
                    context.Save<ScheduleEntry>(scheduleEntry, false);
                }
                else //Got a response from the provider, but sending the sms failed.
                {
                    StringBuilder errorMessageBuilder = new StringBuilder();
                    if (!string.IsNullOrEmpty(response.error))
                    {
                        errorMessageBuilder.AppendFormat("Error: {0}. ", response.error);
                    }
                    if (!string.IsNullOrEmpty(response.errorCode))
                    {
                        errorMessageBuilder.AppendFormat("Code: {0}. ", response.errorCode);
                    }
                    if (!string.IsNullOrEmpty(response.messageId))
                    {
                        errorMessageBuilder.AppendFormat("Message ID: {0}", response.messageId);
                    }
                    return GetJsonResult(false, errorMessageBuilder.ToString());
                }
                //Thread.Sleep(5000);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid scheduleEntryId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                ScheduleEntry scheduleEntry = context.GetScheduleEntry(scheduleEntryId, true);
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                context.Delete<ScheduleEntry>(scheduleEntry);
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
                ScheduleEntry scheduleEntry = context.GetScheduleEntry(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (scheduleEntry != null)
                {
                    ScheduleView schedule = context.GetScheduleView(scheduleEntry.ScheduleId, true);
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete entry '{0}' for {1} '{2}' ({3} {4})?",
                        DataShaper.GetDefaultDateString(scheduleEntry.EntryDate),
                        DataShaper.ShapeCamelCaseString(typeof(Schedule).Name),
                        schedule.ScheduleName,
                        schedule.CustomerFullName,
                        schedule.CellPhoneNumber);
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
                ScheduleEntry scheduleEntry = context.GetScheduleEntry(model.Identifier, true);
                context.Delete<ScheduleEntry>(scheduleEntry);
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

                string scheduleIdString = searchParameters[searchParameters.Length - 1];
                Guid scheduleId = Guid.Parse(scheduleIdString);
                if (scheduleId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<ScheduleEntry>.GetPropertyName(p => p.ScheduleId, false)));
                }
                ScheduleView schedule = context.GetScheduleView(scheduleId, true);
                model.ParentId = schedule.ScheduleId;
                model.ParentCaption = string.Format("{0} {1} {2}", schedule.ScheduleName, schedule.CustomerFullName, schedule.CellPhoneNumber);
                model.SearchText = searchText;
                model.ConfirmationMessage = string.Format("Delete all Schedules Entries currently loaded for {0} '{1}' for subscription {2} {3}'?", 
                    DataShaper.ShapeCamelCaseString(typeof(Schedule).Name), 
                    schedule.ScheduleName,
                    schedule.CustomerFullName,
                    schedule.CellPhoneNumber);
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
                        EntityReader<ScheduleEntry>.GetPropertyName(p => p.ScheduleId, false)));
                }
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                context.DeleteScheduleEntriesForScheduleByFilter(model.SearchText, model.ParentId);
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
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                SpreadEntityContext context = SpreadEntityContext.Create();

                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);

                string scheduleIdString = searchParameters[searchParameters.Length - 1];
                Guid scheduleId = Guid.Parse(scheduleIdString);
                if (scheduleId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<ScheduleEntry>.GetPropertyName(p => p.ScheduleId, false)));
                }
                Schedule schedule = context.GetSchedule(scheduleId, true);
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<ScheduleEntryView> scheduleEntryViewList = context.GetScheduleEntryViewsForScheduleByFilter(searchText, scheduleId);
                EntityCache<Guid, ScheduleEntryCsv> cache = new EntityCache<Guid, ScheduleEntryCsv>();
                foreach (ScheduleEntryView v in scheduleEntryViewList)
                {
                    ScheduleEntryCsv csv = new ScheduleEntryCsv();
                    csv.CopyPropertiesFromScheduleEntryView(v);
                    cache.Add(csv.ScheduleEntryId, csv);
                }
                return GetCsvFileResult<ScheduleEntryCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> scheduleEntryId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!scheduleEntryId.HasValue)
                {
                    return PartialView(EDIT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, new ScheduleEntryModel());
                }
                ScheduleEntryView scheduleEntryView = context.GetScheduleEntryView(scheduleEntryId.Value, true);
                ScheduleEntryModel model = new ScheduleEntryModel();
                model.CopyPropertiesFromScheduleEntryView(scheduleEntryView);
                PartialViewResult result = PartialView(EDIT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(ScheduleEntryModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                ScheduleEntry scheduleEntry = context.GetScheduleEntry(model.ScheduleEntryId, true);
                model.CopyPropertiesToScheduleEntry(scheduleEntry);
                context.Save<ScheduleEntry>(scheduleEntry, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult ShiftDialog(Nullable<Guid> scheduleEntryId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!scheduleEntryId.HasValue)
                {
                    return PartialView(SHIFT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, new ScheduleEntryModel());
                }
                ScheduleEntryView scheduleEntryView = context.GetScheduleEntryView(scheduleEntryId.Value, true);
                ScheduleEntryModel model = new ScheduleEntryModel();
                model.CopyPropertiesFromScheduleEntryView(scheduleEntryView);
                model.EntryDateShift = model.EntryDate;
                PartialViewResult result = PartialView(SHIFT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, model);
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
        public ActionResult ShiftDialog(ScheduleEntryModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                ScheduleEntry scheduleEntry = context.GetScheduleEntry(model.ScheduleEntryId, true);
                scheduleEntry.SMSNotificationSent = model.SMSNotificationSent;
                if (!scheduleEntry.SMSNotificationSent) //Update the other fields (outside of the Entry and Notification dates).
                {
                    scheduleEntry.SMSMessageId = null;
                    scheduleEntry.SMSDateSent = null;
                    scheduleEntry.SmsSentLogId = null;
                }
                context.Save<ScheduleEntry>(scheduleEntry, false);
                context.ShiftScheduleEntry(model.ScheduleEntryId, model.EntryDateShift, "zaf", 0); //Update the Entry and Notification dates.
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult CreateDialog(Nullable<Guid> scheduleId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                Schedule schedule = null;
                if (!scheduleId.HasValue)
                {
                    return PartialView(CREATE_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, new ScheduleEntryModel());
                }
                else
                {
                    schedule = context.GetSchedule(scheduleId.Value, true);
                }
                return PartialView(CREATE_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, new ScheduleEntryModel()
                {
                    ScheduleId = scheduleId.Value,
                    NotificationMessage = schedule != null ? schedule.NotificationMessage: null,
                    EntryTime = new TimeSpan(9, 0, 0),
                    EntryDateCreate = DateTime.Now,
                    NotificationDateCreate = DateTime.Now
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
        public ActionResult CreateDialog(ScheduleEntryModel model)
        {
            try
            {
                model.ScheduleEntryId = Guid.NewGuid();
                model.EntryDate = model.EntryDateCreate;
                model.EntryDateFormatted = DataShaper.GetDefaultDateString(model.EntryDate);
                model.NotificationDate = model.NotificationDateCreate;
                model.NotificationDateFormatted = DataShaper.GetDefaultDateString(model.NotificationDate);
                model.DateCreated = DateTime.Now;
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                ScheduleEntry scheduleEntry = new ScheduleEntry();
                model.CopyPropertiesToScheduleEntry(scheduleEntry);
                context.Save<ScheduleEntry>(scheduleEntry, false);
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
