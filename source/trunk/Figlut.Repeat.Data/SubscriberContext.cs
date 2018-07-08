namespace Figlut.Repeat.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Repeat.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class RepeatEntityContext : EntityContext
    {
        #region Subscriber

        public Subscriber GetSubscriber(Guid subscriberId, bool throwExceptionOnNotFound)
        {
            Subscriber result = (from s in DB.GetTable<Subscriber>()
                                 where s.SubscriberId == subscriberId
                                 select s).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Subscriber).Name,
                    EntityReader<Subscriber>.GetPropertyName(p => p.SubscriberId, false),
                    subscriberId.ToString()));
            }
            return result;
        }

        public Subscriber GetSubscriberByCellPhoneNumber(string cellPhoneNumber, bool throwExceptionOnNotFound)
        {
            string cellPhoneNumberLower = cellPhoneNumber.Trim().ToLower();
            List<Subscriber> q = (from s in DB.GetTable<Subscriber>()
                                  where s.CellPhoneNumber.ToLower() == cellPhoneNumberLower
                                  select s).ToList();
            Subscriber result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Subscriber).Name,
                    EntityReader<Subscriber>.GetPropertyName(p => p.CellPhoneNumber, false),
                    cellPhoneNumberLower));
            }
            return result;
        }

        public bool SubscriberExistsbyCellPhoneNumber(string cellPhoneNumber)
        {
            Subscriber subscriber = GetSubscriberByCellPhoneNumber(cellPhoneNumber, false);
            return subscriber != null;
        }

        public long GetAllSubscriberCount()
        {
            return DB.GetTable<Subscriber>().LongCount();
        }

        public List<Subscriber> GetSubscribersByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<Subscriber> result = null;
            if (organizationId.HasValue)
            {
                result = (from o in DB.GetTable<Organization>()
                          join subscription in DB.GetTable<Subscription>() on o.OrganizationId equals subscription.OrganizationId
                          join subscriber in DB.GetTable<Subscriber>() on subscription.SubscriberId equals subscriber.SubscriberId
                          where (subscription.OrganizationId == organizationId.Value) &&
                          (subscriber.CellPhoneNumber.ToLower().Contains(searchFilterLower))
                          orderby subscriber.DateCreated descending
                          select subscriber).ToList();
            }
            else
            {
                result = (from subscriber in DB.GetTable<Subscriber>()
                          where subscriber.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          subscriber.Name.ToLower().Contains(searchFilterLower) ||
                          subscriber.Enabled.ToString().ToLower().Contains(searchFilterLower)
                          orderby subscriber.DateCreated descending
                          select subscriber).ToList();
            }
            return result;
        }

        public void DeleteSubscribersByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<Subscriber> subscribers = GetSubscribersByFilter(searchFilter, organizationId);
                DB.GetTable<Subscriber>().DeleteAllOnSubmit(subscribers);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Subscriber
    }
}