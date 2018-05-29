namespace Figlut.Spread.Receiver.Web.Service.REST
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
    using Figlut.Spread.Receiver.Web.Service.Configuration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SpreadReceiverRestService : RestService, ISpreadReceiverRestService
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
            if (SpreadReceiverApp.Instance.Settings.AuditServiceCalls)
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

        public Stream ReceiveSmsZoomConnect(
            string fromValue,
            string messageIdValue,
            string messageValue,
            string dateValue,
            string campaignValue,
            string dataFieldValue,
            string nonceValue,
            string noncedateValue,
            string checksumValue)
        {
            try
            {
                ValidateRequestMethod(HttpVerb.POST);
                StringBuilder m = new StringBuilder();
                m.AppendLine("Received SMS:");
                m.AppendLine(string.Format("from: {0}", fromValue));
                m.AppendLine(string.Format("messageId: {0}", messageIdValue));
                m.AppendLine(string.Format("message: {0}", messageValue));
                m.AppendLine(string.Format("date: {0}", dateValue));
                m.AppendLine(string.Format("campaign: {0}", campaignValue));
                m.AppendLine(string.Format("dataField: {0}", dataFieldValue));
                m.AppendLine(string.Format("nonce: {0}", nonceValue));
                m.AppendLine(string.Format("nonce-date: {0}", noncedateValue));
                m.AppendLine(string.Format("checksum: {0}", checksumValue));
                AuditServiceCall(m.ToString());
                SmsReceivedQueueItem queueItem = SpreadReceiverApp.Instance.EnqueueSmsReceived(
                    fromValue,
                    messageIdValue,
                    messageValue,
                    dateValue,
                    campaignValue,
                    dataFieldValue,
                    nonceValue,
                    noncedateValue,
                    checksumValue,
                    SmsProvider.Zoom);
                if (queueItem == null)
                {
                    throw new Exception("Invalid SMS received.");
                }
                return StreamHelper.GetStreamFromString("Received SMS enqueued for processing.", GOC.Instance.Encoding);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                UpdateHttpStatusOnException(ex);
                throw ex;
            }
        }

        public Stream DeliveryReportSmsZoomConnect(
            string fromValue,
            string messageIdValue,
            string messageValue,
            string dateValue,
            string campaignValue,
            string dataFieldValue,
            string nonceValue,
            string noncedateValue,
            string checksumValue)
        {
            try
            {
                ValidateRequestMethod(HttpVerb.POST);
                StringBuilder m = new StringBuilder();
                m.AppendLine("Delivery Report SMS:");
                m.AppendLine(string.Format("from: {0}", fromValue));
                m.AppendLine(string.Format("messageId: {0}", messageIdValue));
                m.AppendLine(string.Format("message: {0}", messageValue));
                m.AppendLine(string.Format("date: {0}", dateValue));
                m.AppendLine(string.Format("campaign: {0}", campaignValue));
                m.AppendLine(string.Format("dataField: {0}", dataFieldValue));
                m.AppendLine(string.Format("nonce: {0}", nonceValue));
                m.AppendLine(string.Format("nonce-date: {0}", noncedateValue));
                m.AppendLine(string.Format("checksum: {0}", checksumValue));
                AuditServiceCall(m.ToString());
                SmsDeliveryReportLog smsDeliveryReportLog = SpreadReceiverApp.Instance.LogSmsDeliveryReportToDB(
                    fromValue,
                    messageIdValue,
                    messageValue,
                    dateValue,
                    campaignValue,
                    dataFieldValue,
                    nonceValue,
                    noncedateValue,
                    checksumValue,
                    SmsProvider.Zoom);
                if (smsDeliveryReportLog == null)
                {
                    throw new Exception("Invalid SMS delivery report.");
                }
                return StreamHelper.GetStreamFromString("Delivery Report SMS processed.", GOC.Instance.Encoding);
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