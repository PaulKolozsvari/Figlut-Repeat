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

        public CreateRepeatScheduleView GetCreateRepeatScheduleModelView(Guid subscriptionId, bool throwExceptionOnNotFound)
        {
            CreateRepeatScheduleView result = (from subscription in DB.GetTable<Subscription>()
                                               join subscriber in DB.GetTable<Subscriber>() on subscription.SubscriberId equals subscriber.SubscriberId into setSubsciber
                                               from subscriberView in setSubsciber.DefaultIfEmpty()
                                               join organization in DB.GetTable<Organization>() on subscription.OrganizationId equals organization.OrganizationId into setOrganization
                                               from organizationView in setOrganization.DefaultIfEmpty()
                                               where subscription.SubscriptionId == subscriptionId
                                               select new CreateRepeatScheduleView()
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
                                                   OrganizationDateCreated = organizationView.DateCreated
                                               }).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(CreateRepeatScheduleView).Name,
                    EntityReader<CreateRepeatScheduleView>.GetPropertyName(p => p.SubscriptionId, false),
                    subscriptionId.ToString()));
            }
            return result;
        }

        public RepeatScheduleView GetRepeatScheduleView(Guid repeatScheduleId, bool throwExceptionOnNotFound)
        {
            RepeatScheduleView result = (from repeatSchedule in DB.GetTable<RepeatSchedule>()
                                         join subscription in DB.GetTable<Subscription>() on repeatSchedule.SubscriptionId equals subscription.SubscriptionId into setSubscription
                                         from subscriptionView in setSubscription.DefaultIfEmpty()
                                         join subscriber in DB.GetTable<Subscriber>() on subscriptionView.SubscriberId equals subscriber.SubscriberId into setSubscriber
                                         from subscriberView in setSubscriber.DefaultIfEmpty()
                                         where repeatSchedule.RepeatScheduleId == repeatScheduleId
                                         select new RepeatScheduleView()
                                         {
                                             //Repeat Schedule
                                             RepeatScheduleId = repeatSchedule.RepeatScheduleId,
                                             SubscriptionId = repeatSchedule.SubscriptionId,
                                             NotificationMessage = repeatSchedule.NotificationMessage,
                                             ScheduleName = repeatSchedule.ScheduleName,
                                             Quantity = repeatSchedule.Quantity,
                                             UnitOfMeasure = repeatSchedule.UnitOfMeasure,
                                             DaysRepeatInterval = repeatSchedule.DaysRepeatInterval,
                                             Notes = repeatSchedule.Notes,
                                             DateCreated = repeatSchedule.DateCreated,
                                             StartDate = GetFirstRepeatScheduleEntryRepeatDate(repeatSchedule.RepeatScheduleId, false),
                                             EndDate = GetLastRepeatScheduleEntryRepeatDate(repeatSchedule.RepeatScheduleId, false),
                                             EntryCount = GetRepeatScheduleEntriesCount(repeatSchedule.RepeatScheduleId),

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
                          repeatSchedule.NotificationMessage.ToLower().Contains(searchFilterLower) ||
                          repeatSchedule.DaysRepeatInterval.ToString().Contains(searchFilterLower) ||
                          repeatSchedule.UnitOfMeasure.ToLower().Contains(searchFilterLower) ||
                          repeatSchedule.Notes.ToLower().Contains(searchFilterLower))
                          select new RepeatScheduleView()
                          {
                              //Repeat Schedule
                              RepeatScheduleId = repeatSchedule.RepeatScheduleId,
                              SubscriptionId = repeatSchedule.SubscriptionId,
                              NotificationMessage = repeatSchedule.NotificationMessage,
                              ScheduleName = repeatSchedule.ScheduleName,
                              Quantity = repeatSchedule.Quantity,
                              UnitOfMeasure = repeatSchedule.UnitOfMeasure,
                              DaysRepeatInterval = repeatSchedule.DaysRepeatInterval,
                              Notes = repeatSchedule.Notes,
                              DateCreated = repeatSchedule.DateCreated,
                              StartDate = GetFirstRepeatScheduleEntryRepeatDate(repeatSchedule.RepeatScheduleId, false),
                              EndDate = GetLastRepeatScheduleEntryRepeatDate(repeatSchedule.RepeatScheduleId, false),
                              EntryCount = GetRepeatScheduleEntriesCount(repeatSchedule.RepeatScheduleId),

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
                          where repeatSchedule.ScheduleName.ToLower().Contains(searchFilterLower) ||
                          repeatSchedule.NotificationMessage.ToLower().Contains(searchFilterLower) ||
                          repeatSchedule.DaysRepeatInterval.ToString().Contains(searchFilterLower) ||
                          repeatSchedule.UnitOfMeasure.ToLower().Contains(searchFilterLower) ||
                          repeatSchedule.Notes.ToLower().Contains(searchFilterLower)
                          select new RepeatScheduleView()
                          {
                              //Repeat Schedule
                              RepeatScheduleId = repeatSchedule.RepeatScheduleId,
                              SubscriptionId = repeatSchedule.SubscriptionId,
                              NotificationMessage = repeatSchedule.NotificationMessage,
                              ScheduleName = repeatSchedule.ScheduleName,
                              Quantity = repeatSchedule.Quantity,
                              UnitOfMeasure = repeatSchedule.UnitOfMeasure,
                              DaysRepeatInterval = repeatSchedule.DaysRepeatInterval,
                              Notes = repeatSchedule.Notes,
                              DateCreated = repeatSchedule.DateCreated,
                              StartDate = GetFirstRepeatScheduleEntryRepeatDate(repeatSchedule.RepeatScheduleId, false),
                              EndDate = GetLastRepeatScheduleEntryRepeatDate(repeatSchedule.RepeatScheduleId, false),
                              EntryCount = GetRepeatScheduleEntriesCount(repeatSchedule.RepeatScheduleId),

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
            foreach (RepeatScheduleView r in result)
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
                foreach (RepeatSchedule r in repeatSchedules) //Delete all the children first.
                {
                    List<RepeatScheduleEntry> repeatScheduleEntries = GetRepeatScheduleEntriesByFilter(null, r.RepeatScheduleId);
                    DB.GetTable<RepeatScheduleEntry>().DeleteAllOnSubmit(repeatScheduleEntries);
                    DB.SubmitChanges();
                }
                DB.GetTable<RepeatSchedule>().DeleteAllOnSubmit(repeatSchedules);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        public void DeleteRepeatScheduleAndEntries(Guid repeatScheduleId, bool throwExceptionOnNotFound)
        {
            using (TransactionScope t = new TransactionScope())
            {
                RepeatSchedule repeatSchedule = GetRepeatSchedule(repeatScheduleId, throwExceptionOnNotFound);
                List<RepeatScheduleEntry> repeatScheduleEntries = GetRepeatScheduleEntriesByFilter(null, repeatSchedule.RepeatScheduleId);
                DB.GetTable<RepeatScheduleEntry>().DeleteAllOnSubmit(repeatScheduleEntries); //Delete the children first.
                DB.SubmitChanges();
                DB.GetTable<RepeatSchedule>().DeleteOnSubmit(repeatSchedule);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #region Repeat Schedule Generation

        public RepeatSchedule CreateRepeatSchedule(
            CreateRepeatScheduleView view)
        {
            RepeatSchedule result = null;
            using (TransactionScope t = new TransactionScope())
            {
                Country country = GetCountry(view.CountryId, true);
                Dictionary<string, RepeatDateSet> dates = GeneraterepeatScheduleDates(view.StartDate, view.EndDate, country.CountryCode, view.DaysRepeatInterval);
                result = view.ToRepeatSchedule();
                DB.GetTable<RepeatSchedule>().InsertOnSubmit(result);
                List<RepeatScheduleEntry> entries = new List<RepeatScheduleEntry>();
                dates.Values.ToList().ForEach(p => entries.Add(new RepeatScheduleEntry()
                {
                    RepeatScheduleEntryId = Guid.NewGuid(),
                    RepeatScheduleId = result.RepeatScheduleId,
                    RepeatDate = p.RepeatDate,
                    RepeatDateFormatted = DataShaper.GetDefaultDateString(p.RepeatDate),
                    RepeatDateDayOfWeek = p.RepeatDate.DayOfWeek.ToString(),
                    NotificationDate = p.NotificationDate,
                    NotificationDateFormatted = DataShaper.GetDefaultDateString(p.NotificationDate),
                    NotificationDateDayOfWeek = p.NotificationDate.DayOfWeek.ToString(),
                    SMSNotificationSent = false,
                    SMSMessageId = null,
                    SMSDateSent = null,
                    SmsSentLogId = null,
                    DateCreated = DateTime.Now
                }));
                DB.GetTable<RepeatScheduleEntry>().InsertAllOnSubmit(entries);
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        public Dictionary<string, RepeatDateSet> GeneraterepeatScheduleDates(
            DateTime startDate,
            DateTime endDate,
            string countryCode,
            int daysRepeatInterval)
        {
            if (startDate > endDate)
            {
                throw new ArgumentOutOfRangeException("Start Date may not be greater than End Date when generating Repeat Schedule Dates.");
            }
            Dictionary<string, RepeatDateSet> result = new Dictionary<string, RepeatDateSet>();
            while (startDate.Date <= endDate.Date)
            {
                RepeatDateSet dateSet = new RepeatDateSet();
                dateSet.RepeatDate = startDate;
                dateSet.NotificationDate = dateSet.RepeatDate;
                while (!IsDateWorkingDate(dateSet.NotificationDate, countryCode))
                {
                    dateSet.NotificationDate = dateSet.NotificationDate.Subtract(new TimeSpan(1, 0, 0, 0));
                }
                string dateIdentifier = DataShaper.GetDefaultDateString(dateSet.RepeatDate);
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

        public DateTime ComputeNotificationDate(DateTime repeatDate, string countryCode)
        {
            DateTime result = repeatDate;
            while (!IsDateWorkingDate(result, countryCode))
            {
                result = result.Subtract(new TimeSpan(1, 0, 0, 0));
            }
            return result;
        }

        public List<RepeatScheduleEntry> GetFutureEntriesForRepeatSchedule(
            Guid repeatScheduleId,
            Guid repeatScheduleEntryIdToExclude,
            DateTime startRepeatDate)
        {
            List<RepeatScheduleEntry> queryResult = (from me in DB.GetTable<RepeatScheduleEntry>()
                                                     where (me.RepeatScheduleId == repeatScheduleId) &&
                                                     (me.RepeatDate > startRepeatDate) &&
                                                     (me.RepeatScheduleEntryId != repeatScheduleEntryIdToExclude)
                                                     orderby me.RepeatDate ascending
                                                     select me).ToList();
            return queryResult;
        }

        public Nullable<DateTime> GetLastRepeatScheduleEntryRepeatDate(Guid repeatScheduleId, bool throwExceptionOnNotFound)
        {
            Nullable<DateTime> result = null;
            RepeatSchedule repeatSchedule = GetRepeatSchedule(repeatScheduleId, true);
            RepeatScheduleEntry repeatScheduleEntry = (from e in DB.GetTable<RepeatScheduleEntry>()
                                                       where e.RepeatScheduleId == repeatScheduleId
                                                       orderby e.RepeatDate descending
                                                       select e).FirstOrDefault();
            if (throwExceptionOnNotFound && repeatScheduleEntry == null)
            {
                throw new Exception(string.Format("No {0} records linked to {1} with {2} of {3}.",
                    typeof(RepeatScheduleEntry).Name,
                    typeof(RepeatSchedule).Name,
                    EntityReader<RepeatSchedule>.GetPropertyName(p => p.RepeatScheduleId, false),
                    repeatScheduleId));
            }
            if (repeatScheduleEntry != null)
            {
                result = repeatScheduleEntry.RepeatDate;
            }
            return result;
        }

        public int GetRepeatScheduleEntriesCount(Guid repeatScheduleId)
        {
            return (from e in DB.GetTable<RepeatScheduleEntry>()
                    where e.RepeatScheduleId == repeatScheduleId
                    select e).Count();
        }

        public RepeatScheduleEntry GetLastRepeatScheduleEntry(Guid repeatScheduleId)
        {
            RepeatScheduleEntry result = (from e in DB.GetTable<RepeatScheduleEntry>()
                                          where e.RepeatScheduleId == repeatScheduleId
                                          orderby e.RepeatDate descending
                                          select e).FirstOrDefault();
            return result;
        }

        public Nullable<DateTime> GetFirstRepeatScheduleEntryRepeatDate(Guid repeatScheduleId, bool throwExceptionOnNotFound)
        {
            Nullable<DateTime> result = null;
            RepeatSchedule repeatSchedule = GetRepeatSchedule(repeatScheduleId, true);
            RepeatScheduleEntry repeatScheduleEntry = (from e in DB.GetTable<RepeatScheduleEntry>()
                                                       where e.RepeatScheduleId == repeatScheduleId
                                                       orderby e.RepeatDate ascending
                                                       select e).FirstOrDefault();
            if (throwExceptionOnNotFound && repeatScheduleEntry == null)
            {
                throw new Exception(string.Format("No {0} records linked to {1} with {2} of {3}.",
                    typeof(RepeatScheduleEntry).Name,
                    typeof(RepeatSchedule).Name,
                    EntityReader<RepeatSchedule>.GetPropertyName(p => p.RepeatScheduleId, false),
                    repeatScheduleId));
            }
            if (repeatScheduleEntry != null)
            {
                result = repeatScheduleEntry.RepeatDate;
            }
            return result;
        }

        public RepeatScheduleEntry GetFirstRepeatScheduleEntry(Guid repeatScheduleId)
        {
            RepeatScheduleEntry result = (from e in DB.GetTable<RepeatScheduleEntry>()
                                          where e.RepeatScheduleId == repeatScheduleId
                                          orderby e.RepeatDate ascending
                                          select e).FirstOrDefault();
            return result;
        }

        public RepeatScheduleEntry GetFirstUpcomingRepeatScheduleEntry(Guid repeatScheduleId, DateTime startDate)
        {
            RepeatScheduleEntry result = (from e in DB.GetTable<RepeatScheduleEntry>()
                                          where e.RepeatScheduleId == repeatScheduleId &&
                                          e.RepeatDate.Date >= startDate.Date
                                          orderby e.RepeatDate ascending
                                          select e).FirstOrDefault();
            return result;
        }

        public RepeatScheduleEntry ShiftRepeatScheduleEntry(
            Guid repeatScheduleEntryId,
            DateTime newRepeatDate,
            string countryCode,
            int extraDays)
        {
            RepeatScheduleEntry result = null;
            using (TransactionScope t = new TransactionScope())
            {
                RepeatScheduleEntry original = GetRepeatScheduleEntry(repeatScheduleEntryId, true);
                RepeatSchedule repeatSchedule = GetRepeatSchedule(original.RepeatScheduleId, true);

                DateTime originalRepeatDate = original.RepeatDate.Date;
                Nullable<DateTime> lastRepeatDate = GetLastRepeatScheduleEntryRepeatDate(original.RepeatScheduleId, true);
                int numberOfDaysToMove = newRepeatDate.Subtract(originalRepeatDate).Days;

                //Delete the old entries.
                List<RepeatScheduleEntry> entriesToDelete = new List<RepeatScheduleEntry>() { original };
                if (newRepeatDate >= originalRepeatDate) //Repeat date is being moved to a future time. So we need to delete all entries from the original delivery date to the last delivery date. Then recreate entries from the new delivery date to the last delivery date.
                {
                    entriesToDelete.AddRange(GetFutureEntriesForRepeatSchedule(
                        original.RepeatScheduleId,
                        original.RepeatScheduleEntryId,
                        originalRepeatDate));
                }
                else //Repeat date is being moved to a closer time. So we need to delete all entries from the new repeat date to the last delivery date.
                {
                    entriesToDelete.AddRange(GetFutureEntriesForRepeatSchedule(
                        original.RepeatScheduleId,
                        original.RepeatScheduleEntryId,
                        newRepeatDate));
                }
                DB.GetTable<RepeatScheduleEntry>().DeleteAllOnSubmit(entriesToDelete);
                DB.SubmitChanges();

                DateTime startDate = newRepeatDate;
                DateTime endDate = lastRepeatDate.Value.Date.AddDays(numberOfDaysToMove + extraDays);
                ///Creating the new schedule entries.
                Dictionary<string, RepeatDateSet> dates = GeneraterepeatScheduleDates(
                    startDate,
                    endDate,
                    countryCode,
                    repeatSchedule.DaysRepeatInterval);
                List<RepeatScheduleEntry> entries = new List<RepeatScheduleEntry>();
                dates.Values.ToList().ForEach(p => entries.Add(new RepeatScheduleEntry()
                {
                    RepeatScheduleEntryId = Guid.NewGuid(),
                    RepeatScheduleId = repeatSchedule.RepeatScheduleId,
                    RepeatDate = p.RepeatDate,
                    RepeatDateFormatted = DataShaper.GetDefaultDateString(p.RepeatDate),
                    RepeatDateDayOfWeek = p.RepeatDate.DayOfWeek.ToString(),
                    NotificationDate = p.NotificationDate,
                    NotificationDateFormatted = DataShaper.GetDefaultDateString(p.NotificationDate),
                    NotificationDateDayOfWeek = p.NotificationDate.DayOfWeek.ToString(),
                    SMSNotificationSent = false,
                    DateCreated = DateTime.Now
                }));
                result = entries.FirstOrDefault();
                DB.GetTable<RepeatScheduleEntry>().InsertAllOnSubmit(entries);
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        #endregion //Repeat Schedule Generation

        #endregion //Repeat Schedule
    }
}