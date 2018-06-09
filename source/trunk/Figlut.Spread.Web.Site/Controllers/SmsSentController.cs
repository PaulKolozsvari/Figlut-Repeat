namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Csv;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.SMS;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class SmsSentController : SpreadController
    {
        #region Constants

        private const string SMS_SENT_GRID_PARTIAL_VIEW_NAME = "_SmsSentGrid";
        private const string SEND_SMS_DIALOG_PARTIAL_VIEW_NAME = "_SendSmsDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<SmsSentLogModel> GetSmsSentFilterModel(SpreadEntityContext context, FilterModel<SmsSentLogModel> model)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            DateTime startDate;
            DateTime endDate;
            if (!model.StartDate.HasValue || !model.EndDate.HasValue) //Has not been specified by the user on the page yet i.e. this is the first time the page is loading.
            {
                context.GetGlobalSettingSmsDaysDateRange(out startDate, out endDate);
                model.StartDate = startDate;
                model.EndDate = endDate;
            }
            else
            {
                if (model.StartDate > model.EndDate)
                {
                    ViewBag.ErrorMessage = string.Format("{0} may not be later than {1}.",
                        EntityReader<FilterModel<SmsSentLogModel>>.GetPropertyName(p => p.StartDate, true),
                        EntityReader<FilterModel<SmsSentLogModel>>.GetPropertyName(p => p.EndDate, true));
                    return null;
                }
                startDate = model.StartDate.Value;
                endDate = model.EndDate.Value;
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            int maxiumSmsDateRangeDaysToDisplay = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaximumSmsDateRangeDaysToDisplay].SettingValue);
            if (!model.IsAdministrator && endDate.Date.Subtract(startDate.Date).Days > maxiumSmsDateRangeDaysToDisplay)
            {
                ViewBag.ErrorMessage = string.Format("Date range may not be greater than {0} days.", maxiumSmsDateRangeDaysToDisplay);
                return null;
            }
            bool smsContentsTrimOnGrid = Convert.ToBoolean(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.SmsContentsTrimOnGrid].SettingValue);
            int smsContentsTrimLengthOnGrid = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.SmsContentsTrimLengthOnGrid].SettingValue);
            int smsErrorTrimLengthOnGrid = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.SmsErrorTrimLengthOnGrid].SettingValue);
            bool cellPhoneNumberTrimOnGrid = Convert.ToBoolean(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.CellPhoneNumberTrimOnGrid].SettingValue);
            int cellPhoneNumberTrimLengthOnGrid = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.CellPhoneNumberTrimLengthOnGrid].SettingValue);
            bool trimCellPhoneNumber = model.IsAdministrator ? false : cellPhoneNumberTrimOnGrid;
            List<SmsSentLog> smsSentLogList = null;
            if (model.IsAdministrator)
            {
                smsSentLogList = context.GetSmsSentLogByFilter(model.SearchText, startDate, endDate, null);
            }
            else
            {
                User currentUser = GetCurrentUser(context);
                if (!currentUser.OrganizationId.HasValue)
                {
                    SetViewBagErrorMessage(string.Format("User {0} is not part of an Organization.", currentUser.UserName));
                    return null;
                }
                smsSentLogList = context.GetSmsSentLogByFilter(model.SearchText, startDate, endDate, currentUser.OrganizationId.Value);
            }
            List<SmsSentLogModel> modelList = new List<SmsSentLogModel>();
            foreach (SmsSentLog s in smsSentLogList)
            {
                SmsSentLogModel m = new SmsSentLogModel();
                m.CopyPropertiesFromSmsSentLog(s);
                if (smsContentsTrimOnGrid)
                {
                    m.MessageContentsTrimmed = DataEditor.GetTrimmedMessageContents(s.MessageContents, smsContentsTrimLengthOnGrid);
                }
                m.ErrorMessageTrimmed = DataEditor.GetTrimmedSmsErrorMessage(s.ErrorMessage, smsErrorTrimLengthOnGrid);
                if (trimCellPhoneNumber)
                {
                    m.CellPhoneNumber = DataEditor.GetTrimmedCellPhoneNumber(m.CellPhoneNumber, 8);
                }
                if (m.SenderUserId.HasValue)
                {
                    User sender = context.GetUser(m.SenderUserId.Value, false);
                    m.SenderUserName = sender != null ? sender.UserName : null;
                }
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllSmsSentLogCount();
            return model;
        }

        #endregion //Methods

        #region Actions

        #region Compose

        public ActionResult SendSms()
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                RefreshSmsMessageTemplatesList(context);
                SendSmsModel model = new SendSmsModel();
                model.MaxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                PartialViewResult result = PartialView(SEND_SMS_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult SendSms(SendSmsModel model)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                RefreshSmsMessageTemplatesList(context);
                string errorMessage = null;
                int maxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                string organizationIdentifierIndicator = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.OrganizationIdentifierIndicator].SettingValue;
                string smsSendMessageSuffix = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.SmsSendMessageSuffix].SettingValue;
                if (!model.IsValid(out errorMessage, maxSmsSendMessageLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                User currentUser = GetCurrentUser(context);
                Organization currentOrganization = GetCurrentOrganization(context, true);
                if (!CurrentUserHasAccessToOrganization(currentOrganization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                if (currentOrganization.SmsCreditsBalance < 1 && !currentOrganization.AllowSmsCreditsDebt)
                {
                    return GetJsonResult(false, string.Format("{0} '{1}' has insufficient SMS credits to send an SMS.", typeof(Organization).Name, currentOrganization.Name));
                }
                SmsResponse response;
                try
                {
                    response = SpreadWebApp.Instance.SmsSender.SendSms(new SmsRequest(
                        model.CellPhoneNumberSendSmsDialog, model.MessageContentsSendSmsDialog, maxSmsSendMessageLength, smsSendMessageSuffix, currentOrganization.Identifier, organizationIdentifierIndicator));
                }
                catch (Exception exFailed) //Failed to send the SMS Web Request to the provider.
                {
                    int smsProviderCode = (int)SpreadWebApp.Instance.Settings.SmsProvider;
                    context.LogFailedSmsSent(
                        model.CellPhoneNumberSendSmsDialog, model.MessageContentsSendSmsDialog, smsProviderCode, exFailed, currentUser, out errorMessage);
                    return GetJsonResult(false, errorMessage);
                }
                SmsSentLog smsSentLog = SpreadWebApp.Instance.LogSmsSentToDB(
                    model.CellPhoneNumberSendSmsDialog, model.MessageContentsSendSmsDialog, response, currentUser, true, null, null, null, null); //If this line throws an exception then we don't havea record of the SMS having been sent, therefore cannot deduct credits from the Organization. Therefore there's no point in wrapping this call in a try catch and swallowing any exception i.e. if we don't have a record of the SMS having been sent we cannot charge for it.
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

        public ActionResult ComposeStandalone()
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                int maxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                return View(new ComposeSmsStandaloneModel() { MaxSmsSendMessageLength = maxSmsSendMessageLength }); ;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ComposeStandalone(ComposeSmsStandaloneModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                string errorMessage = null;
                int maxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                string organizationIdentifierIndicator = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.OrganizationIdentifierIndicator].SettingValue;
                string smsSendMessageSuffix = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.SmsSendMessageSuffix].SettingValue;
                if (!model.IsValid(out errorMessage, maxSmsSendMessageLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                User currentUser = GetCurrentUser(context);
                Organization currentOrganization = GetCurrentOrganization(context, true);
                if (!CurrentUserHasAccessToOrganization(currentOrganization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                if (currentOrganization.SmsCreditsBalance < 1 && !currentOrganization.AllowSmsCreditsDebt)
                {
                    return GetJsonResult(false, string.Format("{0} '{1}' has insufficient SMS credits to send an SMS.", typeof(Organization).Name, currentOrganization.Name));
                }
                SmsResponse response;
                try
                {
                    response = SpreadWebApp.Instance.SmsSender.SendSms(new SmsRequest(model.CellPhoneNumber, model.MessageContents, maxSmsSendMessageLength, smsSendMessageSuffix, currentOrganization.Identifier, organizationIdentifierIndicator));
                }
                catch (Exception exFailed) //Failed to send the SMS Web Request to the provider.
                {
                    int smsProviderCode = (int)SpreadWebApp.Instance.Settings.SmsProvider;
                    context.LogFailedSmsSent(model.CellPhoneNumber, model.MessageContents, smsProviderCode, exFailed, currentUser, out errorMessage);
                    return GetJsonResult(false, errorMessage);
                }
                SpreadWebApp.Instance.LogSmsSentToDB(model.CellPhoneNumber, model.MessageContents, response, currentUser, true, null, null, null, null); //If this line throws an exception then we don't havea record of the SMS having been sent, therefore cannot deduct credits from the Organization. Therefore there's no point in wrapping this call in a try catch and swallowing any exception i.e. if we don't have a record of the SMS having been sent we cannot charge for it.
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

        #endregion //Compose

        public ActionResult Index()
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<SmsSentLogModel> model = GetSmsSentFilterModel(context, new FilterModel<SmsSentLogModel>());
                if (model == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return View(new FilterModel<SmsSentLogModel>());
                }
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
        public ActionResult Index(FilterModel<SmsSentLogModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                FilterModel<SmsSentLogModel> resultModel = GetSmsSentFilterModel(context, model);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(SMS_SENT_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsSentLogModel>());
                }
                return PartialView(SMS_SENT_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid smsSentLogId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                SmsSentLog smsSent = context.GetSmsSentLog(smsSentLogId, true);
                context.Delete<SmsSentLog>(smsSent);
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
                SmsSentLog smsSent = context.GetSmsSentLog(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (smsSent != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete SMS '{0}' ?", !string.IsNullOrEmpty(smsSent.MessageId) ? smsSent.MessageId : smsSent.MessageContents);
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
                SmsSentLog smsSent = context.GetSmsSentLog(model.Identifier, true);
                context.Delete<SmsSentLog>(smsSent);
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
                Nullable<DateTime> startDate;
                Nullable<DateTime> endDate;
                base.GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText, out startDate, out endDate);

                model.SearchText = searchText;
                model.StartDate = startDate;
                model.EndDate = endDate;

                if (model.StartDate.HasValue && model.EndDate.HasValue)
                {
                    model.ConfirmationMessage = string.Format(
                        "Delete all SMS' currently loaded between {0}-{1}-{2} and {3}-{4}-{5} ?",
                        model.StartDate.Value.Year.ToString(),
                        model.StartDate.Value.Month.ToString(),
                        model.StartDate.Value.Day.ToString(),
                        model.EndDate.Value.Year.ToString(),
                        model.EndDate.Value.Month.ToString(),
                        model.EndDate.Value.Day.ToString());
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
        public ActionResult ConfirmDeleteAllDialog(ConfirmationModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (IsCurrentUserAdministrator(context))
                {
                    context.DeleteSmsSentLogByFilter(model.SearchText, model.StartDate.Value, model.EndDate.Value, null);
                }
                else
                {
                    Organization organization = GetCurrentOrganization(context, true);
                    context.DeleteSmsSentLogByFilter(model.SearchText, model.StartDate.Value, model.EndDate.Value, organization.OrganizationId);
                }
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
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                string[] searchParameters;
                string searchText;
                Nullable<DateTime> startDate;
                Nullable<DateTime> endDate;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText, out startDate, out endDate);
                if (!startDate.HasValue)
                {
                    throw new ArgumentException("Start Date not specified.");
                }
                if (!endDate.HasValue)
                {
                    throw new ArgumentException("Start Date not specified.");
                }
                List<SmsSentLog> smsList = null;
                if (IsCurrentUserAdministrator(context))
                {
                    smsList = context.GetSmsSentLogByFilter(searchText, startDate.Value, endDate.Value, null);
                }
                else
                {
                    Organization organization = GetCurrentOrganization(context, true);
                    smsList = context.GetSmsSentLogByFilter(searchText, startDate.Value, endDate.Value, organization.OrganizationId);
                }
                bool cellPhoneNumberTrimOnGrid = Convert.ToBoolean(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.CellPhoneNumberTrimOnGrid].SettingValue);
                int cellPhoneNumberTrimLengthOnGrid = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.CellPhoneNumberTrimLengthOnGrid].SettingValue);
                bool trimCellPhoneNumber = IsCurrentUserAdministrator(context) ? false : cellPhoneNumberTrimOnGrid;
                EntityCache<Guid, SmsSentCsv> cache = new EntityCache<Guid, SmsSentCsv>();
                foreach (SmsSentLog s in smsList)
                {
                    SmsSentCsv csv = new SmsSentCsv();
                    string senderUserName = null;
                    if (s.SenderUserId.HasValue)
                    {
                        User sender = context.GetUser(s.SenderUserId.Value, false);
                        senderUserName = sender != null ? sender.UserName : null;
                    }
                    csv.CopyPropertiesFromSmsSentLog(s, senderUserName);
                    if (trimCellPhoneNumber)
                    {
                        csv.CellPhoneNumber = DataEditor.GetTrimmedCellPhoneNumber(csv.CellPhoneNumber, 8);
                    }
                    cache.Add(csv.SmsSentLogId, csv);
                }
                return GetCsvFileResult<SmsSentCsv>(cache);
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
