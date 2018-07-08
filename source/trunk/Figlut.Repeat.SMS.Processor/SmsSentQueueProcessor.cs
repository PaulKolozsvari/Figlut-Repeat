namespace Figlut.Repeat.SMS.Processor
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
using Figlut.Server.Toolkit.Utilities;
using Figlut.Server.Toolkit.Utilities.Logging;
using Figlut.Repeat.Data;
using Figlut.Repeat.Email;
using Figlut.Repeat.ORM;
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
            int maxSmsSendMessageLength,
            string smsSendMessageSuffix,
            int organizationIdentifierMaxLength,
            Guid smsProcessorId,
            int executionInterval,
            bool startImmediately,
            string organizationIdentifierIndicator,
            string subscriberNameIndicator,
            EmailSender emailSender) : 
            base(smsProcessorId, executionInterval, startImmediately, organizationIdentifierIndicator, subscriberNameIndicator, emailSender)
        {
            if (smsSender == null)
            {
                throw new NullReferenceException(string.Format("{0} may not be null when constructing a {1}.",
                    EntityReader<SmsSentQueueProcessor>.GetPropertyName(p => p.SmsSender, false),
                    typeof(SmsSentQueueProcessor).Name));
            }
            _smsSender = smsSender;
            _maxSmsSendMessageLength = maxSmsSendMessageLength;
            _smsSendMessageSuffix = smsSendMessageSuffix;
            _organizationIdentifierMaxLength = organizationIdentifierMaxLength;
        }

        #endregion //Constructors

        #region Fields

        protected SmsSender _smsSender;
        protected int _maxSmsSendMessageLength;
        protected string _smsSendMessageSuffix;
        protected int _organizationIdentifierMaxLength;

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

        protected override bool ProcessNextItemInQueue(RepeatEntityContext context, Guid smsProcessorId, string organizationIdentifierIndicator, string subscriberIdentifierIndicator)
        {
            long itemsInQueue = context.GetAllSmsSentQueueItemCount(true);
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

        protected SmsSentLog ProcessSmsSentQueueItem(SmsSentQueueItem i, RepeatEntityContext context)
        {
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            User senderUser = null;
            Organization senderOrganization = null;
            ValidateSmsSending(i, context, out senderUser, out senderOrganization);

            SmsRequest smsRequest = new SmsRequest(i.CellPhoneNumber, i.MessageContents, _maxSmsSendMessageLength, _smsSendMessageSuffix, senderOrganization.Identifier, base._organizationIdentifierIndicator);
            StringBuilder auditM = new StringBuilder();
            auditM.AppendLine("Sending SMS: ");
            auditM.AppendLine(string.Format("recipientNumber: {0}", smsRequest.recipientNumber));
            auditM.AppendLine(string.Format("message: {0}", smsRequest.message));
            SmsResponse smsResponse;
            try
            {
                string auditMessage = auditM.ToString();
                GOC.Instance.Logger.LogMessage(new LogMessage(auditMessage, LogMessageType.Information, LoggingLevel.Maximum));
                context.LogSmsProcesorAction(this.SmsProcessorId, auditMessage, LogMessageType.Information.ToString());
                smsResponse = _smsSender.SendSms(smsRequest);
            }
            catch (Exception exFailed) //Failed to send the SMS Web Request to the provider.
            {
                int smsProviderCode = (int)_smsSender.SmsProvider;
                string errorMessage = null;
                context.LogFailedSmsSent(smsRequest.recipientNumber, smsRequest.message, smsProviderCode, exFailed, senderUser, out errorMessage);
                context.FlagSmsSentQueueItemAsFailedToSend(i.SmsSentQueueItemId, errorMessage, true, true, true);
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
            SmsSentLog result = RepeatEntityContext.Create().LogSmsSent(
                smsRequest.recipientNumber,
                smsRequest.message,
                smsResponse.success,
                smsResponse.errorCode,
                smsResponse.error,
                smsResponse.messageId,
                smsRequest.message,
                (int)smsResponse.smsProvider,
                senderUser,
                true,
                i.SubscriberId,
                i.SubscriberName,
                i.Campaign,
                i.SmsCampaignId);
            if (smsResponse.success)
            {
                //long smsCredits = context.DecrementSmsCreditFromOrganization(senderOrganization.OrganizationId).SmsCreditsBalance; //Sms credits are substracted when the SMS' are enqueued.
                string successMessage = string.Format("{0} '{1}' has sent an SMS. Credits remaining: {2}.", typeof(Organization).Name, senderOrganization.Name, senderOrganization.SmsCreditsBalance);
                GOC.Instance.Logger.LogMessage(new LogMessage(successMessage, LogMessageType.SuccessAudit, LoggingLevel.Normal));
                context.LogSmsProcesorAction(this.SmsProcessorId, successMessage, LogMessageType.SuccessAudit.ToString());
                context.DeleteSmsSentQueueItem(i.SmsSentQueueItemId, true, true);
                context.SaveSubscriber(i.CellPhoneNumber, i.SubscriberName, true, true); //Creates  subscriber for the given cell phone to which this SMS has been sent if the subscriber with the given cell phone number does not already exist.
            }
            else //Got a response from the provider, but sending the SMS failed.
            {
                StringBuilder errorM = new StringBuilder();
                if (!string.IsNullOrEmpty(smsResponse.error))
                {
                    errorM.AppendFormat("Error: {0}. ", smsResponse.error);
                }
                if (!string.IsNullOrEmpty(smsResponse.errorCode))
                {
                    errorM.AppendFormat("Code: {0}. ", smsResponse.errorCode);
                }
                if (!string.IsNullOrEmpty(smsResponse.messageId))
                {
                    errorM.AppendFormat("Message ID: {0}", smsResponse.messageId);
                }
                string errorMessage = errorM.ToString();
                context.FlagSmsSentQueueItemAsFailedToSend(i.SmsSentQueueItemId, errorMessage, true, true, true);
                throw new Exception(errorMessage);
            }
            return result;
        }

        private void ValidateSmsSending(SmsSentQueueItem i, RepeatEntityContext context, out User senderUser, out Organization senderOrganization)
        {
            senderUser = i.SenderUserId.HasValue ? context.GetUser(i.SenderUserId.Value, true) : null;
            string failedToSendErrorMessage = null;
            if (senderUser == null)
            {
                failedToSendErrorMessage = string.Format("{0} with {1} of '{2}' created at '{3}' does not have a {4} value. Cannot link to a {5}.",
                    typeof(SmsSentQueueItem).Name, //0
                    EntityReader<SmsSentQueueItem>.GetPropertyName(p => p.SmsSentQueueItemId, false), //1
                    i.SmsSentQueueItemId.ToString(), //2
                    i.DateCreated.ToString(), //3
                    EntityReader<SmsSentQueueItem>.GetPropertyName(p => p.SenderUserId, false), //4
                    typeof(User).Name);
                context.FlagSmsSentQueueItemAsFailedToSend(i.SmsSentQueueItemId, failedToSendErrorMessage, true, true, true);
                throw new NullReferenceException(failedToSendErrorMessage); //5
            }
            senderOrganization = i.OrganizationId.HasValue ? context.GetOrganization(i.OrganizationId.Value, true) : null;
            if (senderOrganization == null)
            {
                failedToSendErrorMessage = string.Format("{0} with {1} of '{2}' created at '{3}' does not have a {4} value. Cannot link to a {5}.",
                    typeof(SmsSentQueueItem).Name, //0
                    EntityReader<SmsSentQueueItem>.GetPropertyName(p => p.SmsSentQueueItemId, false), //1
                    i.SmsSentQueueItemId.ToString(), //2
                    i.DateCreated.ToString(), //3
                    EntityReader<SmsSentQueueItem>.GetPropertyName(p => p.OrganizationId, false), //4
                    typeof(Organization).Name);
                context.FlagSmsSentQueueItemAsFailedToSend(i.SmsSentQueueItemId, failedToSendErrorMessage, true, true, true);
                throw new NullReferenceException(failedToSendErrorMessage); //5
            }
            //Validation and subtraction of credits was done when the SMS was enqueued, so no need to validate credits here.
            //if (senderOrganization.SmsCreditsBalance < 1 && !senderOrganization.AllowSmsCreditsDebt)
            //{
            //    failedToSendErrorMessage = string.Format("{0} '{1}' has insufficient SMS credits to send an SMS.", typeof(Organization).Name, senderOrganization.Name);
            //    context.FlagSmsSentQueueItemAsFailedToSend(i.SmsSentQueueItemId, failedToSendErrorMessage, true, true, true);
            //    throw new NullReferenceException(failedToSendErrorMessage);
            //}
        }

        #endregion //Methods
    }
}