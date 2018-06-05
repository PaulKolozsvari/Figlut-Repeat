namespace Figlut.Spread.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class SpreadEntityContext : EntityContext
    {
        #region Sms Campaign

        public long GetAllSmsCampaignCount(Nullable<Guid> organizationId)
        {
            if (!organizationId.HasValue)
            {
                return 0;
            }
            return DB.GetTable<SmsCampaign>().LongCount(p => p.OrganizationId == organizationId.Value);
        }

        public SmsCampaign GetSmsCampaign(Guid smsCampaignId, bool throwExceptionOnNotFound)
        {
            SmsCampaign result = (from c in DB.GetTable<SmsCampaign>()
                                  where c.SmsCampaignId == smsCampaignId
                                  select c).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(SmsCampaign).Name,
                    EntityReader<SmsCampaign>.GetPropertyName(p => p.SmsCampaignId, false),
                    smsCampaignId.ToString()));
            }
            return result;
        }

        public SmsCampaignView GetSmsCampaignView(Guid smsCampaignId, bool throwExceptionOnNotFound)
        {
            SmsCampaignView result = (from c in DB.GetTable<SmsCampaign>()
                                      join o in DB.GetTable<Organization>() on c.OrganizationId equals o.OrganizationId into set
                                      from sub in set.DefaultIfEmpty()
                                      where c.SmsCampaignId == smsCampaignId
                                      orderby c.DateCreated descending
                                      select new SmsCampaignView()
                                      {
                                          SmsCampaignId = c.SmsCampaignId,
                                          Name = c.Name,
                                          MessageContents = c.MessageContents,
                                          OrganizationSelectedCode = c.OrganizationSelectedCode,
                                          OrganizationId = c.OrganizationId,
                                          DateCreated = c.DateCreated,
                                          SmsSentQueueItemCount = GetSmsSentQueueItemCountForSmsCampaign(c.SmsCampaignId),
                                          OrganizationName = sub.Name
                                      }).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(SmsCampaign).Name,
                    EntityReader<SmsCampaign>.GetPropertyName(p => p.SmsCampaignId, false),
                    smsCampaignId.ToString()));
            }
            return result;
        }

        public List<SmsCampaignView> GetSmsCampaignViewsByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<SmsCampaignView> result = null;
            if (organizationId.HasValue)
            {
                result = (from c in DB.GetTable<SmsCampaign>()
                          join o in DB.GetTable<Organization>() on c.OrganizationId equals o.OrganizationId into set
                          from sub in set.DefaultIfEmpty()
                          where (c.OrganizationId == organizationId.Value) &&
                          (c.Name.ToLower().Contains(searchFilterLower) ||
                          (c.OrganizationSelectedCode != null && c.OrganizationSelectedCode.ToLower().Contains(searchFilterLower)))
                          orderby c.DateCreated descending
                          select new SmsCampaignView()
                          {
                              SmsCampaignId = c.SmsCampaignId,
                              Name = c.Name,
                              MessageContents = c.MessageContents,
                              OrganizationSelectedCode = c.OrganizationSelectedCode,
                              OrganizationId = c.OrganizationId,
                              DateCreated = c.DateCreated,
                              SmsSentQueueItemCount = GetSmsSentQueueItemCountForSmsCampaign(c.SmsCampaignId),
                              OrganizationName = sub.Name
                          }).ToList();
            }
            else
            {
                result = (from c in DB.GetTable<SmsCampaign>()
                          join o in DB.GetTable<Organization>() on c.OrganizationId equals o.OrganizationId into set
                          from sub in set.DefaultIfEmpty()
                          where (c.Name.ToLower().Contains(searchFilterLower) ||
                          (c.OrganizationSelectedCode != null && c.OrganizationSelectedCode.ToLower().Contains(searchFilterLower)))
                          orderby c.DateCreated descending
                          select new SmsCampaignView()
                          {
                              SmsCampaignId = c.SmsCampaignId,
                              Name = c.Name,
                              MessageContents = c.MessageContents,
                              OrganizationSelectedCode = c.OrganizationSelectedCode,
                              OrganizationId = c.OrganizationId,
                              DateCreated = c.DateCreated,
                              SmsSentQueueItemCount = GetSmsSentQueueItemCountForSmsCampaign(c.SmsCampaignId),
                              OrganizationName = sub.Name
                          }).ToList();
            }
            return result;
        }

        public long GetSmsSentQueueItemCountForSmsCampaign(Guid smsCampaignId)
        {
            return (from s in DB.GetTable<SmsSentQueueItem>()
                    where s.SmsCampaignId == smsCampaignId
                    select s).LongCount();
        }

        public List<SmsCampaign> GetSmsCampaignsByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<SmsCampaign> result = null;
            if (organizationId.HasValue)
            {
                result = (from c in DB.GetTable<SmsCampaign>()
                          where (c.OrganizationId == organizationId.Value) &&
                          (c.Name.ToLower().Contains(searchFilterLower) ||
                          (c.OrganizationSelectedCode != null && c.OrganizationSelectedCode.ToLower().Contains(searchFilterLower)))
                          orderby c.DateCreated descending
                          select c).ToList();
            }
            else
            {
                result = (from c in DB.GetTable<SmsCampaign>()
                          where (c.Name.ToLower().Contains(searchFilterLower) ||
                          (c.OrganizationSelectedCode != null && c.OrganizationSelectedCode.ToLower().Contains(searchFilterLower)))
                          orderby c.DateCreated descending
                          select c).ToList();
            }
            return result;
        }

        public void DeleteSmsCampaignsByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<SmsCampaign> smsCampaigns = GetSmsCampaignsByFilter(searchFilter, organizationId);
                DB.GetTable<SmsCampaign>().DeleteAllOnSubmit(smsCampaigns);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        public long CreateSmsCampaign(
            Guid smsCampaignId,
            string name, 
            string messageContents, 
            string organizationSelectedCode, 
            Guid organizationId,
            DateTime dateCreated,
            string organizationSubscriptionsSearchText,
            int smsProviderCode,
            Guid senderUserId,
            out string errorMessage)
        {
            errorMessage = null;
            long enqueuedSmsCount = 0;
            using (TransactionScope t = new TransactionScope())
            {
                List<OrganizationSubscriptionView> subscriptions = GetOrganizationSubscriptionViewsByFilter(
                    organizationSubscriptionsSearchText,
                    organizationId,
                    true);
                if (subscriptions.Count <= 0)
                {
                    errorMessage = string.Format("Cannot create campaign with 0 selected subscriptions that are enabled.");
                    return 0;
                }
                SmsCampaign smsCampaign = new SmsCampaign()
                {
                    SmsCampaignId = smsCampaignId,
                    Name = name,
                    MessageContents = messageContents,
                    OrganizationSelectedCode = organizationSelectedCode,
                    OrganizationId = organizationId,
                    DateCreated = dateCreated
                };
                DB.GetTable<SmsCampaign>().InsertOnSubmit(smsCampaign);
                DB.SubmitChanges();
                foreach (OrganizationSubscriptionView v in subscriptions)
                {
                    SmsSentQueueItem smsSentQueueItem = EnqueueSmsSent(
                        v.SubscriberCellPhoneNumber,
                        messageContents,
                        null,
                        null,
                        null,
                        smsProviderCode,
                        senderUserId,
                        organizationId,
                        v.SubscriberId,
                        v.SubscriberName,
                        smsCampaign.Name,
                        smsCampaign.SmsCampaignId,
                        false,
                        false);
                    enqueuedSmsCount++;
                }
                DB.SubmitChanges();
                t.Complete();
            }
            return enqueuedSmsCount;
        }

        #endregion //Sms Campaign
    }
}
