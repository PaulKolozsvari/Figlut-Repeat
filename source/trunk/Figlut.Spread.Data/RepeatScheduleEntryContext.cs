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
                                              where repeatScheduleEntry.RepeatScheduleEntryId == repeatScheduleEntryId
                                              select new RepeatScheduleEntryView()
                                              {
                                                  RepeatScheduleEntryId = repeatScheduleEntry.RepeatScheduleEntryId,
                                                  RepeatScheduleId = repeatScheduleEntry.RepeatScheduleId,
                                                  RepeatDate = repeatScheduleEntry.RepeatDate,
                                                  NotificationDate = repeatScheduleEntry.NotificationDate,
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
                                                  RepeatScheduleDateCreated = repeatScheduleView.DateCreated
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
                result = (from e in DB.GetTable<RepeatScheduleEntry>()
                          join r in DB.GetTable<RepeatSchedule>() on e.RepeatScheduleId equals r.RepeatScheduleId into setRepeatSchedule
                          from repeatScheduleView in setRepeatSchedule.DefaultIfEmpty()
                          where e.RepeatScheduleId == repeatScheduleId.Value ||
                          (e.RepeatDate.ToString().ToLower().Contains(searchFilterLower) ||
                          e.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (!string.IsNullOrEmpty(e.SMSMessageId) && e.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          select new RepeatScheduleEntryView()
                          {
                              RepeatScheduleEntryId = e.RepeatScheduleEntryId,
                              RepeatScheduleId = e.RepeatScheduleId,
                              RepeatDate = e.RepeatDate,
                              NotificationDate = e.NotificationDate,
                              SMSNotificationSent = e.SMSNotificationSent,
                              SMSMessageId = e.SMSMessageId,
                              SMSDateSent = e.SMSDateSent,
                              SmsSentLogId = e.SmsSentLogId,
                              DateCreated = e.DateCreated,

                              //Repeat Schedule
                              SubscriptionId = repeatScheduleView.SubscriptionId,
                              NotificationMessage = repeatScheduleView.NotificationMessage,
                              ScheduleName = repeatScheduleView.ScheduleName,
                              Quantity = repeatScheduleView.Quantity,
                              UnitOfMeasure = repeatScheduleView.UnitOfMeasure,
                              DaysRepeatInterval = repeatScheduleView.DaysRepeatInterval,
                              RepeatScheduleNotes = repeatScheduleView.Notes,
                              RepeatScheduleDateCreated = repeatScheduleView.DateCreated
                          }).ToList();
            }
            else
            {
                result = (from e in DB.GetTable<RepeatScheduleEntry>()
                          join r in DB.GetTable<RepeatSchedule>() on e.RepeatScheduleId equals r.RepeatScheduleId into setRepeatSchedule
                          from repeatScheduleView in setRepeatSchedule.DefaultIfEmpty()
                          where (e.RepeatDate.ToString().ToLower().Contains(searchFilterLower) ||
                          e.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (!string.IsNullOrEmpty(e.SMSMessageId) && e.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          select new RepeatScheduleEntryView()
                          {
                              RepeatScheduleEntryId = e.RepeatScheduleEntryId,
                              RepeatScheduleId = e.RepeatScheduleId,
                              RepeatDate = e.RepeatDate,
                              NotificationDate = e.NotificationDate,
                              SMSNotificationSent = e.SMSNotificationSent,
                              SMSMessageId = e.SMSMessageId,
                              SMSDateSent = e.SMSDateSent,
                              SmsSentLogId = e.SmsSentLogId,
                              DateCreated = e.DateCreated,

                              //Repeat Schedule
                              SubscriptionId = repeatScheduleView.SubscriptionId,
                              NotificationMessage = repeatScheduleView.NotificationMessage,
                              ScheduleName = repeatScheduleView.ScheduleName,
                              Quantity = repeatScheduleView.Quantity,
                              UnitOfMeasure = repeatScheduleView.UnitOfMeasure,
                              DaysRepeatInterval = repeatScheduleView.DaysRepeatInterval,
                              RepeatScheduleNotes = repeatScheduleView.Notes,
                              RepeatScheduleDateCreated = repeatScheduleView.DateCreated
                          }).ToList();
            }
            return result;
        }

        public List<RepeatScheduleEntry> GetRepeatScheduleEntriesbyFilter(string searchFilter, Nullable<Guid> repeatScheduleId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<RepeatScheduleEntry> result = null;
            if (repeatScheduleId.HasValue)
            {
                result = (from e in DB.GetTable<RepeatScheduleEntry>()
                          where e.RepeatScheduleId == repeatScheduleId.Value &&
                          (e.RepeatDate.ToString().ToLower().Contains(searchFilterLower) ||
                          e.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (!string.IsNullOrEmpty(e.SMSMessageId) && e.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          select e).ToList();
            }
            else
            {
                result = (from e in DB.GetTable<RepeatScheduleEntry>()
                          where (e.RepeatDate.ToString().ToLower().Contains(searchFilterLower) ||
                          e.SMSNotificationSent.ToString().ToLower().Contains(searchFilterLower) ||
                          (!string.IsNullOrEmpty(e.SMSMessageId) && e.SMSMessageId.ToLower().Contains(searchFilterLower)))
                          select e).ToList();
            }
            return result;
        }

        public void DeleteRepeatScheduleEntriesbyFilter(string searchFilter, Nullable<Guid> repeatScheduleId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<RepeatScheduleEntry> repeatScheduleEntries = GetRepeatScheduleEntriesbyFilter(searchFilter, repeatScheduleId);
                DB.GetTable<RepeatScheduleEntry>().DeleteAllOnSubmit(repeatScheduleEntries);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Repeat Schedule Entry
    }
}
