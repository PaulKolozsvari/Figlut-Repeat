﻿namespace Figlut.Spread.ORM.Csv
{
    #region Using Directives

    using Figlut.Spread.ORM.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class OrganizationSubscriptionCsv
    {
        #region Properties

        #region Subscription Properties

        public Guid SubscriptionId { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid SubscriberId { get; set; }

        public bool Enabled { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Subscription Properties

        #region Subscriber Properties

        public string SubscriberCellPhoneNumber { get; set; }

        public string SubscriberName { get; set; }

        public bool SubscriberIsEnabled { get; set; }

        public DateTime SubscriberDateCreated { get; set; }

        #endregion //Subscriber Properties

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromOrganizationSubscriptionView(OrganizationSubscriptionView view)
        {
            this.SubscriptionId = view.SubscriptionId;
            this.OrganizationId = view.OrganizationId;
            this.SubscriberId = view.SubscriberId;
            this.Enabled = view.Enabled;
            this.DateCreated = view.DateCreated;

            this.SubscriberCellPhoneNumber = view.SubscriberCellPhoneNumber;
            this.SubscriberName = view.SubscriberName;
            this.SubscriberIsEnabled = view.SubscriberIsEnabled;
            this.SubscriberDateCreated = view.SubscriberDateCreated;
        }

        public void CopyPropertiesTo(OrganizationSubscriptionView view)
        {
            view.SubscriptionId = this.SubscriptionId;
            view.OrganizationId = this.OrganizationId;
            view.SubscriberId = this.SubscriberId;
            view.Enabled = this.Enabled;
            view.DateCreated = this.DateCreated;

            view.SubscriberCellPhoneNumber = this.SubscriberCellPhoneNumber;
            view.SubscriberName = this.SubscriberName;
            view.SubscriberIsEnabled = this.SubscriberIsEnabled;
            view.SubscriberDateCreated = this.SubscriberDateCreated;
        }

        #endregion //Methods
    }
}