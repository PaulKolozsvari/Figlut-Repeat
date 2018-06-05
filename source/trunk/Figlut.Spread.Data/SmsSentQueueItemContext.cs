namespace Figlut.Spread.Data
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Spread.ORM;
    using Figlut.Server.Toolkit.Data;
    using System.Transactions;

    #endregion //Using Directives

    public partial class SpreadEntityContext : EntityContext
    {
        #region Sms Sent Queue Item

        public SmsSentQueueItem GetSmsSentQueueItem(Guid smsSentQueueItemId, bool throwExceptionOnNotFound)
        {
            SmsSentQueueItem result = (from i in DB.GetTable<SmsSentQueueItem>()
                                       where i.SmsSentQueueItemId == smsSentQueueItemId
                                       select i).FirstOrDefault();
            if (throwExceptionOnNotFound && result == null)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(SmsSentQueueItem).Name,
                    EntityReader<SmsSentQueueItem>.GetPropertyName(p => p.SmsSentQueueItemId, false),
                    smsSentQueueItemId.ToString()));
            }
            return result;
        }

        public long GetAllSmsSentQueueItemCount(bool excludeFailedSmsSent)
        {
            if (excludeFailedSmsSent)
            {
                return DB.GetTable<SmsSentQueueItem>().Where(p => !p.FailedToSend).LongCount();
            }
            return DB.GetTable<SmsSentQueueItem>().LongCount();
        }

        public List<SmsSentQueueItem> GetSmsSentQueueItemsByFilter(string searchFilter, Nullable<Guid> smsCampaignId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower().Trim();
            List<SmsSentQueueItem> result = null;
            if (smsCampaignId.HasValue)
            {
                result = (from s in DB.GetTable<SmsSentQueueItem>()
                          where s.SmsCampaignId == smsCampaignId.Value &&
                          (s.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          s.Campaign.ToLower().Contains(searchFilterLower) ||
                          s.MessageContents.ToLower().Contains(searchFilterLower))
                          select s).ToList();
            }
            else
            {
                result = (from s in DB.GetTable<SmsSentQueueItem>()
                          where (s.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          s.Campaign.ToLower().Contains(searchFilterLower) ||
                          s.MessageContents.ToLower().Contains(searchFilterLower))
                          select s).ToList();
            }
            return result;
        }

        public void DeleteSmsSentQueueItemsByFilter(string searchFilter, Nullable<Guid> smsCampaignId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<SmsSentQueueItem> smsSentQueueItems = GetSmsSentQueueItemsByFilter(searchFilter, smsCampaignId);
                DB.GetTable<SmsSentQueueItem>().DeleteAllOnSubmit(smsSentQueueItems);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        /// <summary>
        /// Deletes and Sms sent from the queue.
        /// </summary>
        public void DeleteSmsSentQueueItem(Guid smsSentQueueItemId, bool submitChanges, bool startTransaction)
        {
            if (startTransaction)
            {
                using (TransactionScope t = new TransactionScope())
                {
                    SmsSentQueueItem i = GetSmsSentQueueItem(smsSentQueueItemId, true);
                    DB.GetTable<SmsSentQueueItem>().DeleteOnSubmit(i);
                    if (submitChanges)
                    {
                        DB.SubmitChanges();
                    }
                    t.Complete();
                }
            }
            else
            {
                SmsSentQueueItem i = GetSmsSentQueueItem(smsSentQueueItemId, true);
                DB.GetTable<SmsSentQueueItem>().DeleteOnSubmit(i);
                if (submitChanges)
                {
                    DB.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Gets items off the Sms sent queue.
        /// </summary>
        public List<SmsSentQueueItem> GetTopSmsSentQueueItems(int numberOfItems)
        {
            List<SmsSentQueueItem> result = null;
            using (TransactionScope t = new TransactionScope())
            {
                result = (from i in DB.GetTable<SmsSentQueueItem>()
                          where !i.FailedToSend
                          orderby i.DateCreated ascending
                          select i).Take(numberOfItems).ToList();
                t.Complete();
            }
            return result;
        }

        public void FlagSmsSentQueueItemAsFailedToSend(
            Guid smsSentQueueItemId,
            string failedToSendErrorMessage,
            bool throwExceptionOnNotFound,
            bool submitChanges,
            bool startTransaction)
        {
            if (startTransaction)
            {
                using (TransactionScope t = new TransactionScope())
                {
                    SmsSentQueueItem i = GetSmsSentQueueItem(smsSentQueueItemId, throwExceptionOnNotFound);
                    if (i != null)
                    {
                        i.FailedToSend = true;
                        i.FailedToSendErrorMessage = failedToSendErrorMessage;
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
                SmsSentQueueItem i = GetSmsSentQueueItem(smsSentQueueItemId, throwExceptionOnNotFound);
                if (i != null)
                {
                    i.FailedToSend = true;
                    i.FailedToSendErrorMessage = failedToSendErrorMessage;
                }
                if (submitChanges)
                {
                    DB.SubmitChanges();
                }
            }
        }

        public SmsSentQueueItem EnqueueSmsSent(
            string subscriberCellPhoneNumber,
            string messageContents,
            string tableReference,
            Nullable<Guid> recordReference,
            string tag,
            int smsProviderCode,
            Guid senderUserId,
            Guid organizationId,
            Nullable<Guid> subscriberId,
            string subscriberName,
            string campaign,
            Nullable<Guid> smsCampaignId,
            bool startTransaction,
            bool submitChanges)
        {
            SmsSentQueueItem result = new SmsSentQueueItem()
            {
                SmsSentQueueItemId = Guid.NewGuid(),
                CellPhoneNumber = subscriberCellPhoneNumber,
                MessageContents = messageContents,
                TableReference = tableReference,
                RecordReference = recordReference,
                Tag = tag,
                SmsProviderCode = smsProviderCode,
                SenderUserId = senderUserId,
                OrganizationId = organizationId,
                SubscriberId = subscriberId,
                SubscriberName = subscriberName,
                Campaign = campaign,
                SmsCampaignId = smsCampaignId,
                DateCreated = DateTime.Now
            };
            if (startTransaction)
            {
                using (TransactionScope t = new TransactionScope())
                {
                    DB.GetTable<SmsSentQueueItem>().InsertOnSubmit(result);
                    if (submitChanges)
                    {
                        DB.SubmitChanges();
                    }
                    t.Complete();
                }
            }
            else
            {
                DB.GetTable<SmsSentQueueItem>().InsertOnSubmit(result);
                if (submitChanges)
                {
                    DB.SubmitChanges();
                }
            }
            return result;
        }

        #endregion //Sms Sent Queue Item
    }
}