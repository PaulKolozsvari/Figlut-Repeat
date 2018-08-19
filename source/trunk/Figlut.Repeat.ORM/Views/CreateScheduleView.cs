namespace Figlut.Repeat.ORM.Views
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Figlut.Server.Toolkit.Data;

    #endregion //Using Directives

    public class CreateScheduleView
    {
        #region Properties

        #region Subscription Properties

        public Guid SubscriptionId { get; set; }

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

        public DateTime SubscriberDateCreated { get; set; }

        #endregion //Subscriber Properties

        #region Organization Properties

        public string OrganizationName { get; set; }

        public string OrganizationIdentifier { get; set; }

        public string OrganizationPrimaryContactEmailAddress { get; set; }

        public string OrganizationAddress { get; set; }

        public long OrganizationSmsCreditsBalance { get; set; }

        public bool OrganizationAllowSmsCreditsDebt { get; set; }

        public Nullable<Guid> OrganizationSubscriptionTypeId { get; set; }

        public bool OrganizationSubscriptionEnabled { get; set; }

        public int OrganizationBillingDayOfTheMonth { get; set; }

        public bool IsMondayWorkDay { get; set; }

        public bool IsTuesdayWorkDay { get; set; }

        public bool IsWednesdayWorkDay { get; set; }

        public bool IsThursdayWorkDay { get; set; }

        public bool IsFridayWorkDay { get; set; }

        public bool IsSaturdayWorkDay { get; set; }

        public bool IsSundayWorkDay { get; set; }

        public Nullable<Guid> AccountManagerUserId { get; set; }

        public DateTime OrganizationDateCreated { get; set; }

        #endregion //Organization Properties

        #region Schedule

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

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid CountryId { get; set; }

        #endregion //Schedule

        #endregion Properties

        #region Methods

        public Schedule ToSchedule()
        {
            return new Schedule()
            {
                ScheduleId = Guid.NewGuid(),
                SubscriptionId = this.SubscriptionId,
                NotificationMessage = this.NotificationMessage,
                EntriesTime = this.EntriesTime,
                ScheduleName = this.ScheduleName,
                Quantity = this.Quantity,
                UnitOfMeasure = this.UnitOfMeasure,
                DaysRepeatInterval = this.DaysRepeatInterval,
                Notes = this.Notes,
                CreateScheduleEntries = this.CreateScheduleEntries,
                ExcludeNonWorkingDays = this.ExcludeNonWorkingDays,
                ExcludePublicHolidays = this.ExcludePublicHolidays,
                IsMondayWorkDay = this.IsMondayWorkDay,
                IsTuesdayWorkDay = this.IsTuesdayWorkDay,
                IsWednesdayWorkDay = this.IsWednesdayWorkDay,
                IsThursdayWorkDay = this.IsThursdayWorkDay,
                IsFridayWorkDay = this.IsFridayWorkDay,
                IsSaturdayWorkDay = this.IsSaturdayWorkDay,
                IsSundayWorkDay = this.IsSundayWorkDay,
                DateCreated = DateTime.Now
            };
        }

        #endregion //Methods
    }
}