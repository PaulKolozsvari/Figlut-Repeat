namespace Figlut.Spread.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.ORM.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;
    using Figlut.Server.Toolkit.Data;

    #endregion //Using Directives

    public partial class SpreadEntityContext : EntityContext
    {
        #region Schedule

        public Schedule GetSchedule(Guid scheduleId, bool throwExceptionOnNotFound)
        {
            Schedule result = (from r in DB.GetTable<Schedule>()
                               where r.ScheduleId == scheduleId
                               select r).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Schedule).Name,
                    EntityReader<Schedule>.GetPropertyName(p => p.ScheduleId, false),
                    scheduleId.ToString()));
            }
            return result;
        }

        public long GetAllScheduleCount()
        {
            return DB.GetTable<Schedule>().LongCount();
        }

        public CreateScheduleView GetCreateScheduleModelView(Guid subscriptionId, bool throwExceptionOnNotFound)
        {
            CreateScheduleView result = (from subscription in DB.GetTable<Subscription>()
                                               join subscriber in DB.GetTable<Subscriber>() on subscription.SubscriberId equals subscriber.SubscriberId into setSubsciber
                                               from subscriberView in setSubsciber.DefaultIfEmpty()
                                               join organization in DB.GetTable<Organization>() on subscription.OrganizationId equals organization.OrganizationId into setOrganization
                                               from organizationView in setOrganization.DefaultIfEmpty()
                                               where subscription.SubscriptionId == subscriptionId
                                               select new CreateScheduleView()
                                               {
                                                   //Subscription
                                                   SubscriptionId = subscription.SubscriptionId,
                                                   OrganizationId = subscription.OrganizationId,
                                                   SubscriberId = subscription.SubscriberId,
                                                   SubscriptionEnabled = subscription.Enabled,
                                                   CustomerFullName = subscription.CustomerFullName,
                                                   CustomerIdentifier = subscription.CustomerIdentifier,
                                                   CustomerPhysicalAddress = subscription.CustomerPhysicalAddress,
                                                   CustomerNotes = subscription.CustomerNotes,
                                                   SubscriptionDateCreated = subscription.DateCreated,
                                                   //Subscriber
                                                   CellPhoneNumber = subscriberView.CellPhoneNumber,
                                                   SubscriberName = subscriberView.Name,
                                                   SubscriberEnabled = subscriberView.Enabled,
                                                   SubscriberDateCreated = subscriberView.DateCreated,
                                                   //Organization
                                                   OrganizationName = organizationView.Name,
                                                   OrganizationIdentifier = organizationView.Identifier,
                                                   OrganizationEmailAddress = organizationView.EmailAddress,
                                                   OrganizationAddress = organizationView.Address,
                                                   OrganizationSmsCreditsBalance = organizationView.SmsCreditsBalance,
                                                   OrganizationAllowSmsCreditsDebt = organizationView.AllowSmsCreditsDebt,
                                                   OrganizationSubscriptionTypeId = organizationView.OrganizationSubscriptionTypeId,
                                                   OrganizationSubscriptionEnabled = organizationView.OrganizationSubscriptionEnabled,
                                                   OrganizationBillingDayOfTheMonth = organizationView.BillingDayOfTheMonth,
                                                   IsMondayWorkDay = organizationView.IsMondayWorkDay,
                                                   IsTuesdayWorkDay = organizationView.IsTuesdayWorkDay,
                                                   IsWednesdayWorkDay = organizationView.IsWednesdayWorkDay,
                                                   IsThursdayWorkDay = organizationView.IsThursdayWorkDay,
                                                   IsFridayWorkDay = organizationView.IsFridayWorkDay,
                                                   IsSaturdayWorkDay = organizationView.IsSaturdayWorkDay,
                                                   IsSundayWorkDay = organizationView.IsSundayWorkDay,
                                                   AccountManagerUserId = organizationView.AccountManagerUserId,
                                                   OrganizationDateCreated = organizationView.DateCreated
                                               }).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(CreateScheduleView).Name,
                    EntityReader<CreateScheduleView>.GetPropertyName(p => p.SubscriptionId, false),
                    subscriptionId.ToString()));
            }
            return result;
        }

        public ScheduleView GetScheduleView(Guid scheduleId, bool throwExceptionOnNotFound)
        {
            ScheduleView result = (from schedule in DB.GetTable<Schedule>()
                                   join subscription in DB.GetTable<Subscription>() on schedule.SubscriptionId equals subscription.SubscriptionId into setSubscription
                                   from subscriptionView in setSubscription.DefaultIfEmpty()
                                   join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                                   from subscriberView in setSubscriber.DefaultIfEmpty()
                                   where schedule.ScheduleId == scheduleId
                                   select new ScheduleView()
                                   {
                                       //Schedule
                                       ScheduleId = schedule.ScheduleId,
                                       SubscriptionId = schedule.SubscriptionId,
                                       NotificationMessage = schedule.NotificationMessage,
                                       ScheduleName = schedule.ScheduleName,
                                       Quantity = schedule.Quantity,
                                       UnitOfMeasure = schedule.UnitOfMeasure,
                                       DaysRepeatInterval = schedule.DaysRepeatInterval,
                                       Notes = schedule.Notes,
                                       DateCreated = schedule.DateCreated,
                                       StartDate = GetFirstScheduleEntryDate(schedule.ScheduleId, false),
                                       EndDate = GetLastScheduleEntryDate(schedule.ScheduleId, false),
                                       EntryCount = GetScheduleEntriesCount(schedule.ScheduleId),

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
                    typeof(ScheduleView).Name,
                    EntityReader<ScheduleView>.GetPropertyName(p => p.ScheduleId, false),
                    scheduleId.ToString()));
            }
            if (result != null)
            {
                if (result.StartDate.HasValue)
                {
                    result.StartDateFormatted = DataShaper.GetDefaultDateString(result.StartDate.Value);
                }
                if (result.EndDate.HasValue)
                {
                    result.EndDateFormatted = DataShaper.GetDefaultDateString(result.EndDate.Value);
                }
            }
            return result;
        }

        public List<ScheduleView> GetScheduleViewsByFilter(string searchFilter, Nullable<Guid> subscriptionId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<ScheduleView> result = null;
            if (subscriptionId.HasValue)
            {
                result = (from schedule in DB.GetTable<Schedule>()
                          join subscription in DB.GetTable<Subscription>() on schedule.SubscriptionId equals subscription.SubscriptionId into setSubscription
                          from subscriptionView in setSubscription.DefaultIfEmpty()
                          join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                          from subscriberView in setSubscriber
                          where schedule.SubscriptionId == subscriptionId.Value &&
                          (schedule.ScheduleName.ToLower().Contains(searchFilterLower) ||
                          schedule.NotificationMessage.ToLower().Contains(searchFilterLower) ||
                          schedule.DaysRepeatInterval.ToString().Contains(searchFilterLower) ||
                          schedule.UnitOfMeasure.ToLower().Contains(searchFilterLower) ||
                          schedule.Notes.ToLower().Contains(searchFilterLower))
                          select new ScheduleView()
                          {
                              //Schedule
                              ScheduleId = schedule.ScheduleId,
                              SubscriptionId = schedule.SubscriptionId,
                              NotificationMessage = schedule.NotificationMessage,
                              ScheduleName = schedule.ScheduleName,
                              Quantity = schedule.Quantity,
                              UnitOfMeasure = schedule.UnitOfMeasure,
                              DaysRepeatInterval = schedule.DaysRepeatInterval,
                              Notes = schedule.Notes,
                              DateCreated = schedule.DateCreated,
                              StartDate = GetFirstScheduleEntryDate(schedule.ScheduleId, false),
                              EndDate = GetLastScheduleEntryDate(schedule.ScheduleId, false),
                              EntryCount = GetScheduleEntriesCount(schedule.ScheduleId),

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
                result = (from schedule in DB.GetTable<Schedule>()
                          join subscription in DB.GetTable<Subscription>() on schedule.SubscriptionId equals subscription.SubscriptionId into setSubscription
                          from subscriptionView in setSubscription.DefaultIfEmpty()
                          join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                          from subscriberView in setSubscriber
                          where schedule.ScheduleName.ToLower().Contains(searchFilterLower) ||
                          schedule.NotificationMessage.ToLower().Contains(searchFilterLower) ||
                          schedule.DaysRepeatInterval.ToString().Contains(searchFilterLower) ||
                          schedule.UnitOfMeasure.ToLower().Contains(searchFilterLower) ||
                          schedule.Notes.ToLower().Contains(searchFilterLower)
                          select new ScheduleView()
                          {
                              //Schedule
                              ScheduleId = schedule.ScheduleId,
                              SubscriptionId = schedule.SubscriptionId,
                              NotificationMessage = schedule.NotificationMessage,
                              ScheduleName = schedule.ScheduleName,
                              Quantity = schedule.Quantity,
                              UnitOfMeasure = schedule.UnitOfMeasure,
                              DaysRepeatInterval = schedule.DaysRepeatInterval,
                              Notes = schedule.Notes,
                              DateCreated = schedule.DateCreated,
                              StartDate = GetFirstScheduleEntryDate(schedule.ScheduleId, false),
                              EndDate = GetLastScheduleEntryDate(schedule.ScheduleId, false),
                              EntryCount = GetScheduleEntriesCount(schedule.ScheduleId),

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
            foreach (ScheduleView r in result)
            {
                if (r.StartDate.HasValue)
                {
                    r.StartDateFormatted = DataShaper.GetDefaultDateString(r.StartDate.Value);
                }
                if (r.EndDate.HasValue)
                {
                    r.EndDateFormatted = DataShaper.GetDefaultDateString(r.EndDate.Value);
                }
            }
            return result;
        }

        public List<Schedule> GetScheduleByFilter(string searchFilter, Nullable<Guid> subscriptionId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<Schedule> result = null;
            if (subscriptionId.HasValue)
            {
                result = (from r in DB.GetTable<Schedule>()
                          where r.SubscriptionId == subscriptionId.Value &&
                          (r.ScheduleName.ToLower().Contains(searchFilterLower) ||
                          r.Notes.ToLower().Contains(searchFilterLower))
                          select r).ToList();
            }
            else
            {
                result = (from r in DB.GetTable<Schedule>()
                          where (r.ScheduleName.ToLower().Contains(searchFilterLower) ||
                          r.Notes.ToLower().Contains(searchFilterLower))
                          select r).ToList();
            }
            return result;
        }

        public void DeleteSchedulesByFilter(string searchFilter, Nullable<Guid> subscriptionId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<Schedule> schedules = GetScheduleByFilter(searchFilter, subscriptionId);
                foreach (Schedule s in schedules) //Delete all the children first.
                {
                    List<ScheduleEntry> scheduleEntries = GetScheduleEntriesForScheduleByFilter(null, s.ScheduleId);
                    DB.GetTable<ScheduleEntry>().DeleteAllOnSubmit(scheduleEntries);
                    DB.SubmitChanges();
                }
                DB.GetTable<Schedule>().DeleteAllOnSubmit(schedules);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        public void DeleteScheduleAndEntries(Guid scheduleId, bool throwExceptionOnNotFound)
        {
            using (TransactionScope t = new TransactionScope())
            {
                Schedule schedule = GetSchedule(scheduleId, throwExceptionOnNotFound);
                List<ScheduleEntry> scheduleEntries = GetScheduleEntriesForScheduleByFilter(null, schedule.ScheduleId);
                DB.GetTable<ScheduleEntry>().DeleteAllOnSubmit(scheduleEntries); //Delete the children first.
                DB.SubmitChanges();
                DB.GetTable<Schedule>().DeleteOnSubmit(schedule);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #region Schedule Generation

        public Schedule CreateSchedule(
            CreateScheduleView view)
        {
            Schedule result = null;
            using (TransactionScope t = new TransactionScope())
            {
                Country country = GetCountry(view.CountryId, true);
                Dictionary<string, EntryDateSet> dates = GenerateScheduleDates(view.StartDate, view.EndDate, country.CountryCode, view.DaysRepeatInterval);
                result = view.ToSchedule();
                DB.GetTable<Schedule>().InsertOnSubmit(result);
                List<ScheduleEntry> entries = new List<ScheduleEntry>();
                dates.Values.ToList().ForEach(p => entries.Add(new ScheduleEntry()
                {
                    ScheduleEntryId = Guid.NewGuid(),
                    ScheduleId = result.ScheduleId,
                    EntryDate = p.EntryDate,
                    EntryDateFormatted = DataShaper.GetDefaultDateString(p.EntryDate),
                    EntryDateDayOfWeek = p.EntryDate.DayOfWeek.ToString(),
                    NotificationDate = p.NotificationDate,
                    NotificationDateFormatted = DataShaper.GetDefaultDateString(p.NotificationDate),
                    NotificationDateDayOfWeek = p.NotificationDate.DayOfWeek.ToString(),
                    SMSNotificationSent = false,
                    SMSMessageId = null,
                    SMSDateSent = null,
                    SmsSentLogId = null,
                    DateCreated = DateTime.Now
                }));
                DB.GetTable<ScheduleEntry>().InsertAllOnSubmit(entries);
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        public Dictionary<string, EntryDateSet> GenerateScheduleDates(
            DateTime startDate,
            DateTime endDate,
            string countryCode,
            int daysRepeatInterval)
        {
            if (startDate > endDate)
            {
                throw new ArgumentOutOfRangeException("Start Date may not be greater than End Date when generating Schedule Dates.");
            }
            Dictionary<string, EntryDateSet> result = new Dictionary<string, EntryDateSet>();
            while (startDate.Date <= endDate.Date)
            {
                EntryDateSet dateSet = new EntryDateSet();
                dateSet.EntryDate = startDate;
                dateSet.NotificationDate = dateSet.EntryDate;
                while (!IsDateWorkingDate(dateSet.NotificationDate, countryCode))
                {
                    dateSet.NotificationDate = dateSet.NotificationDate.Subtract(new TimeSpan(1, 0, 0, 0));
                }
                string dateIdentifier = DataShaper.GetDefaultDateString(dateSet.EntryDate);
                if (!result.ContainsKey(dateIdentifier))
                {
                    result.Add(dateIdentifier, dateSet);
                }
                startDate = startDate.AddDays(daysRepeatInterval);
            }
            return result;
        }

        public bool IsDateWorkingDate(DateTime date, string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode))
            {
                throw new ArgumentNullException("Country Code may not be null or empty.");
            }
            bool isWeekend = IsDateWeekendDay(date);
            bool isPublicHoliday = IsDatePublicHoliday(date, countryCode);
            return !(isWeekend || isPublicHoliday);
        }

        public bool IsDateWeekendDay(DateTime date)
        {
            DayOfWeek dayOfWeek = date.DayOfWeek;
            if ((date.DayOfWeek == DayOfWeek.Saturday) ||
                (date.DayOfWeek == DayOfWeek.Sunday))
            {
                return true;
            }
            return false;
        }

        public bool IsDatePublicHoliday(DateTime date, string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode))
            {
                throw new ArgumentNullException("Country Code may not be null or empty.");
            }
            string dateIdentifier = DataShaper.GetDefaultDateString(date);
            PublicHoliday result = GetPublicHolidayByCountry(countryCode, dateIdentifier, false);
            return result != null;
        }

        public DateTime ComputeNotificationDate(DateTime entryDate, string countryCode)
        {
            DateTime result = entryDate;
            while (!IsDateWorkingDate(result, countryCode))
            {
                result = result.Subtract(new TimeSpan(1, 0, 0, 0));
            }
            return result;
        }

        public List<ScheduleEntry> GetFutureEntriesForSchedule(
            Guid scheduleId,
            Guid scheduleEntryIdToExclude,
            DateTime startDate)
        {
            List<ScheduleEntry> queryResult = (from me in DB.GetTable<ScheduleEntry>()
                                               where (me.ScheduleId == scheduleId) &&
                                               (me.EntryDate > startDate) &&
                                               (me.ScheduleEntryId != scheduleEntryIdToExclude)
                                               orderby me.EntryDate ascending
                                               select me).ToList();
            return queryResult;
        }

        public Nullable<DateTime> GetLastScheduleEntryDate(Guid scheduleId, bool throwExceptionOnNotFound)
        {
            Nullable<DateTime> result = null;
            Schedule schedule = GetSchedule(scheduleId, true);
            ScheduleEntry scheduleEntry = (from e in DB.GetTable<ScheduleEntry>()
                                           where e.ScheduleId == scheduleId
                                           orderby e.EntryDate descending
                                           select e).FirstOrDefault();
            if (throwExceptionOnNotFound && scheduleEntry == null)
            {
                throw new Exception(string.Format("No {0} records linked to {1} with {2} of {3}.",
                    typeof(ScheduleEntry).Name,
                    typeof(Schedule).Name,
                    EntityReader<Schedule>.GetPropertyName(p => p.ScheduleId, false),
                    scheduleId));
            }
            if (scheduleEntry != null)
            {
                result = scheduleEntry.EntryDate;
            }
            return result;
        }

        public int GetScheduleEntriesCount(Guid scheduleId)
        {
            return (from e in DB.GetTable<ScheduleEntry>()
                    where e.ScheduleId == scheduleId
                    select e).Count();
        }

        public ScheduleEntry GetLastScheduleEntry(Guid scheduleId)
        {
            ScheduleEntry result = (from e in DB.GetTable<ScheduleEntry>()
                                    where e.ScheduleId == scheduleId
                                    orderby e.EntryDate descending
                                    select e).FirstOrDefault();
            return result;
        }

        public Nullable<DateTime> GetFirstScheduleEntryDate(Guid scheduleId, bool throwExceptionOnNotFound)
        {
            Nullable<DateTime> result = null;
            Schedule schedule = GetSchedule(scheduleId, true);
            ScheduleEntry scheduleEntry = (from e in DB.GetTable<ScheduleEntry>()
                                           where e.ScheduleId == scheduleId
                                           orderby e.EntryDate ascending
                                           select e).FirstOrDefault();
            if (throwExceptionOnNotFound && scheduleEntry == null)
            {
                throw new Exception(string.Format("No {0} records linked to {1} with {2} of {3}.",
                    typeof(ScheduleEntry).Name,
                    typeof(Schedule).Name,
                    EntityReader<Schedule>.GetPropertyName(p => p.ScheduleId, false),
                    scheduleId));
            }
            if (scheduleEntry != null)
            {
                result = scheduleEntry.EntryDate;
            }
            return result;
        }

        public ScheduleEntry GetFirstScheduleEntry(Guid scheduleId)
        {
            ScheduleEntry result = (from e in DB.GetTable<ScheduleEntry>()
                                    where e.ScheduleId == scheduleId
                                    orderby e.EntryDate ascending
                                    select e).FirstOrDefault();
            return result;
        }

        public ScheduleEntry GetFirstUpcomingScheduleEntry(Guid scheduleId, DateTime startDate)
        {
            ScheduleEntry result = (from e in DB.GetTable<ScheduleEntry>()
                                    where e.ScheduleId == scheduleId &&
                                    e.EntryDate.Date >= startDate.Date
                                    orderby e.EntryDate ascending
                                    select e).FirstOrDefault();
            return result;
        }

        public ScheduleEntry ShiftScheduleEntry(
            Guid scheduleEntryId,
            DateTime newEntryDate,
            string countryCode,
            int extraDays)
        {
            ScheduleEntry result = null;
            using (TransactionScope t = new TransactionScope())
            {
                ScheduleEntry original = GetScheduleEntry(scheduleEntryId, true);
                Schedule schedule = GetSchedule(original.ScheduleId, true);

                DateTime originalEntryDate = original.EntryDate.Date;
                Nullable<DateTime> lastEntryDate = GetLastScheduleEntryDate(original.ScheduleId, true);
                int numberOfDaysToMove = newEntryDate.Subtract(originalEntryDate).Days;

                //Delete the old entries.
                List<ScheduleEntry> entriesToDelete = new List<ScheduleEntry>() { original };
                if (newEntryDate >= originalEntryDate) //Entry date is being moved to a future time. So we need to delete all entries from the original delivery date to the last delivery date. Then recreate entries from the new delivery date to the last delivery date.
                {
                    entriesToDelete.AddRange(GetFutureEntriesForSchedule(
                        original.ScheduleId,
                        original.ScheduleEntryId,
                        originalEntryDate));
                }
                else //Entry date is being moved to a closer time. So we need to delete all entries from the new entry date to the last delivery date.
                {
                    entriesToDelete.AddRange(GetFutureEntriesForSchedule(
                        original.ScheduleId,
                        original.ScheduleEntryId,
                        newEntryDate));
                }
                DB.GetTable<ScheduleEntry>().DeleteAllOnSubmit(entriesToDelete);
                DB.SubmitChanges();

                DateTime startDate = newEntryDate;
                DateTime endDate = lastEntryDate.Value.Date.AddDays(numberOfDaysToMove + extraDays);
                ///Creating the new schedule entries.
                Dictionary<string, EntryDateSet> dates = GenerateScheduleDates(
                    startDate,
                    endDate,
                    countryCode,
                    schedule.DaysRepeatInterval);
                List<ScheduleEntry> entries = new List<ScheduleEntry>();
                dates.Values.ToList().ForEach(p => entries.Add(new ScheduleEntry()
                {
                    ScheduleEntryId = Guid.NewGuid(),
                    ScheduleId = schedule.ScheduleId,
                    EntryDate = p.EntryDate,
                    EntryDateFormatted = DataShaper.GetDefaultDateString(p.EntryDate),
                    EntryDateDayOfWeek = p.EntryDate.DayOfWeek.ToString(),
                    NotificationDate = p.NotificationDate,
                    NotificationDateFormatted = DataShaper.GetDefaultDateString(p.NotificationDate),
                    NotificationDateDayOfWeek = p.NotificationDate.DayOfWeek.ToString(),
                    SMSNotificationSent = false,
                    DateCreated = DateTime.Now
                }));
                result = entries.FirstOrDefault();
                DB.GetTable<ScheduleEntry>().InsertAllOnSubmit(entries);
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        #endregion //Schedule Generation

        #endregion //Schedule
    }
}