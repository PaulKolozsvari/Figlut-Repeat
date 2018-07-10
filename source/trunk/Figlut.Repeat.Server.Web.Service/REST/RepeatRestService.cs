namespace Figlut.Repeat.Web.Service.REST
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Server.Toolkit.Web.Client;
    using Figlut.Server.Toolkit.Web.Service.REST;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Helpers;
    using Figlut.Repeat.SMS;
    using Figlut.Repeat.Web.Service.Configuration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class RepeatRestService : RestService, IRepeatRestService
    {
        #region Methods

        #region Utility Methods

        protected User GetCurrentUser(RepeatEntityContext context)
        {
            User result = null;
            if (ServiceSecurityContext.Current != null && !string.IsNullOrEmpty(ServiceSecurityContext.Current.PrimaryIdentity.Name))
            {
                if (context == null)
                {
                    context = RepeatEntityContext.Create();
                }
                string userIdentifier = ServiceSecurityContext.Current.PrimaryIdentity.Name;
                result = RepeatEntityContext.Create().GetUserByIdentifier(userIdentifier, true);
            }
            return result;
        }

        protected Organization GetCurrentOrganization(RepeatEntityContext context, out User currentUser, bool throwExceptionOnNotFound)
        {
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            currentUser = GetCurrentUser(context);
            if (currentUser == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("No current {0} logged in to be able to retrieve current {1}.",
                    typeof(User).Name,
                    typeof(Organization)));
            }
            else if (currentUser == null) //Current user may be null if authentication has not been enabled on the web service.
            {
                return null;
            }
            if (!currentUser.OrganizationId.HasValue && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("{0} with {1} of '{2}' is not linked to an {3}.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.UserName, false),
                    currentUser.UserName,
                    typeof(Organization)));
            }
            Organization result = context.GetOrganization(currentUser.OrganizationId.Value, false);
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find an {0} with {1} of '{2}' linked to current {3} with {4} of '{5}'.",
                    typeof(Organization).Name,
                    EntityReader<Organization>.GetPropertyName(p => p.OrganizationId, false),
                    currentUser.OrganizationId.Value,
                    typeof(User),
                    EntityReader<User>.GetPropertyName(p => p.UserName, false),
                    currentUser.UserName));
            }
            return result;
        }

        protected void AuditServiceCall(string message)
        {
            if (RepeatApp.Instance.Settings.AuditServiceCalls)
            {
                GOC.Instance.Logger.LogMessage(new LogMessage(message, LogMessageType.SuccessAudit, LoggingLevel.Maximum));
            }
        }

        #endregion //Utility Methods

        #region Repeat Methods

        public Stream Login()
        {
            try
            {
                ValidateRequestMethod(HttpVerb.GET);
                User u = GetCurrentUser(null);
                AuditServiceCall(string.Format("Login: {0}", u.UserName));
                return GetStreamFromObject(u);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                UpdateHttpStatusOnException(ex);
                throw ex;
            }
        }

        #endregion //Repeat Methods

        #region Zoom Connect Rest Methods

        private Organization ValidateOrganizationCredits(RepeatEntityContext context, out User user)
        {
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            user = null;
            Organization result = GetCurrentOrganization(context, out user, true);
            if (result.SmsCreditsBalance < 1 && !result.AllowSmsCreditsDebt)
            {
                throw new NullReferenceException(string.Format("{0} '{1}' has insufficient SMS credits to send an SMS.", typeof(Organization).Name, result.Name));
            }
            return result;
        }

        public Stream SendSmsSynchronously(Stream inputStream)
        {
            try
            {
                ValidateRequestMethod(HttpVerb.POST);
                RepeatEntityContext context = RepeatEntityContext.Create();
                User user = null;
                Organization organization = ValidateOrganizationCredits(context, out user);
                SmsRequest smsRequest = GetObjectFromStream<SmsRequest>(inputStream);
                if (string.IsNullOrEmpty(smsRequest.recipientNumber))
                {
                    throw new UserThrownException("Recipient cell phone number not provided.", LoggingLevel.Normal);
                }
                if (string.IsNullOrEmpty(smsRequest.message))
                {
                    throw new UserThrownException("SMS message contents not provided.", LoggingLevel.Normal);
                }
                int maxSmsSendMessageLength = Convert.ToInt32(RepeatApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                string smsSendMessageSuffix = RepeatApp.Instance.GlobalSettings[GlobalSettingName.SmsSendMessageSuffix].SettingValue;
                string organizationIdentifierIndicator = RepeatApp.Instance.GlobalSettings[GlobalSettingName.OrganizationIdentifierIndicator].SettingValue;
                smsRequest.ValidateMaxSmsSendMessageLength(maxSmsSendMessageLength, smsSendMessageSuffix, organization.Identifier, organizationIdentifierIndicator, true);

                StringBuilder m = new StringBuilder();
                m.AppendLine("Sending SMS:");
                m.AppendLine(string.Format("recipientNumber: {0}", smsRequest.recipientNumber));
                m.AppendLine(string.Format("message: {0}", smsRequest.message));
                AuditServiceCall(m.ToString());
                SmsResponse smsResponse;
                try
                {
                    smsResponse = RepeatApp.Instance.SmsSender.SendSms(smsRequest);
                }
                catch (Exception exFailed) //Failed to send the SMS Web Request to the provider.
                {
                    int smsProviderCode = (int)RepeatApp.Instance.Settings.SmsProvider;
                    string errorMessage = null;
                    context.LogFailedSmsSent(smsRequest.recipientNumber, smsRequest.message, smsProviderCode, exFailed, user, organization, out errorMessage);
                    throw new Exception(errorMessage);
                }
                if (smsResponse != null)
                {
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine(string.Format("Sms Response to {0}", smsRequest.recipientNumber));
                    logMessage.AppendLine(smsResponse.ToString());
                    AuditServiceCall(logMessage.ToString());
                }
                SmsSentLog result = RepeatApp.Instance.LogSmsSentToDB(smsRequest.recipientNumber, smsRequest.message, smsResponse, user, organization, true, null, null, null, null);
                if (smsResponse.success)
                {
                    long smsCredits = context.DecrementSmsCreditFromOrganization(organization.OrganizationId).SmsCreditsBalance;
                    GOC.Instance.Logger.LogMessage(new LogMessage(
                        string.Format("{0} '{1}' has sent an SMS. Credits remaining: {2}.",
                        typeof(Organization).Name,
                        organization.Name,
                        smsCredits),
                        LogMessageType.SuccessAudit, 
                        LoggingLevel.Normal));
                    Subscriber subscriber = context.SaveSubscriber(smsRequest.recipientNumber, null, true, true); //Creates  subscriber for the given cell phone to which this SMS has been sent if the subscriber with the given cell phone number does not already exist.
                    context.UpdateSmsSentLog(result.SmsSentLogId, subscriber.SubscriberId, subscriber.Name, null, null);
                }
                if (RepeatApp.Instance.Settings.TrimSmsSentLogResponseTag && result.Tag.Length > RepeatApp.Instance.Settings.TrimSmsSentLogResponseTagLength)
                {
                    result.Tag = result.Tag.Substring(0, RepeatApp.Instance.Settings.TrimSmsSentLogResponseTagLength);
                }
                return GetStreamFromObject(result);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                UpdateHttpStatusOnException(ex);
                throw ex;
            }
        }

        public Stream SendSmsAsynchronously(Stream inputStream)
        {
            try
            {
                ValidateRequestMethod(HttpVerb.POST);
                RepeatEntityContext context = RepeatEntityContext.Create();
                User user = null;
                Organization organization = ValidateOrganizationCredits(context, out user);
                SmsRequest smsRequest = GetObjectFromStream<SmsRequest>(inputStream);
                int maxSmsSendMessageLength = Convert.ToInt32(RepeatApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                string smsSendMessageSuffix = RepeatApp.Instance.GlobalSettings[GlobalSettingName.SmsSendMessageSuffix].SettingValue;
                string organizationIdentifierIndicator = RepeatApp.Instance.GlobalSettings[GlobalSettingName.OrganizationIdentifierIndicator].SettingValue;
                smsRequest.ValidateMaxSmsSendMessageLength(maxSmsSendMessageLength, smsSendMessageSuffix, organization.Identifier, organizationIdentifierIndicator, true);

                StringBuilder m = new StringBuilder();
                m.AppendLine("Enqueuing SMS to be sent:");
                m.AppendLine(string.Format("recipientNumber: {0}", smsRequest.recipientNumber));
                m.AppendLine(string.Format("message: {0}", smsRequest.message));
                AuditServiceCall(m.ToString());
                SmsSentQueueItem result = context.EnqueueSmsSent(
                    smsRequest.recipientNumber,
                    smsRequest.message,
                    null,
                    null,
                    "WEB-SERVICE-REQUEST",
                    (int)RepeatApp.Instance.Settings.SmsProvider,
                    user.UserId,
                    organization.OrganizationId,
                    null,
                    null,
                    null,
                    null,
                    true,
                    true);
                long smsCredits = context.DecrementSmsCreditFromOrganization(organization.OrganizationId).SmsCreditsBalance;
                GOC.Instance.Logger.LogMessage(new LogMessage(
                    string.Format("{0} '{1}' has enqueued an SMS to be sent. Credits remaining: {2}.",
                    typeof(Organization).Name,
                    organization.Name,
                    smsCredits),
                    LogMessageType.SuccessAudit,
                    LoggingLevel.Normal));
                return GetStreamFromObject(result);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                UpdateHttpStatusOnException(ex);
                throw ex;
            }
        }

        #endregion //Zoom Connect Rest Methods

        #endregion //Methods
    }
}