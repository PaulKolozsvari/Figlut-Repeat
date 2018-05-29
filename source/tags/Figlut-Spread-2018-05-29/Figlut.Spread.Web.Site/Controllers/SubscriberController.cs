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
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class SubscriberController : SpreadController
    {
        #region Constants

        private const string SUBSCRIBER_GRID_PARTIAL_VIEW_NAME = "_SubscriberGrid";
        private const string SEND_SUBSCRIBER_SMS_DIALOG_PARTIAL_VIEW_NAME = "_SendSubscriberSmsDialog";
        private const string EDIT_SUBSCRIBER_DIALOG_PARTIAL_VIEW_NAME = "_EditSubscriberDialog";
        private const string CREATE_SUBSCRIBER_PARTIAL_VIEW_NAME = "_CreateSubscriberDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<SubscriberModel> GetSubscriberFilterModel(SpreadEntityContext context, FilterModel<SubscriberModel> model)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<Subscriber> subscriberList = context.GetSubscribersByFilter(model.SearchText, null);
            List<SubscriberModel> modelList = new List<SubscriberModel>();
            foreach (Subscriber s in subscriberList)
            {
                SubscriberModel m = new SubscriberModel();
                m.CopyPropertiesFromSubscriber(s);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllSubscriberCount();
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index()
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                FilterModel<SubscriberModel> model = GetSubscriberFilterModel(context, new FilterModel<SubscriberModel>());
                if (model == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return View(new FilterModel<SubscriberModel>());
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
        public ActionResult Index(FilterModel<SubscriberModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                FilterModel<SubscriberModel> resultModel = GetSubscriberFilterModel(context, model);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(SUBSCRIBER_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsSentLogModel>());
                }
                return PartialView(SUBSCRIBER_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid subscriberId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                Subscriber subscriber = context.GetSubscriber(subscriberId, true);
                context.Delete<Subscriber>(subscriber);
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
                Subscriber subscriber = context.GetSubscriber(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (subscriber != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete {0} '{1}'?", 
                        typeof(Subscriber).Name,
                        !string.IsNullOrEmpty(subscriber.Name) ? subscriber.Name : subscriber.CellPhoneNumber);
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
                Subscriber subscriber = context.GetSubscriber(model.Identifier, true);
                context.Delete<Subscriber>(subscriber);
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
                model.SearchText = searchText;
                model.ConfirmationMessage = "Delete all Subscribers currently loaded?";
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
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                context.DeleteSubscribersByFilter(model.SearchText, null);
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
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<Subscriber> subscriberList = context.GetSubscribersByFilter(searchText, null);
                EntityCache<Guid, SubscriberCsv> cache = new EntityCache<Guid, SubscriberCsv>();
                foreach (Subscriber s in subscriberList)
                {
                    SubscriberCsv csv = new SubscriberCsv();
                    csv.CopyPropertiesFromSubscriber(s);
                    cache.Add(csv.SubscriberId, csv);
                }
                return GetCsvFileResult<SubscriberCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult SendSms(Nullable<Guid> subscriberId)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!subscriberId.HasValue || subscriberId == Guid.Empty)
                {
                    return PartialView(SEND_SUBSCRIBER_SMS_DIALOG_PARTIAL_VIEW_NAME, new SendSubscriberSmsModel());
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                RefreshSmsMessageTemplatesList(context);
                Subscriber subscriber = context.GetSubscriber(subscriberId.Value, true);
                SendSubscriberSmsModel model = new SendSubscriberSmsModel();
                model.CopyPropertiesFromSubscriber(subscriber);
                model.MaxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                PartialViewResult result = PartialView(SEND_SUBSCRIBER_SMS_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult SendSms(SendSubscriberSmsModel model)
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
                    response = SpreadWebApp.Instance.SmsSender.SendSms(new SmsRequest(
                        model.CellPhoneNumberSendSubscriberSmsDialog, model.MessageContentsSendSubscriberSmsDialog, maxSmsSendMessageLength, smsSendMessageSuffix, currentOrganization.Identifier, organizationIdentifierIndicator));
                }
                catch (Exception exFailed) //Failed to send the SMS Web Request to the provider.
                {
                    int smsProviderCode = (int)SpreadWebApp.Instance.Settings.SmsProvider;
                    context.LogFailedSmsSent(
                        model.CellPhoneNumberSendSubscriberSmsDialog, model.MessageContentsSendSubscriberSmsDialog, smsProviderCode, exFailed, currentUser, out errorMessage);
                    return GetJsonResult(false, errorMessage);
                }
                SmsSentLog smsSentLog = SpreadWebApp.Instance.LogSmsSentToDB(
                    model.CellPhoneNumberSendSubscriberSmsDialog, model.MessageContentsSendSubscriberSmsDialog, response, currentUser, true); //If this line throws an exception then we don't havea record of the SMS having been sent, therefore cannot deduct credits from the Organization. Therefore there's no point in wrapping this call in a try catch and swallowing any exception i.e. if we don't have a record of the SMS having been sent we cannot charge for it.
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

        public ActionResult EditDialog(Nullable<Guid> subscriberId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                if (!subscriberId.HasValue)
                {
                    return PartialView(EDIT_SUBSCRIBER_DIALOG_PARTIAL_VIEW_NAME, new SubscriberModel());
                }
                RefreshSmsMessageTemplatesList(context);
                Subscriber subscriber = context.GetSubscriber(subscriberId.Value, true);
                SubscriberModel model = new SubscriberModel();
                model.CopyPropertiesFromSubscriber(subscriber);
                PartialViewResult result = PartialView(EDIT_SUBSCRIBER_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(SubscriberModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                Subscriber subscriber = context.GetSubscriber(model.SubscriberId, true);
                bool subscriberCellPhoneNUmberHasChanged = subscriber.CellPhoneNumber != model.CellPhoneNumber;
                if (subscriberCellPhoneNUmberHasChanged)
                {
                    Subscriber original = context.GetSubscriberByCellPhoneNumber(model.CellPhoneNumber, false);
                    if (original != null)
                    {
                        return GetJsonResult(false, string.Format("An {0} with the {1} of '{2}' already exists.",
                            typeof(Subscriber).Name,
                            EntityReader<Subscriber>.GetPropertyName(p => p.CellPhoneNumber, true),
                            model.CellPhoneNumber));
                    }
                }
                model.CopyPropertiesToSubsriber(subscriber);
                context.Save<Subscriber>(subscriber, false);
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
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                return PartialView(CREATE_SUBSCRIBER_PARTIAL_VIEW_NAME, new SubscriberModel() { Enabled = true });
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult CreateDialog(SubscriberModel model)
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
                Subscriber originalSubscriber = context.GetSubscriberByCellPhoneNumber(model.CellPhoneNumber, false);
                if (originalSubscriber != null)
                {
                    return GetJsonResult(false, string.Format("A {0} with the {1} of '{2}' already exists.",
                        typeof(Subscriber).Name,
                        EntityReader<Subscriber>.GetPropertyName(p => p.CellPhoneNumber, true),
                        model.CellPhoneNumber));
                }
                model.SubscriberId = Guid.NewGuid();
                model.DateCreated = DateTime.Now;
                Subscriber subscriber = new Subscriber();
                model.CopyPropertiesToSubsriber(subscriber);
                context.Save<Subscriber>(subscriber, false);
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
