namespace Figlut.Spread.ORM.Views
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Figlut.Server.Toolkit.Data;

    #endregion //Using Directives

    public class CreateRepeatScheduleView
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

        public RepeatSchedule ToRepeatSchedule()
        {
            return new RepeatSchedule()
            {
                RepeatScheduleId = Guid.NewGuid(),
                SubscriptionId = this.SubscriptionId,
                NotificationMessage = this.NotificationMessage,
                ScheduleName = this.ScheduleName,
                Quantity = this.Quantity,
                UnitOfMeasure = this.UnitOfMeasure,
                DaysRepeatInterval = this.DaysRepeatInterval,
                Notes = this.Notes,
                DateCreated = DateTime.Now
            };
        }

        #endregion //Methods
    }
}