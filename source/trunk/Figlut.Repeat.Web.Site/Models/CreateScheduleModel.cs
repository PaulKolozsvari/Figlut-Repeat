namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Repeat.ORM.Views;
    using System.ComponentModel.DataAnnotations;

    #endregion //Using Directives

    public class CreateScheduleModel
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

        public string OrganizationEmailAddress { get; set; }

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

        public string NotificationMessageCreate { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan EntriesTime { get; set; }

        public string ScheduleName { get; set; }

        public Nullable<double> Quantity { get; set; }

        public string UnitOfMeasure { get; set; }

        public int DaysRepeatInterval { get; set; }

        public string Notes { get; set; }

        public bool CreateScheduleEntries { get; set; }

        public bool ExcludeNonWorkingDays { get; set; }

        public bool ExcludePublicHolidays { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDateCreate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDateCreate { get; set; }

        public Guid CountryId { get; set; }

        #endregion //Schedule

        #region Settings

        public int MaxSmsSendMessageLength { get; set; }

        public string SmsMessageTemplateIdCreate { get; set; }

        #endregion //Settings

        #endregion Properties

        #region Methods

        public bool IsValid(out string errorMessage, int maxSmsSendMessageLength)
        {
            errorMessage = null;
            //Subscription
            if (this.SubscriptionId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.SubscriptionId, true));
            }
            else if (this.OrganizationId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.OrganizationId, true));
            }
            else if (this.SubscriberId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.SubscriberId, true));
            }
            //Subscriber
            else if (string.IsNullOrEmpty(this.CellPhoneNumber))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.CellPhoneNumber, true));
            }
            //Organization
            else if (string.IsNullOrEmpty(this.OrganizationName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.OrganizationName, true));
            }
            else if (string.IsNullOrEmpty(this.OrganizationIdentifier))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.OrganizationIdentifier, true));
            }
            else if (string.IsNullOrEmpty(this.OrganizationEmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.OrganizationEmailAddress, true));
            }
            else if (string.IsNullOrEmpty(this.OrganizationAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.OrganizationAddress, true));
            }
            //Schedule
            else if (string.IsNullOrEmpty(this.NotificationMessage))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.NotificationMessage, true));
            }
            else if (this.NotificationMessage.Length > maxSmsSendMessageLength)
            {
                errorMessage = string.Format("{0} may not be greater than {1} characters.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.NotificationMessage, true), maxSmsSendMessageLength);
            }
            else if (string.IsNullOrEmpty(this.ScheduleName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.ScheduleName, true));
            }
            else if (this.DaysRepeatInterval <= 0)
            {
                errorMessage = string.Format("{0} may not be less than or equal to 0.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.DaysRepeatInterval, true));
            }
            else if (this.Quantity.HasValue && this.Quantity.Value < 0.0)
            {
                errorMessage = string.Format("{0} may not be less than or equal to 0.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.Quantity, true));
            }
            else if (this.StartDateCreate == new DateTime())
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.StartDateCreate, true));
            }
            else if (this.EndDateCreate == new DateTime())
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.StartDateCreate, true));
            }
            else if (this.StartDateCreate.Date > this.EndDateCreate)
            {
                errorMessage = string.Format("{0} may not be greater than the {1}.",
                    EntityReader<CreateScheduleModel>.GetPropertyName(p => p.StartDateCreate, true),
                    EntityReader<CreateScheduleModel>.GetPropertyName(p => p.EndDateCreate, true));
            }
            else if (this.CountryId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateScheduleModel>.GetPropertyName(p => p.CountryId, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromCreateScheduleView(CreateScheduleView view)
        {
            //Subscription
            this.SubscriptionId = view.SubscriptionId;
            this.OrganizationId = view.OrganizationId;
            this.SubscriberId = view.SubscriberId;
            this.SubscriptionEnabled = view.SubscriptionEnabled;
            this.CustomerFullName = view.CustomerFullName;
            this.CustomerIdentifier = view.CustomerIdentifier;
            this.CustomerPhysicalAddress = view.CustomerPhysicalAddress;
            this.CustomerNotes = view.CustomerPhysicalAddress;
            this.SubscriptionDateCreated = view.SubscriptionDateCreated;

            //Subscriber
            this.CellPhoneNumber = view.CellPhoneNumber;
            this.SubscriberName = view.SubscriberName;
            this.SubscriberEnabled = view.SubscriberEnabled;
            this.SubscriberDateCreated = view.SubscriberDateCreated;

            //Organization
            this.OrganizationName = view.OrganizationName;
            this.OrganizationIdentifier = view.OrganizationIdentifier;
            this.OrganizationEmailAddress = view.OrganizationPrimaryContactEmailAddress;
            this.OrganizationAddress = view.OrganizationAddress;
            this.OrganizationSmsCreditsBalance = view.OrganizationSmsCreditsBalance;
            this.OrganizationAllowSmsCreditsDebt = view.OrganizationAllowSmsCreditsDebt;
            this.OrganizationSubscriptionTypeId = view.OrganizationSubscriptionTypeId;
            this.OrganizationSubscriptionEnabled = view.OrganizationSubscriptionEnabled;
            this.OrganizationBillingDayOfTheMonth = view.OrganizationBillingDayOfTheMonth;
            this.IsMondayWorkDay = view.IsMondayWorkDay;
            this.IsTuesdayWorkDay = view.IsTuesdayWorkDay;
            this.IsWednesdayWorkDay = view.IsWednesdayWorkDay;
            this.IsThursdayWorkDay = view.IsThursdayWorkDay;
            this.IsFridayWorkDay = view.IsFridayWorkDay;
            this.IsSaturdayWorkDay = view.IsSaturdayWorkDay;
            this.IsSundayWorkDay = view.IsSundayWorkDay;
            this.AccountManagerUserId = view.AccountManagerUserId;
            this.OrganizationDateCreated = view.OrganizationDateCreated;

            //Schedule
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
            this.StartDateCreate = view.StartDate;
            this.EndDateCreate = view.EndDate;
            this.CountryId = view.CountryId;
    }

        public void CopyPropertiesToCreateScheduleView(CreateScheduleView view)
        {
            //Subscription
            view.SubscriptionId = this.SubscriptionId;
            view.OrganizationId = this.OrganizationId;
            view.SubscriberId = this.SubscriberId;
            view.SubscriptionEnabled = this.SubscriptionEnabled;
            view.CustomerFullName = this.CustomerFullName;
            view.CustomerIdentifier = this.CustomerIdentifier;
            view.CustomerPhysicalAddress = this.CustomerPhysicalAddress;
            view.CustomerNotes = this.CustomerPhysicalAddress;
            view.SubscriptionDateCreated = this.SubscriptionDateCreated;

            //Subscriber
            view.CellPhoneNumber = this.CellPhoneNumber;
            view.SubscriberName = this.SubscriberName;
            view.SubscriberEnabled = this.SubscriberEnabled;
            view.SubscriberDateCreated = this.SubscriberDateCreated;

            //Organization
            view.OrganizationName = this.OrganizationName;
            view.OrganizationIdentifier = this.OrganizationIdentifier;
            view.OrganizationPrimaryContactEmailAddress = this.OrganizationEmailAddress;
            view.OrganizationAddress = this.OrganizationAddress;
            view.OrganizationSmsCreditsBalance = this.OrganizationSmsCreditsBalance;
            view.OrganizationAllowSmsCreditsDebt = this.OrganizationAllowSmsCreditsDebt;
            view.OrganizationSubscriptionTypeId = this.OrganizationSubscriptionTypeId;
            view.OrganizationSubscriptionEnabled = this.OrganizationSubscriptionEnabled;
            view.OrganizationBillingDayOfTheMonth = this.OrganizationBillingDayOfTheMonth;
            view.IsMondayWorkDay = this.IsMondayWorkDay;
            view.IsTuesdayWorkDay = this.IsTuesdayWorkDay;
            view.IsWednesdayWorkDay = this.IsWednesdayWorkDay;
            view.IsThursdayWorkDay = this.IsThursdayWorkDay;
            view.IsFridayWorkDay = this.IsFridayWorkDay;
            view.IsSaturdayWorkDay = this.IsSaturdayWorkDay;
            view.IsSundayWorkDay = this.IsSundayWorkDay;
            view.AccountManagerUserId = this.AccountManagerUserId;
            view.OrganizationDateCreated = this.OrganizationDateCreated;

            //Schedule
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
            view.StartDate = this.StartDateCreate;
            view.EndDate = this.EndDateCreate;
            view.CountryId = this.CountryId;
        }

        #endregion //Methods
    }
}