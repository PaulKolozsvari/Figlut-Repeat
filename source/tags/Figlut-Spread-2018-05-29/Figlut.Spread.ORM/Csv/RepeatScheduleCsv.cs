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

    public class RepeatScheduleCsv
    {
        #region Properties

        #region Repeat Schedule Properties

        public Guid RepeatScheduleId { get; set; }

        public Guid SubscriptionId { get; set; }

        public string NotificationMessage { get; set; }

        public string ScheduleName { get; set; }

        public Nullable<double> Quantity { get; set; }

        public string UnitOfMeasure { get; set; }

        public int DaysRepeatInterval { get; set; }

        public string Notes { get; set; }

        public DateTime DateCreated { get; set; }

        public Nullable<DateTime> StartDate { get; set; }

        public string StartDateFormatted { get; set; }

        public Nullable<DateTime> EndDate { get; set; }

        public string EndDateFormatted { get; set; }

        public int EntryCount { get; set; }

        #endregion //Repeat Schedule Properties

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

        public void CopyPropertiesFromRepeatScheduleView(RepeatScheduleView view)
        {
            this.RepeatScheduleId = view.RepeatScheduleId;
            this.SubscriptionId = view.SubscriptionId;
            this.NotificationMessage = view.NotificationMessage;
            this.ScheduleName = view.ScheduleName;
            this.Quantity = view.Quantity;
            this.UnitOfMeasure = view.UnitOfMeasure;
            this.DaysRepeatInterval = view.DaysRepeatInterval;
            this.Notes = view.Notes;
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

        public void CopyPropertiesToRepeatSchedule(RepeatScheduleView view)
        {
            view.RepeatScheduleId = this.RepeatScheduleId;
            view.SubscriptionId = this.SubscriptionId;
            view.NotificationMessage = this.NotificationMessage;
            view.ScheduleName = this.ScheduleName;
            view.Quantity = this.Quantity;
            view.UnitOfMeasure = this.UnitOfMeasure;
            view.DaysRepeatInterval = this.DaysRepeatInterval;
            view.Notes = this.Notes;
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
