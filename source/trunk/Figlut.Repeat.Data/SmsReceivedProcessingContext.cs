namespace Figlut.Repeat.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.SMS;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class RepeatEntityContext : EntityContext
    {
        #region Sms Received Processing

        /// <summary>
        /// Adds the SMS received values into a Queue (SmsReceivedQueueItem database table).
        /// </summary>
        public SmsReceivedQueueItem EnqueueSmsReceived(
            string cellPhoneNumber,
            string messageId,
            string messageContents,
            string dateReceivedOriginalFormat,
            string campaign,
            string dataField,
            string nonce,
            string nonceDateOriginalFormat,
            string checksum,
            int smsProviderCode)
        {
            if (string.IsNullOrEmpty(cellPhoneNumber) ||
                string.IsNullOrEmpty(messageId) ||
                string.IsNullOrEmpty(messageContents))
            {
                return null;
            }
            SmsReceivedQueueItem result = null;
            using (TransactionScope t = new TransactionScope())
            {
                result = new SmsReceivedQueueItem()
                {
                    SmsReceivedQueueItemId = Guid.NewGuid(),
                    CellPhoneNumber = cellPhoneNumber,
                    MessageId = messageId,
                    MessageContents = messageContents,
                    DateReceivedOriginalFormat = dateReceivedOriginalFormat,
                    Campaign = campaign,
                    DataField = dataField,
                    Nonce = nonce,
                    NonceDateOriginalFormat = nonceDateOriginalFormat,
                    Checksum = checksum,
                    SmsProviderCode = smsProviderCode,
                    DateCreated = DateTime.Now
                };
                DB.GetTable<SmsReceivedQueueItem>().InsertOnSubmit(result);
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        /// <summary>
        /// Gets items off the Sms received queue.
        /// </summary>
        public List<SmsReceivedQueueItem> GetTopSmsReceivedQueueItems(int numberOfItems)
        {
            List<SmsReceivedQueueItem> result = null;
            using (TransactionScope t = new TransactionScope())
            {
                result = (from i in DB.GetTable<SmsReceivedQueueItem>()
                          orderby i.DateCreated ascending
                          select i).Take(numberOfItems).ToList();
                t.Complete();
            }
            return result;
        }

        public SmsReceivedQueueItem GetSmsReceivedQueueItem(Guid smsReceivedQueueItemId, bool throwExceptionOnNotFound)
        {
            SmsReceivedQueueItem result = (from i in DB.GetTable<SmsReceivedQueueItem>()
                                           where i.SmsReceivedQueueItemId == smsReceivedQueueItemId
                                           select i).FirstOrDefault();
            if (throwExceptionOnNotFound && result == null)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(SmsReceivedQueueItem).Name,
                    EntityReader<SmsReceivedQueueItem>.GetPropertyName(p => p.SmsReceivedQueueItemId, false),
                    smsReceivedQueueItemId.ToString()));
            }
            return result;
        }

        /// <summary>
        /// Deletes an Sms received from the queue.
        /// </summary>
        public void DeleteSmsReceivedQueueItem(Guid smsReceivedQueueItemId, bool submitChanges, bool startTransaction)
        {
            if (startTransaction)
            {
                SmsReceivedQueueItem i = GetSmsReceivedQueueItem(smsReceivedQueueItemId, true);
                DB.GetTable<SmsReceivedQueueItem>().DeleteOnSubmit(i);
                if (submitChanges)
                {
                    DB.SubmitChanges();
                }
            }
            else
            {
                SmsReceivedQueueItem i = GetSmsReceivedQueueItem(smsReceivedQueueItemId, true);
                DB.GetTable<SmsReceivedQueueItem>().DeleteOnSubmit(i);
                if (submitChanges)
                {
                    DB.SubmitChanges();
                }
            }
        }

        public long GetAllSmsReceivedQueueItemCount()
        {
            return DB.GetTable<SmsReceivedQueueItem>().LongCount();
        }

        /// <summary>
        /// * Checks if there's a subscriber for the given Cell Phone Number, if there isn't it will create one. If there is it will update the suscriber's Name with the name that is supplied.
        /// * Checks if if an organization identifier was specified: if it was, it looks for the organization.
        ///     * If there is an organization: 
        ///         * It will create an OrganizationMessageReceived record linked to the organization. 
        ///         * It will also link the subscriber to the organization by creating a subscription if it doesn't already exist.
        /// </summary>
        public SmsReceivedLog ProcessSmsReceived(
            string cellPhoneNumber,
            string messageId,
            string messageContents,
            string dateReceivedOriginalFormat,
            string campaign,
            string dataField,
            string nonce,
            string nonceDateOriginalFormat,
            string checksum,
            int smsProviderCode,
            string organizationIdentifierIndicator,
            string subscriberNameIndicator,
            DateTime dateReceived,
            Nullable<Guid> smsReceivedQueueItemIdToDelete,
            Guid smsProcessorId,
            out Organization organization,
            out Subscriber subscriber)
        {
            organization = null;
            subscriber = null;
            SmsReceivedLog smsReceivedLog = null;
            using (TransactionScope t = new TransactionScope())
            {
                smsReceivedLog = LogSmsReceived(cellPhoneNumber, messageId, messageContents, dateReceivedOriginalFormat, campaign, dataField, nonce, nonceDateOriginalFormat, checksum, smsProviderCode, dateReceived, true);
                if (smsReceivedLog == null)
                {
                    t.Complete();
                    return null;
                }
                SmsReceivedParsed smsReceived = new SmsReceivedParsed(cellPhoneNumber, messageId, messageContents, dateReceivedOriginalFormat, campaign, dataField, nonce, nonceDateOriginalFormat, checksum, organizationIdentifierIndicator, subscriberNameIndicator);
                if (!string.IsNullOrEmpty(smsReceived.WarningMessage))
                {
                    this.LogSmsProcesorAction(smsProcessorId, smsReceived.WarningMessage, LogMessageType.Warning.ToString());
                    GOC.Instance.Logger.LogMessage(new LogMessage(smsReceived.WarningMessage, LogMessageType.Warning, LoggingLevel.Normal));
                }
                if (!smsReceived.ParsedSuccessfully)
                {
                    throw new Exception(string.Format("Could not parse SMS. Error: {0}", smsReceived.ErrorMessage));
                }
                subscriber = SaveSubscriber(smsReceived, true, false);
                organization = null;
                if (!string.IsNullOrEmpty(smsReceived.OrganizationIdentifier))
                {
                    organization = GetOrganizationByIdentifier(smsReceived.OrganizationIdentifier, false);
                    if (organization != null)
                    {
                        Subscription subscription = SaveSubscription(organization, subscriber, false);
                        UpdateSmsReceivedLogWithOrganizationAndSubscriberDetails(smsReceived, smsReceivedLog, organization, subscriber, true);
                    }
                    else
                    {
                        string organizationMissingWarningMessage = string.Format("Could not find {0} with {1} of '{2}'.",
                            typeof(Organization).Name,
                            EntityReader<Organization>.GetPropertyName(p => p.Identifier, false),
                            smsReceived.OrganizationIdentifier);
                        GOC.Instance.Logger.LogMessage(new LogMessage(organizationMissingWarningMessage, LogMessageType.Warning, LoggingLevel.Normal));
                        this.LogSmsProcesorAction(smsProcessorId, organizationMissingWarningMessage, LogMessageType.Warning.ToString());
                    }
                }
                if (smsReceivedQueueItemIdToDelete.HasValue)
                {
                    DeleteSmsReceivedQueueItem(smsReceivedQueueItemIdToDelete.Value, true, false);
                }
                DB.SubmitChanges();
                t.Complete();
            }
            return smsReceivedLog;
        }

        /// <summary>
        /// Saves the SMS Recieved to the database.
        /// </summary>
        private SmsReceivedLog LogSmsReceived(
            string cellPhoneNumber,
            string messageId,
            string messageContents,
            string dateReceivedOriginalFormat,
            string campaign,
            string dataField,
            string nonce,
            string nonceDateOriginalFormat,
            string checksum,
            int smsProviderCode,
            DateTime dateReceived,
            bool submitChanges)
        {
            if (string.IsNullOrEmpty(cellPhoneNumber) ||
                string.IsNullOrEmpty(messageId) ||
                string.IsNullOrEmpty(messageContents))
            {
                return null;
            }
            SmsReceivedLog result = new SmsReceivedLog()
            {
                SmsReceivedLogId = Guid.NewGuid(),
                CellPhoneNumber = cellPhoneNumber,
                MessageId = messageId,
                MessageContents = messageContents,
                DateReceivedOriginalFormat = dateReceivedOriginalFormat,
                Campaign = campaign,
                DataField = dataField,
                Nonce = nonce,
                NonceDateOriginalFormat = nonceDateOriginalFormat,
                Checksum = checksum,
                SmsProviderCode = smsProviderCode,
                DateReceived = dateReceived,
                DateCreated = DateTime.Now
            };
            DB.GetTable<SmsReceivedLog>().InsertOnSubmit(result);
            if (submitChanges)
            {
                DB.SubmitChanges();
            }
            return result;
        }


        public Subscriber SaveSubscriber(SmsReceivedParsed smsReceived, bool submitChanges, bool startTransaction)
        {
            return SaveSubscriber(smsReceived.From, smsReceived.SubscriberName, submitChanges, startTransaction);
        }

        public Subscriber SaveSubscriber(string cellPhoneNumber, string subscriberName, bool submitChanges, bool startTransaction)
        {
            if (string.IsNullOrEmpty(cellPhoneNumber))
            {
                throw new NullReferenceException(string.Format("Cannot save {0} with {1} that is null or empty.",
                    typeof(Subscriber).Name,
                    EntityReader<Subscriber>.GetPropertyName(p => p.CellPhoneNumber, true)));
            }
            string cellPhoneNumberTrimmed = cellPhoneNumber.Trim();
            string subscriberNameTrimmed = string.IsNullOrEmpty(subscriberName) ? null : subscriberName.Trim();
            Subscriber subscriber = null;
            if (startTransaction)
            {
                using (TransactionScope t = new TransactionScope())
                {
                    subscriber = GetSubscriberByCellPhoneNumber(cellPhoneNumberTrimmed, false);
                    if (subscriber == null) //A new subscriber needs to be created.
                    {
                        subscriber = new Subscriber()
                        {
                            SubscriberId = Guid.NewGuid(),
                            CellPhoneNumber = cellPhoneNumberTrimmed,
                            Enabled = true,
                            DateCreated = DateTime.Now
                        };
                        DB.GetTable<Subscriber>().InsertOnSubmit(subscriber);
                    }
                    if (!string.IsNullOrEmpty(subscriberNameTrimmed))
                    {
                        subscriber.Name = subscriberNameTrimmed;
                    }
                    if (submitChanges)
                    {
                        DB.SubmitChanges();
                    }
                    t.Complete();
                }
            }
            else
            {
                subscriber = GetSubscriberByCellPhoneNumber(cellPhoneNumberTrimmed, false);
                if (subscriber == null) //A new subscriber needs to be created.
                {
                    subscriber = new Subscriber()
                    {
                        SubscriberId = Guid.NewGuid(),
                        CellPhoneNumber = cellPhoneNumberTrimmed,
                        Enabled = true,
                        DateCreated = DateTime.Now
                    };
                    DB.GetTable<Subscriber>().InsertOnSubmit(subscriber);
                }
                if (!string.IsNullOrEmpty(subscriberNameTrimmed))
                {
                    subscriber.Name = subscriberNameTrimmed;
                }
                if (submitChanges)
                {
                    DB.SubmitChanges();
                }
            }
            return subscriber;
        }

        private Subscription SaveSubscription(Organization organization, Subscriber subscriber, bool submitChanges)
        {
            Subscription subscription = GetSubscription(subscriber.SubscriberId, organization.OrganizationId, false);
            if (subscription == null)
            {
                subscription = new Subscription()
                {
                    SubscriptionId = Guid.NewGuid(),
                    OrganizationId = organization.OrganizationId,
                    SubscriberId = subscriber.SubscriberId,
                    Enabled = true,
                    DateCreated = DateTime.Now
                };
                DB.GetTable<Subscription>().InsertOnSubmit(subscription);
            }
            if (submitChanges)
            {
                DB.SubmitChanges();
            }
            return subscription;
        }

        private void UpdateSmsReceivedLogWithOrganizationAndSubscriberDetails(
            SmsReceivedParsed smsReceived,
            SmsReceivedLog smsReceivedLog,
            Organization organization,
            Subscriber subscriber,
            bool submitChanges)
        {
            if ((smsReceived == null) && (smsReceivedLog == null))
            {
                return;
            }
            if (organization != null)
            {
                smsReceivedLog.OrganizationId = organization.OrganizationId;
            }
            if (subscriber != null)
            {
                smsReceivedLog.SubscriberId = subscriber.SubscriberId;
                if (!string.IsNullOrEmpty(subscriber.Name))
                {
                    smsReceivedLog.SubscriberName = subscriber.Name;
                }
            }
            if (smsReceived != null && !string.IsNullOrEmpty(smsReceived.MessageToOrganization))
            {
                smsReceivedLog.MessageToOrganization = smsReceived.MessageToOrganization;
            }
            if (submitChanges)
            {
                DB.SubmitChanges();
            }
            return;
        }

        #endregion //Sms Received Processing
    }
}