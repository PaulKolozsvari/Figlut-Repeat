namespace Figlut.Repeat.Processors.Sms
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

    public class SmsReceivedQueueProcessor : IntervalProcessor
    {
        #region Constructors

        public SmsReceivedQueueProcessor(
            Guid processorId,
            int executionInterval, 
            bool startImmediately,
            string organizationIdentifierIndicator,
            string subscriberNameIndicator,
            EmailSender emailSender) : 
            base(processorId, executionInterval, startImmediately, organizationIdentifierIndicator, subscriberNameIndicator, emailSender)
        {
        }

        #endregion //Constructors

        #region Methods

        protected override bool ProcessNextItemInQueue(RepeatEntityContext context, Guid processorId, string organizationIdentifierIndicator, string subscriberIdentifierIndicator)
        {
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            long itemsInQueue = context.GetAllSmsReceivedQueueItemCount();
            if (itemsInQueue < 1) //Continue processing items until the queue has been emptied i.e. there are no more items in the queue.
            {
                return false;
            }
            List<SmsReceivedQueueItem> items = context.GetTopSmsReceivedQueueItems(1);
            if (items.Count < 1)
            {
                throw new Exception(string.Format("{0} {1}s are supposed to exist, but none could be queried with TOP command.",
                    itemsInQueue,
                    typeof(SmsReceivedQueueItem).Name));
            }
            foreach (SmsReceivedQueueItem i in items)
            {
                context.LogProcesorAction(
                    processorId,
                    string.Format("Executing {0}. {1} items in queue. Processing SMS from '{2}': {3}",
                    DataShaper.ShapeCamelCaseString(typeof(SmsReceivedQueueProcessor).Name),
                    itemsInQueue,
                    i.CellPhoneNumber,
                    i.MessageContents),
                    LogMessageType.Information.ToString());
                Organization organization = null;
                Subscriber subscriber = null;
                SmsReceivedLog smsReceivedLog = context.ProcessSmsReceived(
                    i.CellPhoneNumber,
                    i.MessageId,
                    i.MessageContents,
                    i.DateReceivedOriginalFormat,
                    i.Campaign,
                    i.DataField,
                    i.Nonce,
                    i.NonceDateOriginalFormat,
                    i.Checksum,
                    i.SmsProviderCode,
                    organizationIdentifierIndicator,
                    subscriberIdentifierIndicator,
                    i.DateCreated,
                    i.SmsReceivedQueueItemId,
                    processorId,
                    out organization,
                    out subscriber);
                if (smsReceivedLog != null)
                {
                    string message = string.Format("Processed SMS from {0}: {1}", smsReceivedLog.CellPhoneNumber, smsReceivedLog.MessageContents);
                    GOC.Instance.Logger.LogMessage(new LogMessage(message, LogMessageType.SuccessAudit, LoggingLevel.Normal));
                    context.LogProcesorAction(processorId, message, LogMessageType.SuccessAudit.ToString());

                    SendEmailNotification(smsReceivedLog, organization, subscriber, context, processorId);
                }
            }
            return true;
        }

        private bool SendEmailNotification(
            SmsReceivedLog smsReceivedLog, 
            Organization organization, 
            Subscriber subscriber, 
            RepeatEntityContext context,
            Guid processorId)
        {
            string processorActionMessage = null;
            List<EmailNotificationRecipient> recipients = new List<EmailNotificationRecipient>();
            if ((organization != null) && (!string.IsNullOrEmpty(organization.EmailAddress)))
            {
                recipients.Add(new EmailNotificationRecipient() { DisplayName = organization.Name, EmailAddress = organization.EmailAddress });
                processorActionMessage = string.Format("Sent Email Notification for SMS received for {0} '{1}' ({2}), from '{3}'.",
                    typeof(Organization).Name,
                    organization.Name,
                    organization.EmailAddress,
                    smsReceivedLog.CellPhoneNumber);
            }
            else
            {
                processorActionMessage = string.Format("Sent Email Notification for SMS received from  '{0}'.",
                    smsReceivedLog.CellPhoneNumber);
            }
            string subject = string.Format("Figlut SMS Received Notification");
            StringBuilder body = new StringBuilder();
            body.AppendLine("Figlut SMS Received:");
            body.AppendLine();
            if (!string.IsNullOrEmpty(smsReceivedLog.CellPhoneNumber))
            {
                body.AppendLine(string.Format("{0}: {1}", EntityReader<SmsReceivedLog>.GetPropertyName(p => p.CellPhoneNumber, true), smsReceivedLog.CellPhoneNumber));
            }
            if (!string.IsNullOrEmpty(smsReceivedLog.MessageId))
            {
                body.AppendLine(string.Format("{0}: {1}", EntityReader<SmsReceivedLog>.GetPropertyName(p => p.MessageId, true), smsReceivedLog.MessageId));
            }
            if (!string.IsNullOrEmpty(smsReceivedLog.MessageContents))
            {
                body.AppendLine(string.Format("{0}: {1}", EntityReader<SmsReceivedLog>.GetPropertyName(p => p.MessageContents, true), smsReceivedLog.MessageContents));
            }
            if (smsReceivedLog.DateReceived.HasValue)
            {
                body.AppendLine(string.Format("{0}: {1}", EntityReader<SmsReceivedLog>.GetPropertyName(p => p.DateReceived, true), smsReceivedLog.DateReceived.Value));
            }
            body.AppendLine(string.Format("{0}: {1}", EntityReader<SmsReceivedLog>.GetPropertyName(p => p.DateCreated, true), smsReceivedLog.DateCreated));
            Nullable<Guid> organizationId = null;
            if (organization != null)
            {
                if (!string.IsNullOrEmpty(organization.Name))
                {
                    body.AppendLine(string.Format("{0} {1}: {2}", typeof(Organization).Name, EntityReader<Organization>.GetPropertyName(p => p.Name, true), organization.Name));
                }
                if (!string.IsNullOrEmpty(organization.EmailAddress))
                {
                    body.AppendLine(string.Format("{0} {1}: {2}", typeof(Organization).Name, EntityReader<Organization>.GetPropertyName(p => p.EmailAddress, true), organization.EmailAddress));
                }
                organizationId = organization.OrganizationId;
            }
            if (subscriber != null)
            {
                if (!string.IsNullOrEmpty(subscriber.Name))
                {
                    body.AppendLine(string.Format("{0} {1}: {2}", typeof(Subscriber).Name, EntityReader<Subscriber>.GetPropertyName(p => p.Name, true), subscriber.Name));
                }
                if (!string.IsNullOrEmpty(subscriber.CellPhoneNumber))
                {
                    body.AppendLine(string.Format("{0} {1}: {2}", typeof(Subscriber).Name, EntityReader<Subscriber>.GetPropertyName(p => p.CellPhoneNumber, true), subscriber.CellPhoneNumber));
                }
            }
            base._emailSender.SendEmail(EmailCategory.SmsReceived, subject, body.ToString(), null, false, recipients, null, organizationId);
            context.LogProcesorAction(processorId, processorActionMessage, LogMessageType.SuccessAudit.ToString());
            return true;
        }

        #endregion //Methods
    }
}