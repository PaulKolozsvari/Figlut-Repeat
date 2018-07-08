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

    public class OrganizationProfileModel
    {
        #region Properties

        public Guid OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationIdentifier { get; set; }

        public string OrganizationEmailAddress { get; set; }

        public string OrganizationAddress { get; set; }

        public long SmsCreditsBalance { get; set; }

        public bool AllowSmsCreditsDebt { get; set; }

        public Nullable<Guid> OrganizationSubscriptionTypeId { get; set; }

        public bool OrganizationSubscriptionEnabled { get;set;}

        public int BillingDayOfTheMonth { get; set; }

        public bool AutomaticallySendDailyScheduleEntriesSms { get; set; }

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

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.OrganizationName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationProfileModel>.GetPropertyName(p => p.OrganizationName, true));
            }
            else if (string.IsNullOrEmpty(this.OrganizationIdentifier))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationProfileModel>.GetPropertyName(p => p.OrganizationIdentifier, true));
            }
            else if (this.OrganizationIdentifier.Length > 20)
            {
                errorMessage = string.Format("{0} must be shorter than 20 characters.", EntityReader<RegisterModel>.GetPropertyName(p => p.OrganizationIdentifier, true));
            }
            else if (this.OrganizationIdentifier.Contains(' '))
            {
                errorMessage = string.Format("{0} must not contain spaces.", EntityReader<OrganizationProfileModel>.GetPropertyName(p => p.OrganizationIdentifier, true));
            }
            else if (string.IsNullOrEmpty(this.OrganizationEmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationProfileModel>.GetPropertyName(p => p.OrganizationEmailAddress, true));
            }
            else if (!DataShaper.IsValidEmail(this.OrganizationEmailAddress))
            {
                errorMessage = string.Format("{0} is not a  valid email address.", EntityReader<OrganizationProfileModel>.GetPropertyName(p => p.OrganizationEmailAddress, true));
            }
            else if (string.IsNullOrEmpty(this.OrganizationAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationProfileModel>.GetPropertyName(p => p.OrganizationAddress, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromOrganization(Organization organization)
        {
            this.OrganizationId = organization.OrganizationId;
            this.OrganizationName = organization.Name;
            this.OrganizationIdentifier = organization.Identifier;
            this.OrganizationEmailAddress = organization.EmailAddress;
            this.OrganizationAddress = organization.Address;
            this.SmsCreditsBalance = organization.SmsCreditsBalance;
            this.AllowSmsCreditsDebt = organization.AllowSmsCreditsDebt;
            this.OrganizationSubscriptionTypeId = organization.OrganizationSubscriptionTypeId;
            this.OrganizationSubscriptionEnabled = organization.OrganizationSubscriptionEnabled;
            this.BillingDayOfTheMonth = organization.BillingDayOfTheMonth;
            this.AutomaticallySendDailyScheduleEntriesSms = organization.AutomaticallySendDailyScheduleEntriesSms;
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
            organization.Name = this.OrganizationName;
            organization.Identifier = this.OrganizationIdentifier;
            organization.EmailAddress = this.OrganizationEmailAddress;
            organization.Address = this.OrganizationAddress;
            organization.SmsCreditsBalance = this.SmsCreditsBalance;
            organization.AllowSmsCreditsDebt = this.AllowSmsCreditsDebt;
            organization.OrganizationSubscriptionTypeId = this.OrganizationSubscriptionTypeId;
            organization.OrganizationSubscriptionEnabled = this.OrganizationSubscriptionEnabled;
            organization.BillingDayOfTheMonth = this.BillingDayOfTheMonth;
            organization.AutomaticallySendDailyScheduleEntriesSms = this.AutomaticallySendDailyScheduleEntriesSms;
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