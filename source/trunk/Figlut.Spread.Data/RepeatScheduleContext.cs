namespace Figlut.Spread.Data
{
    using Figlut.Server.Toolkit.Data;
    #region Using Directives

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
        #region Repeat Schedule

        public RepeatSchedule GetRepeatSchedule(Guid repeatScheduleId, bool throwExceptionOnNotFound)
        {
            RepeatSchedule result = (from r in DB.GetTable<RepeatSchedule>()
                                     where r.RepeatScheduleId == repeatScheduleId
                                     select r).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(RepeatSchedule).Name,
                    EntityReader<RepeatSchedule>.GetPropertyName(p => p.RepeatScheduleId, false),
                    repeatScheduleId.ToString()));
            }
            return result;
        }

        public long GetAllRepeatScheduleCount()
        {
            return DB.GetTable<RepeatSchedule>().LongCount();
        }

        public RepeatScheduleView GetRepeatScheduleView(Guid repeatScheduleId, bool throwExceptionOnNotFound)
        {
            RepeatScheduleView result = (from repeatSchedule in DB.GetTable<RepeatSchedule>()
                                         join subscription in DB.GetTable<Subscription>() on repeatSchedule.SubscriptionId equals subscription.SubscriptionId into setSubscription
                                         from subscriptionView in setSubscription.DefaultIfEmpty()
                                         join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                                         from subscriberView in setSubscriber
                                         where repeatSchedule.RepeatScheduleId == repeatScheduleId
                                         select new RepeatScheduleView()
                                         {
                                             RepeatScheduleId = repeatSchedule.RepeatScheduleId,
                                             SubscriptionId = repeatSchedule.SubscriptionId,
                                             NotificationMessage = repeatSchedule.NotificationMessage,
                                             ScheduleName = repeatSchedule.ScheduleName,
                                             Quantity = repeatSchedule.Quantity,
                                             UnitOfMeasure = repeatSchedule.UnitOfMeasure,
                                             DaysRepeatInterval = repeatSchedule.DaysRepeatInterval,
                                             Notes = repeatSchedule.Notes,
                                             DateCreated = repeatSchedule.DateCreated,

                                             //Subscription
                                             OrganizationId = subscriptionView.OrganizationId,
                                             SubscriberId = subscriptionView.SubscriberId,
                                             SubscriptionEnabled = subscriptionView.Enabled,
                                             CustomerFullName = subscriptionView.CustomerFullName,
                                             CustomerIdentifier = subscriptionView.CustomerIdentifier,
                                             CustomerPhysicalAddress = subscriptionView.CustomerPhysicalAddress,
                                             CustomerNotes = subscriptionView.CustomerNotes,
                                             SubscriptionDateCreated = subscriptionView.DateCreated,

                                             //Subscriber
                                             CellPhoneNumber = subscriberView.CellPhoneNumber,
                                             SubscriberName = subscriberView.Name,
                                             SubscriberEnabled = subscriberView.Enabled
                                         }).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(RepeatScheduleView).Name,
                    EntityReader<RepeatScheduleView>.GetPropertyName(p => p.RepeatScheduleId, false),
                    repeatScheduleId.ToString()));
            }
            return result;
        }

        public List<RepeatScheduleView> GetRepeatScheduleViewsByFilter(string searchFilter, Nullable<Guid> subscriptionId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<RepeatScheduleView> result = null;
            if (subscriptionId.HasValue)
            {
                result = (from repeatSchedule in DB.GetTable<RepeatSchedule>()
                          join subscription in DB.GetTable<Subscription>() on repeatSchedule.SubscriptionId equals subscription.SubscriptionId into setSubscription
                          from subscriptionView in setSubscription.DefaultIfEmpty()
                          join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                          from subscriberView in setSubscriber
                          where repeatSchedule.SubscriptionId == subscriptionId.Value &&
                          (repeatSchedule.ScheduleName.ToLower().Contains(searchFilterLower) ||
                          repeatSchedule.Notes.ToLower().Contains(searchFilterLower))
                          select new RepeatScheduleView()
                          {
                              RepeatScheduleId = repeatSchedule.RepeatScheduleId,
                              SubscriptionId = repeatSchedule.SubscriptionId,
                              NotificationMessage = repeatSchedule.NotificationMessage,
                              ScheduleName = repeatSchedule.ScheduleName,
                              Quantity = repeatSchedule.Quantity,
                              UnitOfMeasure = repeatSchedule.UnitOfMeasure,
                              DaysRepeatInterval = repeatSchedule.DaysRepeatInterval,
                              Notes = repeatSchedule.Notes,
                              DateCreated = repeatSchedule.DateCreated,

                              //Subscription
                              OrganizationId = subscriptionView.OrganizationId,
                              SubscriberId = subscriptionView.SubscriberId,
                              SubscriptionEnabled = subscriptionView.Enabled,
                              CustomerFullName = subscriptionView.CustomerFullName,
                              CustomerIdentifier = subscriptionView.CustomerIdentifier,
                              CustomerPhysicalAddress = subscriptionView.CustomerPhysicalAddress,
                              CustomerNotes = subscriptionView.CustomerNotes,
                              SubscriptionDateCreated = subscriptionView.DateCreated,

                              //Subscriber
                              CellPhoneNumber = subscriberView.CellPhoneNumber,
                              SubscriberName = subscriberView.Name,
                              SubscriberEnabled = subscriberView.Enabled
                          }).ToList();
            }
            else
            {
                result = (from repeatSchedule in DB.GetTable<RepeatSchedule>()
                          join subscription in DB.GetTable<Subscription>() on repeatSchedule.SubscriptionId equals subscription.SubscriptionId into setSubscription
                          from subscriptionView in setSubscription.DefaultIfEmpty()
                          join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                          from subscriberView in setSubscriber
                          where (repeatSchedule.ScheduleName.ToLower().Contains(searchFilterLower) ||
                          repeatSchedule.Notes.ToLower().Contains(searchFilterLower))
                          select new RepeatScheduleView()
                          {
                              RepeatScheduleId = repeatSchedule.RepeatScheduleId,
                              SubscriptionId = repeatSchedule.SubscriptionId,
                              NotificationMessage = repeatSchedule.NotificationMessage,
                              ScheduleName = repeatSchedule.ScheduleName,
                              Quantity = repeatSchedule.Quantity,
                              UnitOfMeasure = repeatSchedule.UnitOfMeasure,
                              DaysRepeatInterval = repeatSchedule.DaysRepeatInterval,
                              Notes = repeatSchedule.Notes,
                              DateCreated = repeatSchedule.DateCreated,

                              //Subscription
                              OrganizationId = subscriptionView.OrganizationId,
                              SubscriberId = subscriptionView.SubscriberId,
                              SubscriptionEnabled = subscriptionView.Enabled,
                              CustomerFullName = subscriptionView.CustomerFullName,
                              CustomerIdentifier = subscriptionView.CustomerIdentifier,
                              CustomerPhysicalAddress = subscriptionView.CustomerPhysicalAddress,
                              CustomerNotes = subscriptionView.CustomerNotes,
                              SubscriptionDateCreated = subscriptionView.DateCreated,

                              //Subscriber
                              CellPhoneNumber = subscriberView.CellPhoneNumber,
                              SubscriberName = subscriberView.Name,
                              SubscriberEnabled = subscriberView.Enabled
                          }).ToList();
            }
            return result;
        }

        public List<RepeatSchedule> GetRepeatScheduleByFilter(string searchFilter, Nullable<Guid> subscriptionId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<RepeatSchedule> result = null;
            if (subscriptionId.HasValue)
            {
                result = (from r in DB.GetTable<RepeatSchedule>()
                          where r.SubscriptionId == subscriptionId.Value &&
                          (r.ScheduleName.ToLower().Contains(searchFilterLower) ||
                          r.Notes.ToLower().Contains(searchFilterLower))
                          select r).ToList();
            }
            else
            {
                result = (from r in DB.GetTable<RepeatSchedule>()
                          where (r.ScheduleName.ToLower().Contains(searchFilterLower) ||
                          r.Notes.ToLower().Contains(searchFilterLower))
                          select r).ToList();
            }
            return result;
        }

        public void DeleteRepeatSchedulesByFilter(string searchFilter, Nullable<Guid> subscriptionId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<RepeatSchedule> repeatSchedules = GetRepeatScheduleByFilter(searchFilter, subscriptionId);
                DB.GetTable<RepeatSchedule>().DeleteAllOnSubmit(repeatSchedules);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Repeat Schedule
    }
}