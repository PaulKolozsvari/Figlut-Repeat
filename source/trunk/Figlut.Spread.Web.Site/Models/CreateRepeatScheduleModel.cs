namespace Figlut.Spread.Web.Site.Models
{
    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM.Views;
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class CreateRepeatScheduleModel
    {
        #region Properties

        #region Subscription Properties

        public Guid SubscriptionId { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid SubscriberId { get; set; }

        public bool Enabled { get; set; }

        public string CustomerFullName { get; set; }

        public string CustomerIdentifier { get; set; }

        public string CustomerPhysicalAddress { get; set; }

        public string CustomerNotes { get; set; }

        public DateTime DateCreated { get; set; }

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

        public DateTime OrganizationDateCreated { get; set; }

        #endregion //Organization Properties

        #region Repeat Schedule

        public string NotificationMessage { get; set; }

        public string ScheduleName { get; set; }

        public Nullable<double> Quantity { get; set; }

        public string UnitOfMeasure { get; set; }

        public int DaysRepeatInterval { get; set; }

        public string Notes { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid CountryId { get; set; }

        #endregion //Repeat Schedule

        #endregion Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            //Subscription
            if (this.SubscriptionId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.SubscriptionId, true));
            }
            if (this.OrganizationId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.OrganizationId, true));
            }
            if (this.SubscriberId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.SubscriberId, true));
            }
            //Subscriber
            if (string.IsNullOrEmpty(this.CellPhoneNumber))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.CellPhoneNumber, true));
            }
            //Organization
            if (string.IsNullOrEmpty(this.OrganizationName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.OrganizationName, true));
            }
            if (string.IsNullOrEmpty(this.OrganizationIdentifier))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.OrganizationIdentifier, true));
            }
            if (string.IsNullOrEmpty(this.OrganizationEmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.OrganizationEmailAddress, true));
            }
            if (string.IsNullOrEmpty(this.OrganizationAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.OrganizationAddress, true));
            }
            //Repeat Schedule
            if (string.IsNullOrEmpty(this.NotificationMessage))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.NotificationMessage, true));
            }
            if (string.IsNullOrEmpty(this.ScheduleName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.ScheduleName, true));
            }
            if (this.DaysRepeatInterval <= 0)
            {
                errorMessage = string.Format("{0} may not be less than or equal to 0.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.DaysRepeatInterval, true));
            }
            if (this.Quantity.HasValue && this.Quantity.Value < 0.0)
            {
                errorMessage = string.Format("{0} may not be less than 0.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.Quantity, true));
            }
            if (this.StartDate == new DateTime())
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.StartDate, true));
            }
            if (this.EndDate == new DateTime())
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.StartDate, true));
            }
            if (this.StartDate.Date > this.EndDate)
            {
                errorMessage = string.Format("{0} may not be greater than the {1}.",
                    EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.StartDate, true),
                    EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.EndDate, true));
            }
            if (this.CountryId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CreateRepeatScheduleModel>.GetPropertyName(p => p.CountryId, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromCreateRepeatScheduleView(CreateRepeatScheduleView view)
        {
            //Subscription
            this.SubscriptionId = view.SubscriptionId;
            this.OrganizationId = view.OrganizationId;
            this.SubscriberId = view.SubscriberId;
            this.Enabled = view.Enabled;
            this.CustomerFullName = view.CustomerFullName;
            this.CustomerIdentifier = view.CustomerIdentifier;
            this.CustomerPhysicalAddress = view.CustomerPhysicalAddress;
            this.CustomerNotes = view.CustomerPhysicalAddress;
            this.DateCreated = view.DateCreated;

            //Subscriber
            this.CellPhoneNumber = view.CellPhoneNumber;
            this.SubscriberName = view.SubscriberName;
            this.SubscriberEnabled = view.SubscriberEnabled;
            this.SubscriberDateCreated = view.SubscriberDateCreated;

            //Organization
            this.OrganizationName = view.OrganizationName;
            this.OrganizationIdentifier = view.OrganizationIdentifier;
            this.OrganizationEmailAddress = view.OrganizationEmailAddress;
            this.OrganizationAddress = view.OrganizationAddress;
            this.OrganizationSmsCreditsBalance = view.OrganizationSmsCreditsBalance;
            this.OrganizationAllowSmsCreditsDebt = view.OrganizationAllowSmsCreditsDebt;
            this.OrganizationSubscriptionTypeId = view.OrganizationSubscriptionTypeId;
            this.OrganizationSubscriptionEnabled = view.OrganizationSubscriptionEnabled;
            this.OrganizationBillingDayOfTheMonth = view.OrganizationBillingDayOfTheMonth;
            this.OrganizationDateCreated = view.OrganizationDateCreated;

            //Repeat Schedule
            this.NotificationMessage = view.NotificationMessage;
            this.ScheduleName = view.ScheduleName;
            this.Quantity = view.Quantity;
            this.UnitOfMeasure = view.UnitOfMeasure;
            this.DaysRepeatInterval = view.DaysRepeatInterval;
            this.Notes = view.Notes;
            this.StartDate = view.StartDate;
            this.EndDate = view.EndDate;
            this.CountryId = view.CountryId;
    }

        public void CopyPropertiesToCreateRepeatScheduleView(CreateRepeatScheduleView view)
        {
            //Subscription
            view.SubscriptionId = this.SubscriptionId;
            view.OrganizationId = this.OrganizationId;
            view.SubscriberId = this.SubscriberId;
            view.Enabled = this.Enabled;
            view.CustomerFullName = this.CustomerFullName;
            view.CustomerIdentifier = this.CustomerIdentifier;
            view.CustomerPhysicalAddress = this.CustomerPhysicalAddress;
            view.CustomerNotes = this.CustomerPhysicalAddress;
            view.DateCreated = this.DateCreated;

            //Subscriber
            view.CellPhoneNumber = this.CellPhoneNumber;
            view.SubscriberName = this.SubscriberName;
            view.SubscriberEnabled = this.SubscriberEnabled;
            view.SubscriberDateCreated = this.SubscriberDateCreated;

            //Organization
            view.OrganizationName = this.OrganizationName;
            view.OrganizationIdentifier = this.OrganizationIdentifier;
            view.OrganizationEmailAddress = this.OrganizationEmailAddress;
            view.OrganizationAddress = this.OrganizationAddress;
            view.OrganizationSmsCreditsBalance = this.OrganizationSmsCreditsBalance;
            view.OrganizationAllowSmsCreditsDebt = this.OrganizationAllowSmsCreditsDebt;
            view.OrganizationSubscriptionTypeId = this.OrganizationSubscriptionTypeId;
            view.OrganizationSubscriptionEnabled = this.OrganizationSubscriptionEnabled;
            view.OrganizationBillingDayOfTheMonth = this.OrganizationBillingDayOfTheMonth;
            view.OrganizationDateCreated = this.OrganizationDateCreated;

            //Repeat Schedule
            view.NotificationMessage = this.NotificationMessage;
            view.ScheduleName = this.ScheduleName;
            view.Quantity = this.Quantity;
            view.UnitOfMeasure = this.UnitOfMeasure;
            view.DaysRepeatInterval = this.DaysRepeatInterval;
            view.Notes = this.Notes;
            view.StartDate = this.StartDate;
            view.EndDate = this.EndDate;
            view.CountryId = this.CountryId;
        }

        #endregion //Methods
    }
}