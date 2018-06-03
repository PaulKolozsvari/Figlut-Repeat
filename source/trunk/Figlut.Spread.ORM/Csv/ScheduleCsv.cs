namespace Figlut.Spread.ORM.Csv
{
    #region Using Directives

    using Figlut.Spread.ORM.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class ScheduleCsv
    {
        #region Properties

        #region Schedule Properties

        public Guid ScheduleId { get; set; }

        public Guid SubscriptionId { get; set; }

        public string NotificationMessage { get; set; }

        public TimeSpan EntriesTime { get; set; }

        public string ScheduleName { get; set; }

        public Nullable<double> Quantity { get; set; }

        public string UnitOfMeasure { get; set; }

        public int DaysRepeatInterval { get; set; }

        public string Notes { get; set; }

        public bool CreateScheduleEntries { get; set; }

        public bool ExcludeNonWorkingDays { get; set; }

        public bool ExcludePublicHolidays { get; set; }

        public bool IsMondayWorkDay { get; set; }

        public bool IsTuesdayWorkDay { get; set; }

        public bool IsWednesdayWorkDay { get; set; }

        public bool IsThursdayWorkDay { get; set; }

        public bool IsFridayWorkDay { get; set; }

        public bool IsSaturdayWorkDay { get; set; }

        public bool IsSundayWorkDay { get; set; }

        public DateTime DateCreated { get; set; }

        public Nullable<DateTime> StartDate { get; set; }

        public string StartDateFormatted { get; set; }

        public Nullable<DateTime> EndDate { get; set; }

        public string EndDateFormatted { get; set; }

        public int EntryCount { get; set; }

        #endregion //Schedule Properties

        #region Subscription Properties

        public Guid OrganizationId { get; set; }

        public Guid SubscriberId { get; set; }

        public bool SubscriptionEnabled { get; set; }

        public string CustomerFullName { get; set; }

        public string CustomerIdentifier { get; set; }

        public string CustomerPhysicalAddress { get; set; }

        public string CustomerNotes { get; set; }

        public DateTime SubscriptionDateCreated { get; set; }

        #endregion //Subscription Properties

        #region Subscriber Properties

        public string CellPhoneNumber { get; set; }

        public string SubscriberName { get; set; }

        public bool SubscriberEnabled { get; set; }

        #endregion //Subscriber Properties

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromScheduleView(ScheduleView view)
        {
            this.ScheduleId = view.ScheduleId;
            this.SubscriptionId = view.SubscriptionId;
            this.NotificationMessage = view.NotificationMessage;
            this.EntriesTime = view.EntriesTime;
            this.ScheduleName = view.ScheduleName;
            this.Quantity = view.Quantity;
            this.UnitOfMeasure = view.UnitOfMeasure;
            this.DaysRepeatInterval = view.DaysRepeatInterval;
            this.Notes = view.Notes;
            this.CreateScheduleEntries = view.CreateScheduleEntries;
            this.ExcludeNonWorkingDays = view.ExcludeNonWorkingDays;
            this.ExcludePublicHolidays = view.ExcludePublicHolidays;
            this.IsMondayWorkDay = view.IsMondayWorkDay;
            this.IsTuesdayWorkDay = view.IsTuesdayWorkDay;
            this.IsWednesdayWorkDay = view.IsWednesdayWorkDay;
            this.IsThursdayWorkDay = view.IsThursdayWorkDay;
            this.IsFridayWorkDay = view.IsFridayWorkDay;
            this.IsSaturdayWorkDay = view.IsSaturdayWorkDay;
            this.IsSundayWorkDay = view.IsSundayWorkDay;
            this.DateCreated = view.DateCreated;
            this.StartDate = view.StartDate;
            this.EndDate = view.EndDate;
            this.EntryCount = view.EntryCount;

            this.OrganizationId = view.OrganizationId;
            this.SubscriberId = view.SubscriberId;
            this.SubscriptionEnabled = view.SubscriptionEnabled;
            this.CustomerFullName = view.CustomerFullName;
            this.CustomerIdentifier = view.CustomerIdentifier;
            this.CustomerPhysicalAddress = view.CustomerPhysicalAddress;
            this.CustomerNotes = view.CustomerNotes;
            this.SubscriptionDateCreated = view.SubscriptionDateCreated;

            this.CellPhoneNumber = view.CellPhoneNumber;
            this.SubscriberName = view.SubscriberName;
            this.SubscriberEnabled = view.SubscriberEnabled;
        }

        public void CopyPropertiesToSchedule(ScheduleView view)
        {
            view.ScheduleId = this.ScheduleId;
            view.SubscriptionId = this.SubscriptionId;
            view.NotificationMessage = this.NotificationMessage;
            view.EntriesTime = this.EntriesTime;
            view.ScheduleName = this.ScheduleName;
            view.Quantity = this.Quantity;
            view.UnitOfMeasure = this.UnitOfMeasure;
            view.DaysRepeatInterval = this.DaysRepeatInterval;
            view.Notes = this.Notes;
            view.CreateScheduleEntries = this.CreateScheduleEntries;
            view.ExcludeNonWorkingDays = this.ExcludeNonWorkingDays;
            view.ExcludePublicHolidays = this.ExcludePublicHolidays;
            view.IsMondayWorkDay = this.IsMondayWorkDay;
            view.IsTuesdayWorkDay = this.IsTuesdayWorkDay;
            view.IsWednesdayWorkDay = this.IsWednesdayWorkDay;
            view.IsThursdayWorkDay = this.IsThursdayWorkDay;
            view.IsFridayWorkDay = this.IsFridayWorkDay;
            view.IsSaturdayWorkDay = this.IsSaturdayWorkDay;
            view.IsSundayWorkDay = this.IsSundayWorkDay;
            view.DateCreated = this.DateCreated;
            view.StartDate = this.StartDate;
            view.EndDate = this.EndDate;
            view.EntryCount = this.EntryCount;

            view.OrganizationId = this.OrganizationId;
            view.SubscriberId = this.SubscriberId;
            view.SubscriptionEnabled = this.SubscriptionEnabled;
            view.CustomerFullName = this.CustomerFullName;
            view.CustomerIdentifier = this.CustomerIdentifier;
            view.CustomerPhysicalAddress = this.CustomerPhysicalAddress;
            view.CustomerNotes = this.CustomerNotes;
            view.SubscriptionDateCreated = this.SubscriptionDateCreated;

            view.CellPhoneNumber = this.CellPhoneNumber;
            view.SubscriberName = this.SubscriberName;
            view.SubscriberEnabled = this.SubscriberEnabled;
        }

        #endregion //Methods
    }
}
