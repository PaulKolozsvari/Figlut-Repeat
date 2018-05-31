﻿namespace Figlut.Spread.Web.Site.Controllers
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

    public class RepeatScheduleEntryController : SpreadController
    {
        #region Constants

        private const string REPEAT_SCHEDULE_ENTRY_GRID_PARTIAL_VIEW_NAME = "_RepeatScheduleEntryGrid";
        private const string EDIT_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME = "_EditRepeatScheduleEntryDialog";
        private const string SHIFT_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME = "_ShiftRepeatScheduleEntryDialog";
        private const string CREATE_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME = "_CreateRepeatScheduleEntryDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<RepeatScheduleEntryModel> GetRepeatScheduleEntryFilterModelForSchedule(
            SpreadEntityContext context,
            FilterModel<RepeatScheduleEntryModel> model,
            Nullable<Guid> repeatScheduleId)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<RepeatScheduleEntryView> repeatScheduleEntryViewList = context.GetRepeatScheduleEntryViewsForScheduleByFilter(model.SearchText, repeatScheduleId);
            List<RepeatScheduleEntryModel> modelList = new List<RepeatScheduleEntryModel>();
            foreach (RepeatScheduleEntryView v in repeatScheduleEntryViewList)
            {
                RepeatScheduleEntryModel m = new RepeatScheduleEntryModel();
                m.CopyPropertiesFromRepeatScheduleEntryView(v);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllRepeatScheduleEntryCount();
            if (repeatScheduleId.HasValue)
            {
                RepeatScheduleView repeatSchedule = context.GetRepeatScheduleView(repeatScheduleId.Value, false);
                if (repeatSchedule != null)
                {
                    model.ParentId = repeatSchedule.RepeatScheduleId;
                    model.ParentCaption = string.Format("{0} {1} {2}", repeatSchedule.ScheduleName, repeatSchedule.CustomerFullName, repeatSchedule.CellPhoneNumber);
                }
            }
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index(Guid repeatScheduleId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<RepeatScheduleEntryModel> model = GetRepeatScheduleEntryFilterModelForSchedule(
                    context,
                    new FilterModel<RepeatScheduleEntryModel>(),
                    repeatScheduleId);
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
        public ActionResult Index(FilterModel<RepeatScheduleEntryModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<RepeatScheduleEntryModel> resultModel = model.ParentId != Guid.Empty ?
                    GetRepeatScheduleEntryFilterModelForSchedule(context, model, model.ParentId) :
                    GetRepeatScheduleEntryFilterModelForSchedule(context, model, null);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(REPEAT_SCHEDULE_ENTRY_GRID_PARTIAL_VIEW_NAME, new FilterModel<RepeatScheduleEntryModel>());
                }
                return PartialView(REPEAT_SCHEDULE_ENTRY_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult SendSms(Nullable<Guid> repeatScheduleEntryId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!repeatScheduleEntryId.HasValue || repeatScheduleEntryId.Value == Guid.Empty)
                {
                    return GetJsonResult(false, string.Format("{0} not specified for sending an SMS.", EntityReader<RepeatScheduleEntry>.GetPropertyName(p => p.RepeatScheduleEntryId, false)));
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
                RepeatScheduleEntry repeatScheduleEntry = context.GetRepeatScheduleEntry(repeatScheduleEntryId.Value, true);
                RepeatScheduleView repeatScheduleView = context.GetRepeatScheduleView(repeatScheduleEntry.RepeatScheduleId, true);
                if (currentOrganization.SmsCreditsBalance < 1 && !currentOrganization.AllowSmsCreditsDebt)
                {
                    return GetJsonResult(false, string.Format("{0} '{1}' has insufficient SMS credits to send an SMS.", typeof(Organization).Name, currentOrganization.Name));
                }
                SmsResponse response;
                try
                {
                    response = SpreadWebApp.Instance.SmsSender.SendSms(new SmsRequest(
                        repeatScheduleView.CellPhoneNumber, repeatScheduleView.NotificationMessage, maxSmsSendMessageLength, smsSendMessageSuffix, currentOrganization.Identifier, organizationIdentifierIndicator));
                }
                catch (Exception exFailed) //Failed to send the SMS Web Request to the provider.
                {
                    int smsProviderCode = (int)SpreadWebApp.Instance.Settings.SmsProvider;
                    context.LogFailedSmsSent(
                        repeatScheduleView.CellPhoneNumber, repeatScheduleView.NotificationMessage, smsProviderCode, exFailed, currentUser, out errorMessage);
                    return GetJsonResult(false, errorMessage);
                }
                SmsSentLog smsSentLog = SpreadWebApp.Instance.LogSmsSentToDB(
                    repeatScheduleView.CellPhoneNumber, repeatScheduleView.NotificationMessage, response, currentUser, true); //If this line throws an exception then we don't havea record of the SMS having been sent, therefore cannot deduct credits from the Organization. Therefore there's no point in wrapping this call in a try catch and swallowing any exception i.e. if we don't have a record of the SMS having been sent we cannot charge for it.
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

                    repeatScheduleEntry.SMSNotificationSent = true;
                    repeatScheduleEntry.SMSMessageId = response.messageId;
                    repeatScheduleEntry.SMSDateSent = DateTime.Now;
                    if (smsSentLog != null)
                    {
                        repeatScheduleEntry.SmsSentLogId = smsSentLog.SmsSentLogId;
                    }
                    context.Save<RepeatScheduleEntry>(repeatScheduleEntry, false);
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
        public ActionResult Delete(Guid repeatScheduleEntryId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                RepeatScheduleEntry repeatScheduleEntry = context.GetRepeatScheduleEntry(repeatScheduleEntryId, true);
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                context.Delete<RepeatScheduleEntry>(repeatScheduleEntry);
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
                RepeatScheduleEntry repeatScheduleEntry = context.GetRepeatScheduleEntry(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (repeatScheduleEntry != null)
                {
                    RepeatScheduleView repeatSchedule = context.GetRepeatScheduleView(repeatScheduleEntry.RepeatScheduleId, true);
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete entry '{0}' for {1} '{2}' ({3} {4})?",
                        DataShaper.GetDefaultDateString(repeatScheduleEntry.RepeatDate),
                        DataShaper.ShapeCamelCaseString(typeof(RepeatSchedule).Name),
                        repeatSchedule.ScheduleName,
                        repeatSchedule.CustomerFullName,
                        repeatSchedule.CellPhoneNumber);
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
                RepeatScheduleEntry repeatScheduleEntry = context.GetRepeatScheduleEntry(model.Identifier, true);
                context.Delete<RepeatScheduleEntry>(repeatScheduleEntry);
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

                string repeatScheduleIdString = searchParameters[searchParameters.Length - 1];
                Guid repeatScheduleId = Guid.Parse(repeatScheduleIdString);
                if (repeatScheduleId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<RepeatScheduleEntry>.GetPropertyName(p => p.RepeatScheduleId, false)));
                }
                RepeatScheduleView repeatSchedule = context.GetRepeatScheduleView(repeatScheduleId, true);
                model.ParentId = repeatSchedule.RepeatScheduleId;
                model.ParentCaption = string.Format("{0} {1} {2}", repeatSchedule.ScheduleName, repeatSchedule.CustomerFullName, repeatSchedule.CellPhoneNumber);
                model.SearchText = searchText;
                model.ConfirmationMessage = string.Format("Delete all Repeat Schedules Entries currently loaded for {0} '{1}' for subscription {2} {3}'?", 
                    DataShaper.ShapeCamelCaseString(typeof(RepeatSchedule).Name), 
                    repeatSchedule.ScheduleName,
                    repeatSchedule.CustomerFullName,
                    repeatSchedule.CellPhoneNumber);
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
                        EntityReader<RepeatScheduleEntry>.GetPropertyName(p => p.RepeatScheduleId, false)));
                }
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                context.DeleteRepeatScheduleEntriesForScheduleByFilter(model.SearchText, model.ParentId);
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

                string repeatScheduleIdString = searchParameters[searchParameters.Length - 1];
                Guid repeatScheduleId = Guid.Parse(repeatScheduleIdString);
                if (repeatScheduleId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<RepeatScheduleEntry>.GetPropertyName(p => p.RepeatScheduleId, false)));
                }
                RepeatSchedule repeatSchedule = context.GetRepeatSchedule(repeatScheduleId, true);
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<RepeatScheduleEntryView> repeatScheduleEntryViewList = context.GetRepeatScheduleEntryViewsForScheduleByFilter(searchText, repeatScheduleId);
                EntityCache<Guid, RepeatScheduleEntryCsv> cache = new EntityCache<Guid, RepeatScheduleEntryCsv>();
                foreach (RepeatScheduleEntryView v in repeatScheduleEntryViewList)
                {
                    RepeatScheduleEntryCsv csv = new RepeatScheduleEntryCsv();
                    csv.CopyPropertiesFromRepeatScheduleEntryView(v);
                    cache.Add(csv.RepeatScheduleEntryId, csv);
                }
                return GetCsvFileResult<RepeatScheduleEntryCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> repeatScheduleEntryId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!repeatScheduleEntryId.HasValue)
                {
                    return PartialView(EDIT_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, new RepeatScheduleEntryModel());
                }
                RepeatScheduleEntryView repeatScheduleEntryView = context.GetRepeatScheduleEntryView(repeatScheduleEntryId.Value, true);
                RepeatScheduleEntryModel model = new RepeatScheduleEntryModel();
                model.CopyPropertiesFromRepeatScheduleEntryView(repeatScheduleEntryView);
                PartialViewResult result = PartialView(EDIT_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(RepeatScheduleEntryModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                RepeatScheduleEntry repeatScheduleEntry = context.GetRepeatScheduleEntry(model.RepeatScheduleEntryId, true);
                model.CopyPropertiesToRepeatScheduleEntry(repeatScheduleEntry);
                context.Save<RepeatScheduleEntry>(repeatScheduleEntry, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult ShiftDialog(Nullable<Guid> repeatScheduleEntryId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!repeatScheduleEntryId.HasValue)
                {
                    return PartialView(SHIFT_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, new RepeatScheduleEntryModel());
                }
                RepeatScheduleEntryView repeatScheduleEntryView = context.GetRepeatScheduleEntryView(repeatScheduleEntryId.Value, true);
                RepeatScheduleEntryModel model = new RepeatScheduleEntryModel();
                model.CopyPropertiesFromRepeatScheduleEntryView(repeatScheduleEntryView);
                model.RepeatDateShift = model.RepeatDate;
                PartialViewResult result = PartialView(SHIFT_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, model);
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
        public ActionResult ShiftDialog(RepeatScheduleEntryModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                RepeatScheduleEntry repeatScheduleEntry = context.GetRepeatScheduleEntry(model.RepeatScheduleEntryId, true);
                repeatScheduleEntry.SMSNotificationSent = model.SMSNotificationSent;
                if (!repeatScheduleEntry.SMSNotificationSent) //Update the other fields (outside of the Repeat and Notification dates).
                {
                    repeatScheduleEntry.SMSMessageId = null;
                    repeatScheduleEntry.SMSDateSent = null;
                    repeatScheduleEntry.SmsSentLogId = null;
                }
                context.Save<RepeatScheduleEntry>(repeatScheduleEntry, false);
                context.ShiftRepeatScheduleEntry(model.RepeatScheduleEntryId, model.RepeatDateShift, "zaf", 0); //Update the Repeat and Notification dates.
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult CreateDialog(Nullable<Guid> repeatScheduleId)
        {
            try
            {
                if (!repeatScheduleId.HasValue)
                {
                    return PartialView(CREATE_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, new RepeatScheduleEntryModel());
                }
                return PartialView(CREATE_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, new RepeatScheduleEntryModel()
                {
                    RepeatScheduleId = repeatScheduleId.Value,
                    RepeatDateCreate = DateTime.Now,
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
        public ActionResult CreateDialog(RepeatScheduleEntryModel model)
        {
            try
            {
                model.RepeatScheduleEntryId = Guid.NewGuid();
                model.RepeatDate = model.RepeatDateCreate;
                model.RepeatDateFormatted = DataShaper.GetDefaultDateString(model.RepeatDate);
                model.NotificationDate = model.NotificationDateCreate;
                model.NotificationDateFormatted = DataShaper.GetDefaultDateString(model.NotificationDate);
                model.DateCreated = DateTime.Now;
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                RepeatScheduleEntry repeatScheduleEntry = new RepeatScheduleEntry();
                model.CopyPropertiesToRepeatScheduleEntry(repeatScheduleEntry);
                context.Save<RepeatScheduleEntry>(repeatScheduleEntry, false);
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