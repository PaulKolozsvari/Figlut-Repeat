﻿namespace Figlut.Spread.Data
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
    using Figlut.Spread.ORM.Views;
    using System.Transactions;

    #endregion //Using Directives

    public partial class SpreadEntityContext : EntityContext
    {
        #region Repeat Schedule Entry

        public RepeatScheduleEntry GetRepeatScheduleEntry(Guid repeatScheduleEntryId, bool throwExceptionOnNotFound)
        {
            RepeatScheduleEntry result = (from r in DB.GetTable<RepeatScheduleEntry>()
                                          where r.RepeatScheduleEntryId == repeatScheduleEntryId
                                          select r).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(RepeatScheduleEntry).Name,
                    EntityReader<RepeatScheduleEntry>.GetPropertyName(p => p.RepeatScheduleId, false),
                    repeatScheduleEntryId.ToString()));
            }
            return result;
        }

        public long GetAllRepeatScheduleEntryCount()
        {
            return DB.GetTable<RepeatScheduleEntry>().LongCount();
        }

        public RepeatScheduleEntryView GetRepeatScheduleEntryView(Guid repeatScheduleEntryId, bool throwExceptionOnNotFound)
        {
            RepeatScheduleEntryView result = (from repeatScheduleEntry in DB.GetTable<RepeatScheduleEntry>()
                                              join repeatSchedule in DB.GetTable<RepeatSchedule>() on repeatScheduleEntry.RepeatScheduleId equals repeatSchedule.RepeatScheduleId into setRepeatSchedule
                                              from repeatScheduleView in setRepeatSchedule.DefaultIfEmpty()
                                              join subscription in DB.GetTable<Subscription>() on repeatScheduleView.SubscriptionId equals subscription.SubscriptionId into setSubscription
                                              from subscriptionView in setSubscription.DefaultIfEmpty()
                                              join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                                              from subscriberView in setSubscriber.DefaultIfEmpty()
                                              select new RepeatScheduleEntryView()
                                              {
                                                  //Repeat Schedule Entry
                                                  RepeatScheduleEntryId = repeatScheduleEntry.RepeatScheduleEntryId,
                                                  RepeatScheduleId = repeatScheduleEntry.RepeatScheduleId,
                                                  RepeatDate = repeatScheduleEntry.RepeatDate,
                                                  RepeatDateFormatted = repeatScheduleEntry.RepeatDateFormatted,
                                                  RepeatDateDayOfWeek = repeatScheduleEntry.RepeatDateDayOfWeek,
                                                  NotificationDate = repeatScheduleEntry.NotificationDate,
                                                  NotificationDateFormatted = repeatScheduleEntry.NotificationDateFormatted,
                                                  NotificationDateDayOfWeek = repeatScheduleEntry.NotificationDateDayOfWeek,
                                                  SMSNotificationSent = repeatScheduleEntry.SMSNotificationSent,
                                                  SMSMessageId = repeatScheduleEntry.SMSMessageId,
                                                  SMSDateSent = repeatScheduleEntry.SMSDateSent,
                                                  SmsSentLogId = repeatScheduleEntry.SmsSentLogId,
                                                  DateCreated = repeatScheduleEntry.DateCreated,

                                                  //Repeat Schedule
                                                  SubscriptionId = repeatScheduleView.SubscriptionId,
                                                  NotificationMessage = repeatScheduleView.NotificationMessage,
                                                  ScheduleName = repeatScheduleView.ScheduleName,
                                                  Quantity = repeatScheduleView.Quantity,
                                                  UnitOfMeasure = repeatScheduleView.UnitOfMeasure,
                                                  DaysRepeatInterval = repeatScheduleView.DaysRepeatInterval,
                                                  RepeatScheduleNotes = repeatScheduleView.Notes,
                                                  RepeatScheduleDateCreated = repeatScheduleView.DateCreated,

                                                  //Subscriber
                                                  CellPhoneNumber = subscriberView.CellPhoneNumber
                                              }).FirstOrDefault();

            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(RepeatScheduleEntryView).Name,
                    EntityReader<RepeatScheduleEntryView>.GetPropertyName(p => p.RepeatScheduleEntryId, false),
                    repeatScheduleEntryId.ToString()));
            }
            return result;
        }

        public List<RepeatScheduleEntryView> GetRepeatScheduleEntryViewsByFilter(string searchFilter, Nullable<Guid> repeatScheduleId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<RepeatScheduleEntryView> result = null;
            if (repeatScheduleId.HasValue)
            {
                result = (from repeatScheduleEntry in DB.GetTable<RepeatScheduleEntry>()
                          join repeatSchedule in DB.GetTable<RepeatSchedule>() on repeatScheduleEntry.RepeatScheduleId equals repeatSchedule.RepeatScheduleId into setRepeatSchedule
                          from repeatScheduleView in setRepeatSchedule.DefaultIfEmpty()
                          join subscription in DB.GetTable<Subscription>() on repeatScheduleView.SubscriptionId equals subscription.SubscriptionId into setSubscription
                          from subscriptionView in setSubscription.DefaultIfEmpty()
                          join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                          from subscriberView in setSubscriber.DefaultIfEmpty()
                          where repeatScheduleEntry.RepeatScheduleId == repeatScheduleId.Value &&
                          (repeatScheduleEntry.RepeatDateFormatted.Contains(searchFilterLower) ||
                          repeatScheduleEntry.NotificationDateFormatted.Contains(searchFilterLower) ||
                          repeatScheduleEntry.RepeatDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          repeatScheduleEntry.NotificationDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          repeatScheduleEntry.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (repeatScheduleEntry.SMSMessageId != null && repeatScheduleEntry.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          orderby repeatScheduleEntry.RepeatDate
                          select new RepeatScheduleEntryView()
                          {
                              //Repeat Schedule Entry
                              RepeatScheduleEntryId = repeatScheduleEntry.RepeatScheduleEntryId,
                              RepeatScheduleId = repeatScheduleEntry.RepeatScheduleId,
                              RepeatDate = repeatScheduleEntry.RepeatDate,
                              RepeatDateFormatted = repeatScheduleEntry.RepeatDateFormatted,
                              RepeatDateDayOfWeek = repeatScheduleEntry.RepeatDateDayOfWeek,
                              NotificationDate = repeatScheduleEntry.NotificationDate,
                              NotificationDateFormatted = repeatScheduleEntry.NotificationDateFormatted,
                              NotificationDateDayOfWeek = repeatScheduleEntry.NotificationDateDayOfWeek,
                              SMSNotificationSent = repeatScheduleEntry.SMSNotificationSent,
                              SMSMessageId = repeatScheduleEntry.SMSMessageId,
                              SMSDateSent = repeatScheduleEntry.SMSDateSent,
                              SmsSentLogId = repeatScheduleEntry.SmsSentLogId,
                              DateCreated = repeatScheduleEntry.DateCreated,

                              //Repeat Schedule
                              SubscriptionId = repeatScheduleView.SubscriptionId,
                              NotificationMessage = repeatScheduleView.NotificationMessage,
                              ScheduleName = repeatScheduleView.ScheduleName,
                              Quantity = repeatScheduleView.Quantity,
                              UnitOfMeasure = repeatScheduleView.UnitOfMeasure,
                              DaysRepeatInterval = repeatScheduleView.DaysRepeatInterval,
                              RepeatScheduleNotes = repeatScheduleView.Notes,
                              RepeatScheduleDateCreated = repeatScheduleView.DateCreated,

                              //Subscriber
                              CellPhoneNumber = subscriberView.CellPhoneNumber,
                          }).ToList();
            }
            else
            {
                result = (from repeatScheduleEntry in DB.GetTable<RepeatScheduleEntry>()
                          join repeatSchedule in DB.GetTable<RepeatSchedule>() on repeatScheduleEntry.RepeatScheduleId equals repeatSchedule.RepeatScheduleId into setRepeatSchedule
                          from repeatScheduleView in setRepeatSchedule.DefaultIfEmpty()
                          join subscription in DB.GetTable<Subscription>() on repeatScheduleView.SubscriptionId equals subscription.SubscriptionId into setSubscription
                          from subscriptionView in setSubscription.DefaultIfEmpty()
                          join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                          from subscriberView in setSubscriber.DefaultIfEmpty()
                          where (repeatScheduleEntry.RepeatDateFormatted.Contains(searchFilterLower) ||
                          repeatScheduleEntry.NotificationDateFormatted.Contains(searchFilterLower) ||
                          repeatScheduleEntry.RepeatDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          repeatScheduleEntry.NotificationDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          repeatScheduleEntry.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (repeatScheduleEntry.SMSMessageId != null && repeatScheduleEntry.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          orderby repeatScheduleEntry.RepeatDate
                          select new RepeatScheduleEntryView()
                          {
                              //Repeat Schedule Entry
                              RepeatScheduleEntryId = repeatScheduleEntry.RepeatScheduleEntryId,
                              RepeatScheduleId = repeatScheduleEntry.RepeatScheduleId,
                              RepeatDate = repeatScheduleEntry.RepeatDate,
                              RepeatDateFormatted = repeatScheduleEntry.RepeatDateFormatted,
                              RepeatDateDayOfWeek = repeatScheduleEntry.RepeatDateDayOfWeek,
                              NotificationDate = repeatScheduleEntry.NotificationDate,
                              NotificationDateFormatted = repeatScheduleEntry.NotificationDateFormatted,
                              NotificationDateDayOfWeek = repeatScheduleEntry.NotificationDateDayOfWeek,
                              SMSNotificationSent = repeatScheduleEntry.SMSNotificationSent,
                              SMSMessageId = repeatScheduleEntry.SMSMessageId,
                              SMSDateSent = repeatScheduleEntry.SMSDateSent,
                              SmsSentLogId = repeatScheduleEntry.SmsSentLogId,
                              DateCreated = repeatScheduleEntry.DateCreated,

                              //Repeat Schedule
                              SubscriptionId = repeatScheduleView.SubscriptionId,
                              NotificationMessage = repeatScheduleView.NotificationMessage,
                              ScheduleName = repeatScheduleView.ScheduleName,
                              Quantity = repeatScheduleView.Quantity,
                              UnitOfMeasure = repeatScheduleView.UnitOfMeasure,
                              DaysRepeatInterval = repeatScheduleView.DaysRepeatInterval,
                              RepeatScheduleNotes = repeatScheduleView.Notes,
                              RepeatScheduleDateCreated = repeatScheduleView.DateCreated,

                              //Subscriber
                              CellPhoneNumber = subscriberView.CellPhoneNumber,
                          }).ToList();
            }
            return result;
        }

        public List<RepeatScheduleEntry> GetRepeatScheduleEntriesByFilter(string searchFilter, Nullable<Guid> repeatScheduleId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<RepeatScheduleEntry> result = null;
            if (repeatScheduleId.HasValue)
            {
                result = (from e in DB.GetTable<RepeatScheduleEntry>()
                          where e.RepeatScheduleId == repeatScheduleId.Value &&
                          (e.RepeatDateFormatted.Contains(searchFilterLower) ||
                          e.NotificationDateFormatted.Contains(searchFilterLower) ||
                          e.RepeatDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          e.NotificationDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          e.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (e.SMSMessageId != null && e.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          select e).ToList();
            }
            else
            {
                result = (from e in DB.GetTable<RepeatScheduleEntry>()
                          where (e.RepeatDateFormatted.Contains(searchFilterLower) ||
                          e.NotificationDateFormatted.Contains(searchFilterLower) ||
                          e.RepeatDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          e.NotificationDateDayOfWeek.ToLower().Contains(searchFilterLower) ||
                          e.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (e.SMSMessageId != null && e.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          select e).ToList();
            }
            return result;
        }

        public void DeleteRepeatScheduleEntriesbyFilter(string searchFilter, Nullable<Guid> repeatScheduleId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<RepeatScheduleEntry> repeatScheduleEntries = GetRepeatScheduleEntriesByFilter(searchFilter, repeatScheduleId);
                DB.GetTable<RepeatScheduleEntry>().DeleteAllOnSubmit(repeatScheduleEntries);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Repeat Schedule Entry
    }
}
