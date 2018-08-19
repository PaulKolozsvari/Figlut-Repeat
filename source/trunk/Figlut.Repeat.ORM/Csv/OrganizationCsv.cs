namespace Figlut.Repeat.ORM.Csv
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class OrganizationCsv
    {
        #region Properties

        public Guid OrganizationId { get; set; }

        public string Name { get; set; }

        public string Identifier { get; set; }

        public string PrimaryContactEmailAddress { get; set; }

        public string PrimaryContactName { get; set; }

        public string PrimaryContactPhoneNumber { get; set; }

        public bool EnableEmailNotifications { get; set; }

        public string Address { get; set; }

        public long SmsCreditsBalance { get; set; }

        public bool AllowSmsCreditsDebt { get; set; }

        public Nullable<Guid> OrganizationSubscriptionTypeId { get; set; }

        public bool OrganizationSubscriptionEnabled { get; set; }

        public int BillingDayOfTheMonth { get; set; }

        public decimal OutstandingBalance { get; set;}

        public bool AutomaticallySendDailyScheduleEntriesSms { get; set; }

        public bool EnableDailyScheduleEntriesEmailNotifications { get; set; }

        public bool SendDailyScheduleEntriesEmailNotificationOnZeroEntries { get; set; }

        public TimeSpan DailyScheduleEntriesEmailNotificationTime { get; set; }

        public bool IsMondayWorkDay { get; set; }

        public bool IsTuesdayWorkDay { get; set; }

        public bool IsWednesdayWorkDay { get; set; }

        public bool IsThursdayWorkDay { get; set; }

        public bool IsFridayWorkDay { get; set; }

        public bool IsSaturdayWorkDay { get; set; }

        public bool IsSundayWorkDay { get; set; }

        public Nullable<Guid> AccountManagerUserId { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromOrganization(Organization organization)
        {
            this.OrganizationId = organization.OrganizationId;
            this.Name = organization.Name;
            this.Identifier = organization.Identifier;
            this.PrimaryContactEmailAddress = organization.PrimaryContactEmailAddress;
            this.PrimaryContactName = organization.PrimaryContactName;
            this.PrimaryContactPhoneNumber = organization.PrimaryContactPhoneNumber;
            this.EnableEmailNotifications = organization.EnableEmailNotifications;
            this.Address = organization.Address;
            this.SmsCreditsBalance = organization.SmsCreditsBalance;
            this.AllowSmsCreditsDebt = organization.AllowSmsCreditsDebt;
            this.OrganizationSubscriptionTypeId = organization.OrganizationSubscriptionTypeId;
            this.OrganizationSubscriptionEnabled = organization.OrganizationSubscriptionEnabled;
            this.BillingDayOfTheMonth = organization.BillingDayOfTheMonth;
            this.OutstandingBalance = organization.OutstandingBalance;
            this.AutomaticallySendDailyScheduleEntriesSms = organization.AutomaticallySendDailyScheduleEntriesSms;
            this.EnableDailyScheduleEntriesEmailNotifications = organization.EnableDailyScheduleEntriesEmailNotifications;
            this.SendDailyScheduleEntriesEmailNotificationOnZeroEntries = organization.SendDailyScheduleEntriesEmailNotificationOnZeroEntries;
            this.DailyScheduleEntriesEmailNotificationTime = organization.DailyScheduleEntriesEmailNotificationTime;
            this.IsMondayWorkDay = organization.IsMondayWorkDay;
            this.IsTuesdayWorkDay = organization.IsTuesdayWorkDay;
            this.IsWednesdayWorkDay = organization.IsWednesdayWorkDay;
            this.IsThursdayWorkDay = organization.IsThursdayWorkDay;
            this.IsFridayWorkDay = organization.IsFridayWorkDay;
            this.IsSaturdayWorkDay = organization.IsSaturdayWorkDay;
            this.IsSundayWorkDay = organization.IsSundayWorkDay;
            this.AccountManagerUserId = organization.AccountManagerUserId;
            this.DateCreated = organization.DateCreated;
        }

        public void CopyPropertiesToOrganization(Organization organization)
        {
            organization.OrganizationId = this.OrganizationId;
            organization.Name = this.Name;
            organization.Identifier = this.Identifier;
            organization.PrimaryContactEmailAddress = this.PrimaryContactEmailAddress;
            organization.PrimaryContactName = this.PrimaryContactName;
            organization.PrimaryContactPhoneNumber = this.PrimaryContactPhoneNumber;
            organization.EnableEmailNotifications = this.EnableEmailNotifications;
            organization.Address = this.Address;
            organization.SmsCreditsBalance = this.SmsCreditsBalance;
            organization.AllowSmsCreditsDebt = this.AllowSmsCreditsDebt;
            organization.OrganizationSubscriptionTypeId = this.OrganizationSubscriptionTypeId;
            organization.OrganizationSubscriptionEnabled = this.OrganizationSubscriptionEnabled;
            organization.BillingDayOfTheMonth = this.BillingDayOfTheMonth;
            organization.OutstandingBalance = this.OutstandingBalance;
            organization.AutomaticallySendDailyScheduleEntriesSms = this.AutomaticallySendDailyScheduleEntriesSms;
            organization.EnableDailyScheduleEntriesEmailNotifications = this.EnableDailyScheduleEntriesEmailNotifications;
            organization.SendDailyScheduleEntriesEmailNotificationOnZeroEntries = this.SendDailyScheduleEntriesEmailNotificationOnZeroEntries;
            organization.DailyScheduleEntriesEmailNotificationTime = this.DailyScheduleEntriesEmailNotificationTime;
            organization.IsMondayWorkDay = this.IsMondayWorkDay;
            organization.IsTuesdayWorkDay = this.IsTuesdayWorkDay;
            organization.IsWednesdayWorkDay = this.IsWednesdayWorkDay;
            organization.IsThursdayWorkDay = this.IsThursdayWorkDay;
            organization.IsFridayWorkDay = this.IsFridayWorkDay;
            organization.IsSaturdayWorkDay = this.IsSaturdayWorkDay;
            organization.IsSundayWorkDay = this.IsSundayWorkDay;
            organization.AccountManagerUserId = this.AccountManagerUserId;
            organization.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}