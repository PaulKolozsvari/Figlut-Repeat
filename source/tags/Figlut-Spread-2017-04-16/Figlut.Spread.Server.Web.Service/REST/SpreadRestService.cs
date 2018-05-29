namespace Figlut.Spread.Web.Service.REST
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Server.Toolkit.Web.Client;
    using Figlut.Server.Toolkit.Web.Service.REST;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.SMS;
    using Figlut.Spread.Web.Service.Configuration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SpreadRestService : RestService, ISpreadRestService
    {
        #region Methods

        #region Utility Methods

        protected User GetCurrentUser(SpreadEntityContext context)
        {
            User result = null;
            if (ServiceSecurityContext.Current != null && !string.IsNullOrEmpty(ServiceSecurityContext.Current.PrimaryIdentity.Name))
            {
                if (context == null)
                {
                    context = SpreadEntityContext.Create();
                }
                string userIdentifier = ServiceSecurityContext.Current.PrimaryIdentity.Name;
                result = SpreadEntityContext.Create().GetUserByIdentifier(userIdentifier, true);
            }
            return result;
        }

        protected Organization GetCurrentOrganization(SpreadEntityContext context, out User currentUser, bool throwExceptionOnNotFound)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
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
            if (SpreadApp.Instance.Settings.AuditServiceCalls)
            {
                GOC.Instance.Logger.LogMessage(new LogMessage(message, LogMessageType.SuccessAudit, LoggingLevel.Maximum));
            }
        }

        #endregion //Utility Methods

        #region Spread Methods

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
                UpdateHttpStatusOnException(ex);
                throw ex;
            }
        }

        #endregion //Spread Methods

        #region Zoom Connect Rest Methods

        public Stream SendSms(Stream inputStream)
        {
            try
            {
                ValidateRequestMethod(HttpVerb.POST);
                SpreadEntityContext context = SpreadEntityContext.Create();
                User user = null;
                Organization organization = GetCurrentOrganization(context, out user, true);
                if (organization.SmsCreditsBalance < 1 && !organization.AllowSmsCreditsDebt)
                {
                    throw new NullReferenceException(string.Format("{0} '{1}' has insufficient SMS credits to send an SMS.", typeof(Organization).Name, organization.Name));
                }
                SmsRequest smsRequest = GetObjectFromStream<SmsRequest>(inputStream);
                if (string.IsNullOrEmpty(smsRequest.recipientNumber))
                {
                    throw new NullReferenceException(string.Format("{0} may not be null or empty.", EntityReader<SmsRequest>.GetPropertyName(p => p.recipientNumber, true)));
                }
                if (string.IsNullOrEmpty(smsRequest.message))
                {
                    throw new NullReferenceException(string.Format("{0} may not be null or empty.", EntityReader<SmsRequest>.GetPropertyName(p => p.message, true)));
                }
                StringBuilder m = new StringBuilder();
                m.AppendLine("Send SMS:");
                m.AppendLine(string.Format("recipientNumber: {0}", smsRequest.recipientNumber));
                m.AppendLine(string.Format("message: {0}", smsRequest.message));
                AuditServiceCall(m.ToString());
                SmsResponse smsResponse;
                try
                {
                    smsResponse = SpreadApp.Instance.SmsSender.SendSms(smsRequest);
                }
                catch (Exception exFailed) //Failed to send the SMS Web Request to the provider.
                {
                    int smsProviderCode = (int)SpreadApp.Instance.Settings.SmsProvider;
                    string errorMessage = null;
                    context.LogFailedSmsSent(smsRequest.recipientNumber, smsRequest.message, smsProviderCode, exFailed, user, out errorMessage);
                    throw new Exception(errorMessage);
                }
                if (smsResponse != null)
                {
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine(string.Format("Sms Response to {0}", smsRequest.recipientNumber));
                    logMessage.AppendLine(smsResponse.ToString());
                    AuditServiceCall(logMessage.ToString());
                }
                SmsSentLog result = SpreadApp.Instance.LogSmsSentToDB(smsRequest.recipientNumber, smsRequest.message, smsResponse, user, true);
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
                }
                return GetStreamFromObject(result);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                UpdateHttpStatusOnException(ex);
                throw ex;
            }
        }

        #endregion //Zoom Connect Rest Methods

        #endregion //Methods
    }
}