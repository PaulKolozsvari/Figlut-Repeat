namespace Figlut.Repeat.Processors.Schedule
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.Email;
    using Figlut.Repeat.SMS;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Views;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Server.Toolkit.Utilities;

    #endregion //Using Directives

    public class ScheduleProcessor : FiglutProcessor
    {
        public ScheduleProcessor(
            SmsSender smsSender,
            int maxSmsSendMessageLength,
            string smsSendMessageSuffix,
            int organizationIdentifierMaxLength,
            Guid processorId, 
            int executionInterval, 
            bool startImmediately, 
            string organizationIdentifierIndicator, 
            string subscriberNameIndicator, 
            EmailSender emailSender) : 
            base(processorId, executionInterval, startImmediately, organizationIdentifierIndicator, subscriberNameIndicator, emailSender)
        {
            if (smsSender == null)
            {
                throw new NullReferenceException(string.Format("{0} may not be null when constructing a {1}.",
                    EntityReader<ScheduleProcessor>.GetPropertyName(p => p.SmsSender, false),
                    typeof(ScheduleProcessor).Name));
            }
            _smsSender = smsSender;
            _maxSmsSendMessageLength = maxSmsSendMessageLength;
            _smsSendMessageSuffix = smsSendMessageSuffix;
            _organizationIdentifierMaxLength = organizationIdentifierMaxLength;
        }

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
                        EntityReader<ScheduleProcessor>.GetPropertyName(p => p.SmsSender, false),
                        typeof(ScheduleProcessor).Name));
                }
                _smsSender = value;
            }
        }

        #endregion //Properties

        #region Methods

        protected override bool ProcessNextItemInQueue(
            RepeatEntityContext context, 
            Guid processorId, 
            string organizationIdentifierIndicator, 
            string subscriberIdentifierIndicator)
        {
            DateTime currentDateTime = DateTime.Now;
            long entriesInQueue = context.GetPastScheduleEntriesSmsNotificationNotSentCount(currentDateTime, true);
            if (entriesInQueue < 1) //Continue processing entries until the queue has been emptied i.e. there are no more entries in the queue.
            {
                return false;
            }
            List<ScheduleEntry> entries = context.GetTopPastScheduleEntriesSmsNotificationNotSent(currentDateTime, 1, true);
            if (entries.Count < 1)
            {
                throw new Exception(string.Format("{0} {1}s are supposed to exist, but none could be queried with TOP command.",
                    entriesInQueue,
                    typeof(ScheduleEntry).Name));
            }
            foreach (ScheduleEntry e in entries)
            {
                ScheduleView scheduleView = context.GetScheduleView(e.ScheduleId, true);
                Organization organization = context.GetOrganization(scheduleView.OrganizationId, true);
                context.LogProcesorAction(
                    processorId,
                    string.Format("Executing {0}. {1} items in queue. Processing Schedule Entry to '{2}': {3}",
                    DataShaper.ShapeCamelCaseString(typeof(ScheduleProcessor).Name),
                    entriesInQueue,
                    scheduleView.CellPhoneNumber,
                    e.NotificationMessage),
                    LogMessageType.Information.ToString());
                if (organization.SmsCreditsBalance < 1 && !organization.AllowSmsCreditsDebt)
                {
                    throw new Exception(string.Format("{0} '{1}' has insufficient SMS credits to send an SMS.", typeof(Organization).Name, organization.Name));
                }
                SmsSentLog smsSentLog = ProcessScheduleEntryQueueItem(scheduleView, e, context);
                if (smsSentLog != null)
                {
                    string message = string.Format("Processed Schedule Entry SMS to {0}: {1}", smsSentLog.CellPhoneNumber, smsSentLog.MessageContents);
                    context.LogProcesorAction(processorId, message, LogMessageType.SuccessAudit.ToString());
                    GOC.Instance.Logger.LogMessage(new LogMessage(message, LogMessageType.SuccessAudit, LoggingLevel.Normal));
                }
            }
            return true;
        }

        protected SmsSentLog ProcessScheduleEntryQueueItem(ScheduleView scheduleView, ScheduleEntry scheduleEntry, RepeatEntityContext context)
        {
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            Organization senderOrganization = context.GetOrganization(scheduleView.OrganizationId, true);

            SmsRequest smsRequest = new SmsRequest(scheduleView.CellPhoneNumber, scheduleEntry.NotificationMessage, _maxSmsSendMessageLength, _smsSendMessageSuffix, senderOrganization.Identifier, base._organizationIdentifierIndicator);
            StringBuilder auditM = new StringBuilder();
            auditM.AppendLine("Sending SMS: ");
            auditM.AppendLine(string.Format("recipientNumber: {0}", smsRequest.recipientNumber));
            auditM.AppendLine(string.Format("message: {0}", smsRequest.message));
            SmsResponse smsResponse;
            try
            {
                string auditMessage = auditM.ToString();
                GOC.Instance.Logger.LogMessage(new LogMessage(auditMessage, LogMessageType.Information, LoggingLevel.Maximum));
                context.LogProcesorAction(this.ProcessorId, auditMessage, LogMessageType.Information.ToString());
                smsResponse = _smsSender.SendSms(smsRequest);
            }
            catch (Exception exFailed) //Failed to send the SMS Web Request to the provider.
            {
                int smsProviderCode = (int)_smsSender.SmsProvider;
                string errorMessage = null;
                context.LogFailedSmsSent(smsRequest.recipientNumber, smsRequest.message, smsProviderCode, exFailed, null, senderOrganization, out errorMessage);
                context.FlagScheduleEntryAsFailedToSend(scheduleEntry.ScheduleEntryId, errorMessage, true, true, true);
                throw new Exception(errorMessage);
            }
            if (smsResponse != null)
            {
                StringBuilder responseM = new StringBuilder();
                responseM.AppendLine(string.Format("Sms Response to {0}:", smsRequest.recipientNumber));
                responseM.AppendLine(smsResponse.ToString());
                string responseMessage = responseM.ToString();
                GOC.Instance.Logger.LogMessage(new LogMessage(responseMessage, LogMessageType.Information, LoggingLevel.Maximum));
                context.LogProcesorAction(this.ProcessorId, responseMessage, LogMessageType.Information.ToString());
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
                null,
                senderOrganization,
                true,
                scheduleView.SubscriberId,
                scheduleView.SubscriberName,
                null,
                null);
            if (smsResponse.success)
            {
                long smsCredits = context.DecrementSmsCreditFromOrganization(senderOrganization.OrganizationId).SmsCreditsBalance; //Sms credits are substracted when the SMS' are enqueued.
                string successMessage = string.Format("{0} '{1}' has sent an SMS. Credits remaining: {2}.", typeof(Organization).Name, senderOrganization.Name, smsCredits);
                GOC.Instance.Logger.LogMessage(new LogMessage(successMessage, LogMessageType.SuccessAudit, LoggingLevel.Normal));
                context.LogProcesorAction(this.ProcessorId, successMessage, LogMessageType.SuccessAudit.ToString());

                scheduleEntry.SMSNotificationSent = true;
                scheduleEntry.SMSMessageId = smsResponse.messageId;
                scheduleEntry.SMSDateSent = DateTime.Now;
                if (result != null)
                {
                    scheduleEntry.SmsSentLogId = result.SmsSentLogId;
                }
                context.Save<ScheduleEntry>(scheduleEntry, false);
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
                throw new Exception(errorMessage);
            }
            return result;
        }

        #endregion //Methods
    }
}
