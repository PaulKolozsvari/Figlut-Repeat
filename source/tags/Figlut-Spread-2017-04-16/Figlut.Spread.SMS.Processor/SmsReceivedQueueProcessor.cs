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

    public class SmsReceivedQueueProcessor : SmsQueueProcessor
    {
        #region Constructors

        public SmsReceivedQueueProcessor(
            Guid smsProcessorId,
            int executionInterval, 
            bool startImmediately,
            string organizationIdentifierIndicator,
            string subscriberNameIndicator) : 
            base(smsProcessorId, executionInterval, startImmediately, organizationIdentifierIndicator, subscriberNameIndicator)
        {
        }

        #endregion //Constructors

        #region Methods

        protected override bool ProcessNextItemInQueue(SpreadEntityContext context, Guid smsProcessorId, string organizationIdentifierIndicator, string subscriberIdentifierIndicator)
        {
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
                context.LogSmsProcesorAction(
                    smsProcessorId,
                    string.Format("Executing {0}. {1} items in queue. Processing SMS from '{2}': {3}",
                    DataShaper.ShapeCamelCaseString(typeof(SmsReceivedQueueProcessor).Name),
                    itemsInQueue,
                    i.CellPhoneNumber,
                    i.MessageContents),
                    LogMessageType.Information.ToString());
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
                    i.SmsReceivedQueueItemId);
                if (smsReceivedLog != null)
                {
                    string message = string.Format("Processed SMS from {0}: {1}", smsReceivedLog.CellPhoneNumber, smsReceivedLog.MessageContents);
                    context.LogSmsProcesorAction(smsProcessorId, message, LogMessageType.SuccessAudit.ToString());
                    GOC.Instance.Logger.LogMessage(new LogMessage(message, LogMessageType.SuccessAudit, LoggingLevel.Normal));
                }
            }
            return true;
        }

        #endregion //Methods
    }
}