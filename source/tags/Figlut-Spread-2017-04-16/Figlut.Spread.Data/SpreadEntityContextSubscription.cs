﻿namespace Figlut.Spread.Data
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
        #region Subscription

        public Subscription GetSubscription(Guid subscriptionId, bool throwExceptionOnNotFound)
        {
            List<Subscription> q = (from s in DB.GetTable<Subscription>()
                                    where s.SubscriptionId == subscriptionId
                                    select s).ToList();
            Subscription result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Subscription).Name,
                    EntityReader<Subscription>.GetPropertyName(p => p.SubscriptionId, false),
                    subscriptionId.ToString()));
            }
            return result;
        }

        public Subscription GetSubscription(Guid subscriberId, Guid organizationId, bool throwExceptionOnNotFound)
        {
            List<Subscription> q = (from s in DB.GetTable<Subscription>()
                                    where s.SubscriberId == subscriberId && s.OrganizationId == organizationId
                                    select s).ToList();
            Subscription result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}' and {3} of '{4}'.",
                    typeof(Subscription).Name,
                    EntityReader<Subscription>.GetPropertyName(p => p.SubscriberId, false),
                    subscriberId.ToString(),
                    EntityReader<Subscription>.GetPropertyName(p => p.OrganizationId, false),
                    organizationId.ToString()));
            }
            return result;
        }

        public OrganizationSubscriptionView GetOrganizationSubscriptionView(Guid subscriptionId, bool throwExceptionOnNotFound)
        {
            OrganizationSubscriptionView result = (from subscription in DB.GetTable<Subscription>()
                                                   join subscriber in DB.GetTable<Subscriber>() on subscription.SubscriberId equals subscriber.SubscriberId into set
                                                   from sub in set.DefaultIfEmpty()
                                                   where subscription.SubscriptionId == subscriptionId
                                                   select new OrganizationSubscriptionView()
                                                   {
                                                       SubscriptionId = subscription.SubscriptionId,
                                                       OrganizationId = subscription.OrganizationId,
                                                       SubscriberId = sub.SubscriberId,
                                                       Enabled = subscription.Enabled,
                                                       DateCreated = subscription.DateCreated,
                                                       SubscriberCellPhoneNumber = sub.CellPhoneNumber,
                                                       SubscriberName = sub.Name,
                                                       SubscriberIsEnabled = sub.IsEnabled,
                                                       SubscriberDateCreated = sub.DateCreated
                                                   }).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Subscription).Name,
                    EntityReader<Subscription>.GetPropertyName(p => p.SubscriptionId, false),
                    subscriptionId.ToString()));
            }
            return result;
        }

        public SubscriberSubscriptionView GetSubscriberSubscriptionView(Guid subscriptionId, bool throwExceptionOnNotFound)
        {
            SubscriberSubscriptionView result = (from subscription in DB.GetTable<Subscription>()
                                                 join organization in DB.GetTable<Organization>() on subscription.OrganizationId equals organization.OrganizationId into set
                                                 from sub in set.DefaultIfEmpty()
                                                 where subscription.SubscriptionId == subscriptionId
                                                 select new SubscriberSubscriptionView()
                                                 {
                                                     SubscriptionId = subscription.SubscriptionId,
                                                     OrganizationId = sub.OrganizationId,
                                                     SubscriberId = subscription.SubscriberId,
                                                     Enabled = subscription.Enabled,
                                                     DateCreated = subscription.DateCreated,
                                                     OrganizationName = sub.Name,
                                                     OrganizationIdentifier = sub.Identifier,
                                                     OrganizationEmailAddress = sub.EmailAddress,
                                                     OrganizationAddress = sub.Address,
                                                     OrganizationDateCreated = sub.DateCreated
                                                 }).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Subscription).Name,
                    EntityReader<Subscription>.GetPropertyName(p => p.SubscriptionId, false),
                    subscriptionId.ToString()));
            }
            return result;
        }

        public long GetAllSubscriptionsCount()
        {
            return DB.GetTable<Subscription>().LongCount();
        }

        public long GetAllOrganizationSubscriptionsCount(Nullable<Guid> organizationId)
        {
            if (!organizationId.HasValue)
            {
                return 0;
            }
            return DB.GetTable<Subscription>().LongCount(p => p.OrganizationId == organizationId.Value);
        }

        public long GetAllSubscriberSubscriptionsCount(Nullable<Guid> subscriberId)
        {
            if (!subscriberId.HasValue)
            {
                return 0;
            }
            return DB.GetTable<Subscription>().LongCount(p => p.SubscriberId == subscriberId.Value);
        }

        public List<Subscription> GetOrganizationSubscriptionsByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<Subscription> result = null;
            if (organizationId.HasValue)
            {
                result = (from subscription in DB.GetTable<Subscription>()
                          join subscriber in DB.GetTable<Subscriber>() on subscription.SubscriberId equals subscriber.SubscriberId into set
                          from sub in set.DefaultIfEmpty()
                          where subscription.OrganizationId == organizationId.Value &&
                          (sub.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          sub.Name.ToLower().Contains(searchFilterLower))
                          orderby sub.DateCreated descending
                          select subscription).ToList();
            }
            else
            {
                result = (from subscription in DB.GetTable<Subscription>()
                          join subscriber in DB.GetTable<Subscriber>() on subscription.SubscriberId equals subscriber.SubscriberId into set
                          from sub in set.DefaultIfEmpty()
                          where (sub.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          sub.Name.ToLower().Contains(searchFilterLower))
                          orderby sub.DateCreated descending
                          select subscription).ToList();
            }
            return result;
        }

        public List<Subscription> GetSubscriberSubscriptionsByFilter(string searchFilter, Nullable<Guid> subscriberId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<Subscription> result = null;
            if (subscriberId.HasValue)
            {
                result = (from subscription in DB.GetTable<Subscription>()
                          join organization in DB.GetTable<Organization>() on subscription.OrganizationId equals organization.OrganizationId into set
                          from sub in set.DefaultIfEmpty()
                          where subscription.SubscriberId == subscriberId.Value &&
                          (sub.Name.ToLower().Contains(searchFilterLower) ||
                          sub.Identifier.ToLower().Contains(searchFilterLower) ||
                          sub.EmailAddress.ToLower().Contains(searchFilterLower))
                          orderby sub.DateCreated descending, sub.Name, sub.Identifier, sub.EmailAddress
                          select subscription).ToList();
            }
            else
            {
                result = (from subscription in DB.GetTable<Subscription>()
                          join organization in DB.GetTable<Organization>() on subscription.OrganizationId equals organization.OrganizationId into set
                          from sub in set.DefaultIfEmpty()
                          where (sub.Name.ToLower().Contains(searchFilterLower) ||
                          sub.Identifier.ToLower().Contains(searchFilterLower) ||
                          sub.EmailAddress.ToLower().Contains(searchFilterLower))
                          orderby sub.DateCreated descending, sub.Name, sub.Identifier, sub.EmailAddress
                          select subscription).ToList();
            }
            return result;
        }

        public long GetOrganizationSubscriptionViewsByFilterCount(string searchFilter, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            long result = 0;
            if (organizationId.HasValue)
            {
                result = (from subscription in DB.GetTable<Subscription>()
                          join subscriber in DB.GetTable<Subscriber>() on subscription.SubscriberId equals subscriber.SubscriberId into set
                          from sub in set.DefaultIfEmpty()
                          where subscription.OrganizationId == organizationId.Value &&
                          (sub.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          sub.Name.ToLower().Contains(searchFilterLower))
                          orderby sub.DateCreated descending, sub.CellPhoneNumber, sub.Name
                          select new OrganizationSubscriptionView()
                          {
                              SubscriptionId = subscription.SubscriptionId,
                              OrganizationId = subscription.OrganizationId,
                              SubscriberId = sub.SubscriberId,
                              Enabled = subscription.Enabled,
                              DateCreated = subscription.DateCreated,
                              SubscriberCellPhoneNumber = sub.CellPhoneNumber,
                              SubscriberName = sub.Name,
                              SubscriberIsEnabled = sub.IsEnabled,
                              SubscriberDateCreated = sub.DateCreated
                          }).LongCount();
            }
            else
            {
                result = (from subscription in DB.GetTable<Subscription>()
                          join subscriber in DB.GetTable<Subscriber>() on subscription.SubscriberId equals subscriber.SubscriberId into set
                          from sub in set.DefaultIfEmpty()
                          where (sub.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          sub.Name.ToLower().Contains(searchFilterLower))
                          orderby sub.DateCreated descending, sub.CellPhoneNumber, sub.Name
                          select new OrganizationSubscriptionView()
                          {
                              SubscriptionId = subscription.SubscriptionId,
                              OrganizationId = subscription.OrganizationId,
                              SubscriberId = sub.SubscriberId,
                              Enabled = subscription.Enabled,
                              DateCreated = subscription.DateCreated,
                              SubscriberCellPhoneNumber = sub.CellPhoneNumber,
                              SubscriberName = sub.Name,
                              SubscriberIsEnabled = sub.IsEnabled,
                              SubscriberDateCreated = sub.DateCreated
                          }).LongCount();
            }
            return result;
        }

        public List<OrganizationSubscriptionView> GetOrganizationSubscriptionViewsByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<OrganizationSubscriptionView> result = null;
            if (organizationId.HasValue)
            {
                result = (from subscription in DB.GetTable<Subscription>()
                          join subscriber in DB.GetTable<Subscriber>() on subscription.SubscriberId equals subscriber.SubscriberId into set
                          from sub in set.DefaultIfEmpty()
                          where subscription.OrganizationId == organizationId.Value &&
                          (sub.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          sub.Name.ToLower().Contains(searchFilterLower))
                          orderby sub.DateCreated descending, sub.CellPhoneNumber, sub.Name
                          select new OrganizationSubscriptionView()
                          {
                              SubscriptionId = subscription.SubscriptionId,
                              OrganizationId = subscription.OrganizationId,
                              SubscriberId = sub.SubscriberId,
                              Enabled = subscription.Enabled,
                              DateCreated = subscription.DateCreated,
                              SubscriberCellPhoneNumber = sub.CellPhoneNumber,
                              SubscriberName = sub.Name,
                              SubscriberIsEnabled = sub.IsEnabled,
                              SubscriberDateCreated = sub.DateCreated
                          }).ToList();
            }
            else
            {
                result = (from subscription in DB.GetTable<Subscription>()
                          join subscriber in DB.GetTable<Subscriber>() on subscription.SubscriberId equals subscriber.SubscriberId into set
                          from sub in set.DefaultIfEmpty()
                          where (sub.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          sub.Name.ToLower().Contains(searchFilterLower))
                          orderby sub.DateCreated descending, sub.CellPhoneNumber, sub.Name
                          select new OrganizationSubscriptionView()
                          {
                              SubscriptionId = subscription.SubscriptionId,
                              OrganizationId = subscription.OrganizationId,
                              SubscriberId = sub.SubscriberId,
                              Enabled = subscription.Enabled,
                              DateCreated = subscription.DateCreated,
                              SubscriberCellPhoneNumber = sub.CellPhoneNumber,
                              SubscriberName = sub.Name,
                              SubscriberIsEnabled = sub.IsEnabled,
                              SubscriberDateCreated = sub.DateCreated
                          }).ToList();
            }
            return result;
        }

        public List<SubscriberSubscriptionView> GetSubscriberSubscriptionViewsByFilter(string searchFilter, Nullable<Guid> subscriberId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<SubscriberSubscriptionView> result = null;
            if (subscriberId.HasValue)
            {
                result = (from subscription in DB.GetTable<Subscription>()
                          join organization in DB.GetTable<Organization>() on subscription.OrganizationId equals organization.OrganizationId into set
                          from sub in set.DefaultIfEmpty()
                          where subscription.SubscriberId == subscriberId.Value &&
                          (sub.Name.ToLower().Contains(searchFilterLower) ||
                          sub.Identifier.ToLower().Contains(searchFilterLower) ||
                          sub.EmailAddress.ToLower().Contains(searchFilterLower))
                          orderby sub.DateCreated descending, sub.Name, sub.Identifier, sub.EmailAddress
                          select new SubscriberSubscriptionView()
                          {
                              SubscriptionId = subscription.SubscriptionId,
                              OrganizationId = sub.OrganizationId,
                              SubscriberId = subscription.SubscriberId,
                              Enabled = subscription.Enabled,
                              DateCreated = subscription.DateCreated,
                              OrganizationName = sub.Name,
                              OrganizationIdentifier = sub.Identifier,
                              OrganizationEmailAddress = sub.EmailAddress,
                              OrganizationAddress = sub.Address,
                              OrganizationDateCreated = sub.DateCreated
                          }).ToList();
            }
            else
            {
                result = (from subscription in DB.GetTable<Subscription>()
                          join organization in DB.GetTable<Organization>() on subscription.OrganizationId equals organization.OrganizationId into set
                          from sub in set.DefaultIfEmpty()
                          where (sub.Name.ToLower().Contains(searchFilterLower) ||
                          sub.Identifier.ToLower().Contains(searchFilterLower) ||
                          sub.EmailAddress.ToLower().Contains(searchFilterLower))
                          orderby sub.DateCreated descending, sub.Name, sub.Identifier, sub.EmailAddress
                          select new SubscriberSubscriptionView()
                          {
                              SubscriptionId = subscription.SubscriptionId,
                              OrganizationId = sub.OrganizationId,
                              SubscriberId = subscription.SubscriberId,
                              Enabled = subscription.Enabled,
                              DateCreated = subscription.DateCreated,
                              OrganizationName = sub.Name,
                              OrganizationIdentifier = sub.Identifier,
                              OrganizationEmailAddress = sub.EmailAddress,
                              OrganizationAddress = sub.Address,
                              OrganizationDateCreated = sub.DateCreated
                          }).ToList();
            }
            return result;
        }

        public void DeleteOrganizationSubscriptionsByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<Subscription> subscriptions = GetOrganizationSubscriptionsByFilter(searchFilter, organizationId);
                DB.GetTable<Subscription>().DeleteAllOnSubmit(subscriptions);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        public void DeleteSubscriberSubscriptionsByFilter(string searchFilter, Nullable<Guid> subscriberId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<Subscription> subscriptions = GetSubscriberSubscriptionsByFilter(searchFilter, subscriberId);
                DB.GetTable<Subscription>().DeleteAllOnSubmit(subscriptions);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Subscription
    }
}