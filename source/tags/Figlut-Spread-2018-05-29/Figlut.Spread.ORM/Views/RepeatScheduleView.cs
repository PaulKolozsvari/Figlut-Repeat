﻿namespace Figlut.Spread.ORM.Views
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class RepeatScheduleView
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
    }
}