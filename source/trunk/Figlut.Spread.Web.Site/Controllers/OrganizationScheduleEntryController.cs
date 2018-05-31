﻿namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Views;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Spread.SMS;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Server.Toolkit.Data;
    using System.Text;
    using Figlut.Spread.ORM.Csv;

    #endregion //Using Directives

    public class OrganizationScheduleEntryController : SpreadController
    {
        #region Constants

        private const string ORGANIZATION_SCHEDULE_ENTRY_GRID_PARTIAL_VIEW_NAME = "_OrganizationScheduleEntryGrid";

        #endregion //Constants

        #region Methods

        private FilterModel<ScheduleEntryModel> GetScheduleEntryFilterModelForOrganization(
            SpreadEntityContext context,
            FilterModel<ScheduleEntryModel> model,
            Nullable<Guid> organizationId)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            if (!model.StartDate.HasValue)
            {
                model.StartDate = DateTime.Now;
            }
            if (!model.EndDate.HasValue)
            {
                model.EndDate = DateTime.Now;
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<ScheduleEntryView> scheduleEntryViewList = context.GetScheduleEntryViewsForOrganizationByFilter(model.SearchText, organizationId, model.StartDate.Value);
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
            if (organizationId.HasValue)
            {
                Organization organization = context.GetOrganization(organizationId.Value, false);
                if (organization != null)
                {
                    model.ParentId = organization.OrganizationId;
                    model.ParentCaption = string.Format("{0}", organization.Name);
                }
            }
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index(Guid organizationId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<ScheduleEntryModel> model = GetScheduleEntryFilterModelForOrganization(
                    context,
                    new FilterModel<ScheduleEntryModel>(),
                    organizationId);
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
                    GetScheduleEntryFilterModelForOrganization(context, model, model.ParentId) :
                    GetScheduleEntryFilterModelForOrganization(context, model, null);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(ORGANIZATION_SCHEDULE_ENTRY_GRID_PARTIAL_VIEW_NAME, new FilterModel<ScheduleEntryModel>());
                }
                return PartialView(ORGANIZATION_SCHEDULE_ENTRY_GRID_PARTIAL_VIEW_NAME, resultModel);
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
                        scheduleView.CellPhoneNumber, scheduleView.NotificationMessage, maxSmsSendMessageLength, smsSendMessageSuffix, currentOrganization.Identifier, organizationIdentifierIndicator));
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
                Nullable< DateTime> startDate = null;
                Nullable<DateTime> endDate = null;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText, out startDate, out endDate);

                string organizationIdString = searchParameters[searchParameters.Length - 1];
                Guid organizationId = Guid.Parse(organizationIdString);
                if (organizationId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<Organization>.GetPropertyName(p => p.OrganizationId, false)));
                }
                if (!startDate.HasValue || startDate.Value == new DateTime())
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<ScheduleEntry>.GetPropertyName(p => p.EntryDate, false)));
                }
                Organization organization = context.GetOrganization(organizationId, true);
                model.ParentId = organization.OrganizationId;
                model.ParentCaption = string.Format("{0}", organization.Name);
                model.SearchText = searchText;
                model.StartDate = startDate;
                model.EndDate = endDate;
                model.ConfirmationMessage = string.Format("Delete all Schedules Entries currently loaded for {0}?", DataShaper.GetDefaultDateString(model.StartDate.Value));
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
                        EntityReader<Organization>.GetPropertyName(p => p.OrganizationId, false)));
                }
                if (!model.StartDate.HasValue || model.StartDate == new DateTime())
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<ScheduleEntry>.GetPropertyName(p => p.EntryDate, false)));
                }
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                context.DeleteScheduleEntriesForOrganizationByFilter(model.SearchText, model.ParentId, model.StartDate.Value);
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
                Nullable<DateTime> startDate = null;
                Nullable<DateTime> endDate = null;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText, out startDate, out endDate);

                string organizationIdString = searchParameters[searchParameters.Length - 1];
                Guid organizationId = Guid.Parse(organizationIdString);
                if (organizationId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<Organization>.GetPropertyName(p => p.OrganizationId, false)));
                }
                if (!startDate.HasValue || startDate.Value == new DateTime())
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<ScheduleEntry>.GetPropertyName(p => p.EntryDate, false)));
                }
                Organization organization = context.GetOrganization(organizationId, true);
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<ScheduleEntryView> scheduleEntryViewList = context.GetScheduleEntryViewsForOrganizationByFilter(searchText, organizationId, startDate.Value);
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

        #endregion //Actions
    }
}