namespace Figlut.Spread.SMS.Processor
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SmsSentQueueProcessor : SmsQueueProcessor
    {
        #region Constructors

        public SmsSentQueueProcessor(
            SmsSender smsSender,
            Guid smsProcessorId,
            int executionInterval,
            bool startImmediately,
            string organizationIdentifierIndicator,
            string subscriberNameIndicator) : base(smsProcessorId, executionInterval, startImmediately, organizationIdentifierIndicator, subscriberNameIndicator)
        {
            if (_smsSender == null)
            {
                throw new NullReferenceException(string.Format("{0} may not be null when constructing a {1}.",
                    EntityReader<SmsSentQueueProcessor>.GetPropertyName(p => p.SmsSender, false),
                    typeof(SmsSentQueueProcessor).Name));
            }
        }

        #endregion //Constructors

        #region Fields

        protected SmsSender _smsSender;

        #endregion //Fields

        #region Properties

        public SmsSender SmsSender
        {
            get { return _smsSender; }
            set 
            {
                if (value == null)
                {
                    throw new NullReferenceException(string.Format("{0} may not be null on {1}.",
                        EntityReader<SmsSentQueueProcessor>.GetPropertyName(p => p.SmsSender, false),
                        typeof(SmsSentQueueProcessor).Name));
                }
                _smsSender = value;
            }
        }

        #endregion //Properties

        #region Methods

        protected override bool ProcessNextItemInQueue(SpreadEntityContext context, Guid smsProcessorId, string organizationIdentifierIndicator, string subscriberIdentifierIndicator)
        {
            long itemsInQueue = context.GetAllSmsSentQueueItemCount();
            if (itemsInQueue < 1) //Continue processing items until the queue has been emptied i.e. there are no more items in the queue.
            {
                return false;
            }
            List<SmsSentQueueItem> items = context.GetTopSmsSentQueueItems(1);
            if (items.Count < 1)
            {
                throw new Exception(string.Format("{0} {1}s are supposed to exist, but none could be queried with TOP command.",
                    itemsInQueue,
                    typeof(SmsSentQueueItem).Name));
            }
            foreach (SmsSentQueueItem i in items)
            {
                context.LogSmsProcesorAction(
                    smsProcessorId,
                    string.Format("Executing {0}. {1} items in queue. Processing SMS to '{2}': {3}",
                    DataShaper.ShapeCamelCaseString(typeof(SmsSentQueueProcessor).Name),
                    itemsInQueue,
                    i.CellPhoneNumber,
                    i.MessageContents),
                    LogMessageType.Information.ToString());
                SmsSentLog smsSentLog = ProcessSmsSentQueueItem(i, context);
                if (smsSentLog != null)
                {
                    string message = string.Format("Processed SMS to {0}: {1}", smsSentLog.CellPhoneNumber, smsSentLog.MessageContents);
                    context.LogSmsProcesorAction(smsProcessorId, message, LogMessageType.SuccessAudit.ToString());
                    GOC.Instance.Logger.LogMessage(new LogMessage(message, LogMessageType.SuccessAudit, LoggingLevel.Normal));
                }
            }
            return true;
        }

        protected SmsSentLog ProcessSmsSentQueueItem(SmsSentQueueItem i, SpreadEntityContext context)
        {
            User senderUser = i.SenderUserId.HasValue ? context.GetUser(i.SenderUserId.Value, true) : null;
            Organization senderOrganization = i.OrganizationId.HasValue ? context.GetOrganization(i.OrganizationId.Value, true) : null;

            SmsRequest smsRequest = new SmsRequest(i.CellPhoneNumber, i.MessageContents);
            StringBuilder auditM = new StringBuilder();
            auditM.AppendLine("Sending SMS ...");
            auditM.AppendLine(string.Format("recipientNumber: {0}", smsRequest.recipientNumber));
            auditM.AppendLine(string.Format("message: {0}", smsRequest.message));
            SmsResponse smsResponse;
            try
            {
                string auditMessage = auditM.ToString();
                GOC.Instance.Logger.LogMessage(new LogMessage(auditMessage, LogMessageType.Information, LoggingLevel.Maximum));
                context.LogSmsProcesorAction(this.SmsProcessorId, auditMessage, LogMessageType.Error.ToString());
                smsResponse = _smsSender.SendSms(smsRequest);
            }
            catch (Exception exFailed) //Failed to send the SMS Web Request to the provider.
            {
                int smsProviderCode = (int)_smsSender.SmsProvider;
                string errorMessage = null;
                context.LogFailedSmsSent(smsRequest.recipientNumber, smsRequest.message, smsProviderCode, exFailed, senderUser, out errorMessage);
                throw new Exception(errorMessage);
            }
            if (smsResponse != null)
            {
                StringBuilder responseM = new StringBuilder();
                responseM.AppendLine(string.Format("Sms Response to {0}:", smsRequest.recipientNumber));
                responseM.AppendLine(smsResponse.ToString());
                string responseMessage = responseM.ToString();
                GOC.Instance.Logger.LogMessage(new LogMessage(responseMessage, LogMessageType.Information, LoggingLevel.Maximum));
                context.LogSmsProcesorAction(this.SmsProcessorId, responseMessage, LogMessageType.Information.ToString());
            }
            SmsSentLog result = SpreadEntityContext.Create().LogSmsSent(
                smsRequest.recipientNumber,
                smsRequest.message,
                smsResponse.success,
                smsResponse.errorCode,
                smsResponse.error,
                smsResponse.messageId,
                smsRequest.message,
                (int)smsResponse.smsProvider,
                senderUser,
                true);
            if (smsResponse.success)
            {
                long smsCredits = context.DecrementSmsCreditFromOrganization(senderOrganization.OrganizationId).SmsCreditsBalance;
                string successMessage = string.Format("{0} '{1}' has sent an SMS. Credits remaining: {2}.", typeof(Organization).Name, senderOrganization.Name, smsCredits);
                GOC.Instance.Logger.LogMessage(new LogMessage(successMessage, LogMessageType.SuccessAudit, LoggingLevel.Normal));
                context.LogSmsProcesorAction(this.SmsProcessorId, successMessage, LogMessageType.SuccessAudit.ToString());
                context.DeleteSmsSentQueueItem(i.SmsSentQueueItemId, true);
            }
            else //Got a response from the provider, but sending the SMS failed.
            {
                StringBuilder errorMessageBuilder = new StringBuilder();
                if (!string.IsNullOrEmpty(smsResponse.error))
                {
                    errorMessageBuilder.AppendFormat("Error: {0}. ", smsResponse.error);
                }
                if (!string.IsNullOrEmpty(smsResponse.errorCode))
                {
                    errorMessageBuilder.AppendFormat("Code: {0}. ", smsResponse.errorCode);
                }
                if (!string.IsNullOrEmpty(smsResponse.messageId))
                {
                    errorMessageBuilder.AppendFormat("Message ID: {0}", smsResponse.messageId);
                }
                throw new Exception(errorMessageBuilder.ToString());
            }
            return result;
        }

        #endregion //Methods
    }
}