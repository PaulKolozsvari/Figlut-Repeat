namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Repeat.ORM;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class OrganizationModel
    {
        #region Properties

        public Guid OrganizationId { get; set; }

        public string Name { get; set; }

        public string Identifier { get; set; }

        public string EmailAddress { get; set; }

        public bool EnableEmailNotifications { get; set; }

        public string Address { get; set; }

        public long SmsCreditsBalance { get; set; }

        public bool AllowSmsCreditsDebt { get; set; }

        public Nullable<Guid> OrganizationSubscriptionTypeId { get; set; }

        public bool OrganizationSubscriptionEnabled { get; set; }

        public int BillingDayOfTheMonth { get; set; }

        public bool AutomaticallySendDailyScheduleEntriesSms { get; set; }

        public bool EnableDailyScheduleEntriesEmailNotifications { get; set; }

        public bool SendDailyScheduleEntriesEmailNotificationOnZeroEntries { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan DailyScheduleEntriesEmailNotificationTime { get; set; }

        public bool IsMondayWorkDay { get; set; }

        public bool IsTuesdayWorkDay { get; set; }

        public bool IsWednesdayWorkDay { get; set; }

        public bool IsThursdayWorkDay { get; set; }

        public bool IsFridayWorkDay { get; set; }

        public bool IsSaturdayWorkDay { get; set; }

        public bool IsSundayWorkDay { get; set; }

        public Nullable<Guid> AccountManagerUserId { get; set; }

        public string AccountManagerUserName { get; set; }

        public string AccountManagerEmailAddress { get; set; }

        public string AccountManagerCellPhoneNumber { get; set; }

        public DateTime DateCreated { get; set; }

        #region Settings

        public int OrganizationIdentifierMaxLength { get; set; }

        #endregion //Settings

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage, int organizationIdentifierMaxLength)
        {
            errorMessage = null;
            if (this.OrganizationId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationModel>.GetPropertyName(p => p.OrganizationId, true));
            }
            if (string.IsNullOrEmpty(this.Name))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationModel>.GetPropertyName(p => p.Name, true));
            }
            else if (string.IsNullOrEmpty(this.Identifier))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationModel>.GetPropertyName(p => p.Identifier, true));
            }
            else if (this.Identifier.Length > organizationIdentifierMaxLength)
            {
                errorMessage = string.Format("{0} must be {1} characters or less.", EntityReader<OrganizationModel>.GetPropertyName(p => p.Identifier, true), organizationIdentifierMaxLength);
            }
            else if (this.Identifier.Contains(' '))
            {
                errorMessage = string.Format("{0} must not contain spaces.", EntityReader<OrganizationModel>.GetPropertyName(p => p.Identifier, true));
            }
            else if (string.IsNullOrEmpty(this.EmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationModel>.GetPropertyName(p => p.EmailAddress, true));
            }
            else if (!DataShaper.IsValidEmail(this.EmailAddress))
            {
                errorMessage = string.Format("{0} is not a  valid email address.", EntityReader<OrganizationModel>.GetPropertyName(p => p.EmailAddress, true));
            }
            else if (string.IsNullOrEmpty(this.Address))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationModel>.GetPropertyName(p => p.Address, true));
            }
            else if (this.SmsCreditsBalance < 0 && !this.AllowSmsCreditsDebt)
            {
                errorMessage = string.Format("SMS Credits Balance may not be a negative number if SMS Credits Debt is not allowed.");
            }

            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromOrganization(Organization organization, string currencySymbol)
        {
            this.OrganizationId = organization.OrganizationId;
            this.Name = organization.Name;
            this.Identifier = organization.Identifier;
            this.EmailAddress = organization.EmailAddress;
            this.EnableEmailNotifications = organization.EnableEmailNotifications;
            this.Address = organization.Address;
            this.SmsCreditsBalance = organization.SmsCreditsBalance;
            this.AllowSmsCreditsDebt = organization.AllowSmsCreditsDebt;
            this.OrganizationSubscriptionTypeId = organization.OrganizationSubscriptionTypeId;
            this.OrganizationSubscriptionEnabled = organization.OrganizationSubscriptionEnabled;
            this.AutomaticallySendDailyScheduleEntriesSms = organization.AutomaticallySendDailyScheduleEntriesSms;
            this.EnableDailyScheduleEntriesEmailNotifications = organization.EnableDailyScheduleEntriesEmailNotifications;
            this.SendDailyScheduleEntriesEmailNotificationOnZeroEntries = organization.SendDailyScheduleEntriesEmailNotificationOnZeroEntries;
            this.DailyScheduleEntriesEmailNotificationTime = organization.DailyScheduleEntriesEmailNotificationTime;
            this.BillingDayOfTheMonth = organization.BillingDayOfTheMonth;
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
            organization.EmailAddress = this.EmailAddress;
            organization.EnableEmailNotifications = this.EnableEmailNotifications;
            organization.Address = this.Address;
            organization.SmsCreditsBalance = this.SmsCreditsBalance;
            organization.AllowSmsCreditsDebt = this.AllowSmsCreditsDebt;
            organization.OrganizationSubscriptionTypeId = this.OrganizationSubscriptionTypeId;
            organization.OrganizationSubscriptionEnabled = this.OrganizationSubscriptionEnabled;
            organization.BillingDayOfTheMonth = this.BillingDayOfTheMonth;
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