namespace Figlut.Repeat.Data
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Repeat.ORM;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Repeat.ORM.Views;
    using System.Transactions;

    #endregion //Using Directives

    public partial class RepeatEntityContext : EntityContext
    {
        #region Schedule Entry

        public ScheduleEntry GetScheduleEntry(Guid scheduleEntryId, bool throwExceptionOnNotFound)
        {
            ScheduleEntry result = (from r in DB.GetTable<ScheduleEntry>()
                                    where r.ScheduleEntryId == scheduleEntryId
                                    select r).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(ScheduleEntry).Name,
                    EntityReader<ScheduleEntry>.GetPropertyName(p => p.ScheduleId, false),
                    scheduleEntryId.ToString()));
            }
            return result;
        }

        public long GetAllScheduleEntryCount()
        {
            return DB.GetTable<ScheduleEntry>().LongCount();
        }

        public ScheduleEntryView GetScheduleEntryView(Guid scheduleEntryId, bool throwExceptionOnNotFound)
        {
            ScheduleEntryView result = (from scheduleEntry in DB.GetTable<ScheduleEntry>()
                                              join schedule in DB.GetTable<Schedule>() on scheduleEntry.ScheduleId equals schedule.ScheduleId into setSchedule
                                              from scheduleView in setSchedule.DefaultIfEmpty()
                                              join subscription in DB.GetTable<Subscription>() on scheduleView.SubscriptionId equals subscription.SubscriptionId into setSubscription
                                              from subscriptionView in setSubscription.DefaultIfEmpty()
                                              join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                                              from subscriberView in setSubscriber.DefaultIfEmpty()
                                              where scheduleEntry.ScheduleEntryId == scheduleEntryId
                                              select new ScheduleEntryView()
                                              {
                                                  //Schedule Entry
                                                  ScheduleEntryId = scheduleEntry.ScheduleEntryId,
                                                  ScheduleId = scheduleEntry.ScheduleId,
                                                  EntryTime = scheduleEntry.EntryTime,
                                                  NotificationMessage = scheduleEntry.NotificationMessage,
                                                  EntryDate = scheduleEntry.EntryDate,
                                                  EntryDateFormatted = scheduleEntry.EntryDateFormatted,
                                                  EntryDateDayOfWeek = scheduleEntry.EntryDateDayOfWeek,
                                                  NotificationDate = scheduleEntry.NotificationDate,
                                                  NotificationDateFormatted = scheduleEntry.NotificationDateFormatted,
                                                  NotificationDateDayOfWeek = scheduleEntry.NotificationDateDayOfWeek,
                                                  SMSNotificationSent = scheduleEntry.SMSNotificationSent,
                                                  SMSMessageId = scheduleEntry.SMSMessageId,
                                                  SMSDateSent = scheduleEntry.SMSDateSent,
                                                  SmsSentLogId = scheduleEntry.SmsSentLogId,
                                                  DateCreated = scheduleEntry.DateCreated,

                                                  //Schedule
                                                  SubscriptionId = scheduleView.SubscriptionId,
                                                  ScheduleNotificationMessage = scheduleView.NotificationMessage,
                                                  ScheduleName = scheduleView.ScheduleName,
                                                  Quantity = scheduleView.Quantity,
                                                  UnitOfMeasure = scheduleView.UnitOfMeasure,
                                                  DaysRepeatInterval = scheduleView.DaysRepeatInterval,
                                                  ScheduleNotes = scheduleView.Notes,
                                                  ScheduleDateCreated = scheduleView.DateCreated,

                                                  //Subscriber
                                                  CellPhoneNumber = subscriberView.CellPhoneNumber,

                                                  //Subscription View
                                                  CustomerFullName = subscriptionView.CustomerFullName,
                                                  CustomerEmailAddress = subscriptionView.CustomerEmailAddress,
                                                  CustomerIdentifier = subscriptionView.CustomerIdentifier,
                                                  CustomerPhysicalAddress = subscriptionView.CustomerPhysicalAddress,
                                                  CustomerNotes = subscriptionView.CustomerNotes
                                              }).FirstOrDefault();

            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(ScheduleEntryView).Name,
                    EntityReader<ScheduleEntryView>.GetPropertyName(p => p.ScheduleEntryId, false),
                    scheduleEntryId.ToString()));
            }
            return result;
        }

        #region Queries filtering on Schedule

        public List<ScheduleEntryView> GetScheduleEntryViewsForScheduleByFilter(string searchFilter, Nullable<Guid> scheduleId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<ScheduleEntryView> result = null;
            if (scheduleId.HasValue)
            {
                result = (from scheduleEntry in DB.GetTable<ScheduleEntry>()
                          join schedule in DB.GetTable<Schedule>() on scheduleEntry.ScheduleId equals schedule.ScheduleId into setSchedule
                          from scheduleView in setSchedule.DefaultIfEmpty()
                          join subscription in DB.GetTable<Subscription>() on scheduleView.SubscriptionId equals subscription.SubscriptionId into setSubscription
                          from subscriptionView in setSubscription.DefaultIfEmpty()
                          join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                          from subscriberView in setSubscriber.DefaultIfEmpty()
                          where scheduleEntry.ScheduleId == scheduleId.Value &&
                          (scheduleEntry.EntryDateFormatted.Contains(searchFilterLower) ||
                          scheduleEntry.NotificationDateFormatted.Contains(searchFilterLower) ||
                          scheduleEntry.EntryDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          scheduleEntry.NotificationDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          scheduleEntry.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (scheduleEntry.SMSMessageId != null && scheduleEntry.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          orderby scheduleEntry.EntryDate
                          select new ScheduleEntryView()
                          {
                              //Schedule Entry
                              ScheduleEntryId = scheduleEntry.ScheduleEntryId,
                              ScheduleId = scheduleEntry.ScheduleId,
                              NotificationMessage = scheduleEntry.NotificationMessage,
                              EntryTime = scheduleEntry.EntryTime,
                              EntryDate = scheduleEntry.EntryDate,
                              EntryDateFormatted = scheduleEntry.EntryDateFormatted,
                              EntryDateDayOfWeek = scheduleEntry.EntryDateDayOfWeek,
                              NotificationDate = scheduleEntry.NotificationDate,
                              NotificationDateFormatted = scheduleEntry.NotificationDateFormatted,
                              NotificationDateDayOfWeek = scheduleEntry.NotificationDateDayOfWeek,
                              SMSNotificationSent = scheduleEntry.SMSNotificationSent,
                              SMSMessageId = scheduleEntry.SMSMessageId,
                              SMSDateSent = scheduleEntry.SMSDateSent,
                              SmsSentLogId = scheduleEntry.SmsSentLogId,
                              DateCreated = scheduleEntry.DateCreated,

                              //Schedule
                              SubscriptionId = scheduleView.SubscriptionId,
                              ScheduleNotificationMessage = scheduleView.NotificationMessage,
                              ScheduleName = scheduleView.ScheduleName,
                              Quantity = scheduleView.Quantity,
                              UnitOfMeasure = scheduleView.UnitOfMeasure,
                              DaysRepeatInterval = scheduleView.DaysRepeatInterval,
                              ScheduleNotes = scheduleView.Notes,
                              ScheduleDateCreated = scheduleView.DateCreated,

                              //Subscriber
                              CellPhoneNumber = subscriberView.CellPhoneNumber,

                              //Subscription View
                              CustomerFullName = subscriptionView.CustomerFullName,
                              CustomerEmailAddress = subscriptionView.CustomerEmailAddress,
                              CustomerIdentifier = subscriptionView.CustomerIdentifier,
                              CustomerPhysicalAddress = subscriptionView.CustomerPhysicalAddress,
                              CustomerNotes = subscriptionView.CustomerNotes
                          }).ToList();
            }
            else
            {
                result = (from scheduleEntry in DB.GetTable<ScheduleEntry>()
                          join schedule in DB.GetTable<Schedule>() on scheduleEntry.ScheduleId equals schedule.ScheduleId into setSchedule
                          from scheduleView in setSchedule.DefaultIfEmpty()
                          join subscription in DB.GetTable<Subscription>() on scheduleView.SubscriptionId equals subscription.SubscriptionId into setSubscription
                          from subscriptionView in setSubscription.DefaultIfEmpty()
                          join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                          from subscriberView in setSubscriber.DefaultIfEmpty()
                          where (scheduleEntry.EntryDateFormatted.Contains(searchFilterLower) ||
                          scheduleEntry.NotificationDateFormatted.Contains(searchFilterLower) ||
                          scheduleEntry.EntryDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          scheduleEntry.NotificationDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          scheduleEntry.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (scheduleEntry.SMSMessageId != null && scheduleEntry.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          orderby scheduleEntry.EntryDate
                          select new ScheduleEntryView()
                          {
                              //Schedule Entry
                              ScheduleEntryId = scheduleEntry.ScheduleEntryId,
                              ScheduleId = scheduleEntry.ScheduleId,
                              EntryTime = scheduleEntry.EntryTime,
                              NotificationMessage = scheduleEntry.NotificationMessage,
                              EntryDate = scheduleEntry.EntryDate,
                              EntryDateFormatted = scheduleEntry.EntryDateFormatted,
                              EntryDateDayOfWeek = scheduleEntry.EntryDateDayOfWeek,
                              NotificationDate = scheduleEntry.NotificationDate,
                              NotificationDateFormatted = scheduleEntry.NotificationDateFormatted,
                              NotificationDateDayOfWeek = scheduleEntry.NotificationDateDayOfWeek,
                              SMSNotificationSent = scheduleEntry.SMSNotificationSent,
                              SMSMessageId = scheduleEntry.SMSMessageId,
                              SMSDateSent = scheduleEntry.SMSDateSent,
                              SmsSentLogId = scheduleEntry.SmsSentLogId,
                              DateCreated = scheduleEntry.DateCreated,

                              //Schedule
                              SubscriptionId = scheduleView.SubscriptionId,
                              ScheduleNotificationMessage = scheduleView.NotificationMessage,
                              ScheduleName = scheduleView.ScheduleName,
                              Quantity = scheduleView.Quantity,
                              UnitOfMeasure = scheduleView.UnitOfMeasure,
                              DaysRepeatInterval = scheduleView.DaysRepeatInterval,
                              ScheduleNotes = scheduleView.Notes,
                              ScheduleDateCreated = scheduleView.DateCreated,

                              //Subscriber
                              CellPhoneNumber = subscriberView.CellPhoneNumber,

                              //Subscription View
                              CustomerFullName = subscriptionView.CustomerFullName,
                              CustomerEmailAddress = subscriptionView.CustomerEmailAddress,
                              CustomerIdentifier = subscriptionView.CustomerIdentifier,
                              CustomerPhysicalAddress = subscriptionView.CustomerPhysicalAddress,
                              CustomerNotes = subscriptionView.CustomerNotes
                          }).ToList();
            }
            return result;
        }

        public List<ScheduleEntry> GetScheduleEntriesForScheduleByFilter(string searchFilter, Nullable<Guid> scheduleId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<ScheduleEntry> result = null;
            if (scheduleId.HasValue)
            {
                result = (from e in DB.GetTable<ScheduleEntry>()
                          where e.ScheduleId == scheduleId.Value &&
                          (e.EntryDateFormatted.Contains(searchFilterLower) ||
                          e.NotificationDateFormatted.Contains(searchFilterLower) ||
                          e.EntryDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          e.NotificationDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          e.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (e.SMSMessageId != null && e.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          select e).ToList();
            }
            else
            {
                result = (from e in DB.GetTable<ScheduleEntry>()
                          where (e.EntryDateFormatted.Contains(searchFilterLower) ||
                          e.NotificationDateFormatted.Contains(searchFilterLower) ||
                          e.EntryDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          e.NotificationDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          e.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (e.SMSMessageId != null && e.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          select e).ToList();
            }
            return result;
        }

        public void DeleteScheduleEntriesForScheduleByFilter(string searchFilter, Nullable<Guid> scheduleId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<ScheduleEntry> scheduleEntries = GetScheduleEntriesForScheduleByFilter(searchFilter, scheduleId);
                DB.GetTable<ScheduleEntry>().DeleteAllOnSubmit(scheduleEntries);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Queries filtering on Schedule

        public List<ScheduleEntryView> GetScheduleEntryViewsForOrganizationByFilter(
            string searchFilter, 
            Nullable<Guid> organizationId, 
            DateTime date)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<ScheduleEntryView> result = null;
            if (organizationId.HasValue)
            {
                result = (from scheduleEntry in DB.GetTable<ScheduleEntry>()
                          join schedule in DB.GetTable<Schedule>() on scheduleEntry.ScheduleId equals schedule.ScheduleId into setSchedule
                          from scheduleView in setSchedule.DefaultIfEmpty()
                          join subscription in DB.GetTable<Subscription>() on scheduleView.SubscriptionId equals subscription.SubscriptionId into setSubscription
                          from subscriptionView in setSubscription.DefaultIfEmpty()
                          join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                          from subscriberView in setSubscriber.DefaultIfEmpty()
                          where subscriptionView.OrganizationId == organizationId.Value &&
                          (scheduleEntry.EntryDate.Date == date.Date || scheduleEntry.NotificationDate.Date == date.Date) &&
                          (subscriptionView.CustomerFullName.ToLower().Contains(searchFilterLower) ||
                          scheduleView.ScheduleName.ToLower().Contains(searchFilterLower) ||
                          subscriberView.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          scheduleEntry.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (scheduleEntry.SMSMessageId != null && scheduleEntry.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          orderby scheduleEntry.EntryDate
                          select new ScheduleEntryView()
                          {
                              //Schedule Entry
                              ScheduleEntryId = scheduleEntry.ScheduleEntryId,
                              ScheduleId = scheduleEntry.ScheduleId,
                              NotificationMessage = scheduleEntry.NotificationMessage,
                              EntryTime = scheduleEntry.EntryTime,
                              EntryDate = scheduleEntry.EntryDate,
                              EntryDateFormatted = scheduleEntry.EntryDateFormatted,
                              EntryDateDayOfWeek = scheduleEntry.EntryDateDayOfWeek,
                              NotificationDate = scheduleEntry.NotificationDate,
                              NotificationDateFormatted = scheduleEntry.NotificationDateFormatted,
                              NotificationDateDayOfWeek = scheduleEntry.NotificationDateDayOfWeek,
                              SMSNotificationSent = scheduleEntry.SMSNotificationSent,
                              SMSMessageId = scheduleEntry.SMSMessageId,
                              SMSDateSent = scheduleEntry.SMSDateSent,
                              SmsSentLogId = scheduleEntry.SmsSentLogId,
                              DateCreated = scheduleEntry.DateCreated,

                              //Schedule
                              SubscriptionId = scheduleView.SubscriptionId,
                              ScheduleNotificationMessage = scheduleView.NotificationMessage,
                              ScheduleName = scheduleView.ScheduleName,
                              Quantity = scheduleView.Quantity,
                              UnitOfMeasure = scheduleView.UnitOfMeasure,
                              DaysRepeatInterval = scheduleView.DaysRepeatInterval,
                              ScheduleNotes = scheduleView.Notes,
                              ScheduleDateCreated = scheduleView.DateCreated,

                              //Subscriber
                              CellPhoneNumber = subscriberView.CellPhoneNumber,

                              //Subscription
                              CustomerFullName = subscriptionView.CustomerFullName,
                              CustomerEmailAddress = subscriptionView.CustomerEmailAddress,
                              CustomerIdentifier = subscriptionView.CustomerIdentifier,
                              CustomerPhysicalAddress = subscriptionView.CustomerPhysicalAddress,
                              CustomerNotes = subscriptionView.CustomerNotes
                          }).ToList();
            }
            else
            {
                result = (from scheduleEntry in DB.GetTable<ScheduleEntry>()
                          join schedule in DB.GetTable<Schedule>() on scheduleEntry.ScheduleId equals schedule.ScheduleId into setSchedule
                          from scheduleView in setSchedule.DefaultIfEmpty()
                          join subscription in DB.GetTable<Subscription>() on scheduleView.SubscriptionId equals subscription.SubscriptionId into setSubscription
                          from subscriptionView in setSubscription.DefaultIfEmpty()
                          join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                          from subscriberView in setSubscriber.DefaultIfEmpty()
                          where (scheduleEntry.EntryDate.Date == date.Date || scheduleEntry.NotificationDate.Date == date.Date) &&
                          (subscriptionView.CustomerFullName.ToLower().Contains(searchFilterLower) ||
                          scheduleView.ScheduleName.ToLower().Contains(searchFilterLower) ||
                          subscriberView.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          scheduleEntry.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (scheduleEntry.SMSMessageId != null && scheduleEntry.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          orderby scheduleEntry.EntryDate
                          select new ScheduleEntryView()
                          {
                              //Schedule Entry
                              ScheduleEntryId = scheduleEntry.ScheduleEntryId,
                              ScheduleId = scheduleEntry.ScheduleId,
                              NotificationMessage = scheduleEntry.NotificationMessage,
                              EntryTime = scheduleEntry.EntryTime,
                              EntryDate = scheduleEntry.EntryDate,
                              EntryDateFormatted = scheduleEntry.EntryDateFormatted,
                              EntryDateDayOfWeek = scheduleEntry.EntryDateDayOfWeek,
                              NotificationDate = scheduleEntry.NotificationDate,
                              NotificationDateFormatted = scheduleEntry.NotificationDateFormatted,
                              NotificationDateDayOfWeek = scheduleEntry.NotificationDateDayOfWeek,
                              SMSNotificationSent = scheduleEntry.SMSNotificationSent,
                              SMSMessageId = scheduleEntry.SMSMessageId,
                              SMSDateSent = scheduleEntry.SMSDateSent,
                              SmsSentLogId = scheduleEntry.SmsSentLogId,
                              DateCreated = scheduleEntry.DateCreated,

                              //Schedule
                              SubscriptionId = scheduleView.SubscriptionId,
                              ScheduleNotificationMessage = scheduleView.NotificationMessage,
                              ScheduleName = scheduleView.ScheduleName,
                              Quantity = scheduleView.Quantity,
                              UnitOfMeasure = scheduleView.UnitOfMeasure,
                              DaysRepeatInterval = scheduleView.DaysRepeatInterval,
                              ScheduleNotes = scheduleView.Notes,
                              ScheduleDateCreated = scheduleView.DateCreated,

                              //Subscriber
                              CellPhoneNumber = subscriberView.CellPhoneNumber,

                              //Subscription
                              CustomerFullName = subscriptionView.CustomerFullName,
                              CustomerEmailAddress = subscriptionView.CustomerEmailAddress,
                              CustomerIdentifier = subscriptionView.CustomerIdentifier,
                              CustomerPhysicalAddress = subscriptionView.CustomerPhysicalAddress,
                              CustomerNotes = subscriptionView.CustomerNotes
                          }).ToList();
            }
            return result;
        }

        public List<ScheduleEntry> GetScheduleEntriesForOrganizationByFilter(
            string searchFilter, 
            Nullable<Guid> organizationId,
            DateTime date)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<ScheduleEntry> result = null;
            if (organizationId.HasValue)
            {
                result = (from scheduleEntry in DB.GetTable<ScheduleEntry>()
                          join schedule in DB.GetTable<Schedule>() on scheduleEntry.ScheduleId equals schedule.ScheduleId into setSchedule
                          from scheduleView in setSchedule.DefaultIfEmpty()
                          join subscription in DB.GetTable<Subscription>() on scheduleView.SubscriptionId equals subscription.SubscriptionId into setSubscription
                          from subscriptionView in setSubscription.DefaultIfEmpty()
                          join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                          from subscriberView in setSubscriber.DefaultIfEmpty()
                          where subscriptionView.OrganizationId == organizationId.Value &&
                          (scheduleEntry.EntryDate.Date == date.Date || scheduleEntry.NotificationDate.Date == date.Date) &&
                          (subscriptionView.CustomerFullName.ToLower().Contains(searchFilterLower) ||
                          scheduleView.ScheduleName.ToLower().Contains(searchFilterLower) ||
                          subscriberView.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          scheduleEntry.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (scheduleEntry.SMSMessageId != null && scheduleEntry.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          select scheduleEntry).ToList();
            }
            else
            {
                result = (from scheduleEntry in DB.GetTable<ScheduleEntry>()
                          join schedule in DB.GetTable<Schedule>() on scheduleEntry.ScheduleId equals schedule.ScheduleId into setSchedule
                          from scheduleView in setSchedule.DefaultIfEmpty()
                          join subscription in DB.GetTable<Subscription>() on scheduleView.SubscriptionId equals subscription.SubscriptionId into setSubscription
                          from subscriptionView in setSubscription.DefaultIfEmpty()
                          join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                          from subscriberView in setSubscriber.DefaultIfEmpty()
                          where (scheduleEntry.EntryDate.Date == date.Date || scheduleEntry.NotificationDate.Date == date.Date) &&
                          (subscriptionView.CustomerFullName.ToLower().Contains(searchFilterLower) ||
                          scheduleView.ScheduleName.ToLower().Contains(searchFilterLower) ||
                          subscriberView.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          scheduleEntry.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (scheduleEntry.SMSMessageId != null && scheduleEntry.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          select scheduleEntry).ToList();
            }
            return result;
        }

        public void DeleteScheduleEntriesForOrganizationByFilter(
            string searchFilter, 
            Nullable<Guid> organizationId,
            DateTime date)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<ScheduleEntry> scheduleEntries = GetScheduleEntriesForOrganizationByFilter(searchFilter, organizationId, date);
                DB.GetTable<ScheduleEntry>().DeleteAllOnSubmit(scheduleEntries);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Schedule Entry
    }
}