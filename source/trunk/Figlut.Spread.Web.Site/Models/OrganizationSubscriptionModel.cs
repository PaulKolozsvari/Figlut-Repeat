namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class OrganizationSubscriptionModel
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

        public string SubscriberCellPhoneNumber { get; set; }

        public string SubscriberName { get; set; }

        public bool SubscriberIsEnabled { get; set; }

        public DateTime SubscriberDateCreated { get; set; }

        #endregion //Subscriber Properties

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (this.SubscriptionId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationSubscriptionModel>.GetPropertyName(p => p.SubscriptionId, true));
            }
            if (this.OrganizationId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationSubscriptionModel>.GetPropertyName(p => p.OrganizationId, true));
            }
            if (this.SubscriberId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationSubscriptionModel>.GetPropertyName(p => p.SubscriberId, true));
            }
            if (this.DateCreated == new DateTime())
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationSubscriptionModel>.GetPropertyName(p => p.DateCreated, true));
            }
            if (string.IsNullOrEmpty(this.SubscriberCellPhoneNumber))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationSubscriptionModel>.GetPropertyName(p => p.SubscriberCellPhoneNumber, true));
            }
            if (this.SubscriberDateCreated == new DateTime())
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationSubscriptionModel>.GetPropertyName(p => p.SubscriberDateCreated, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromOrganizationSubscriptionView(OrganizationSubscriptionView view)
        {
            this.SubscriptionId = view.SubscriptionId;
            this.OrganizationId = view.OrganizationId;
            this.SubscriberId = view.SubscriberId;
            this.Enabled = view.Enabled;
            this.CustomerFullName = view.CustomerFullName;
            this.CustomerIdentifier = view.CustomerIdentifier;
            this.CustomerPhysicalAddress = view.CustomerPhysicalAddress;
            this.CustomerNotes = view.CustomerNotes;
            this.DateCreated = view.DateCreated;

            this.SubscriberCellPhoneNumber = view.SubscriberCellPhoneNumber;
            this.SubscriberName = view.SubscriberName;
            this.SubscriberIsEnabled = view.SubscriberEnabled;
            this.SubscriberDateCreated = view.SubscriberDateCreated;
        }

        public void CopyPropertiesToOrganizationSubscriptionView(OrganizationSubscriptionView view)
        {
            view.SubscriptionId = this.SubscriptionId;
            view.OrganizationId = this.OrganizationId;
            view.SubscriberId = this.SubscriberId;
            view.Enabled = this.Enabled;
            view.CustomerFullName = this.CustomerFullName;
            view.CustomerIdentifier = this.CustomerIdentifier;
            view.CustomerPhysicalAddress = this.CustomerPhysicalAddress;
            view.CustomerNotes = this.CustomerNotes;
            view.DateCreated = this.DateCreated;

            view.SubscriberCellPhoneNumber = this.SubscriberCellPhoneNumber;
            view.SubscriberName = this.SubscriberName;
            view.SubscriberEnabled = this.SubscriberIsEnabled;
            view.SubscriberDateCreated = this.SubscriberDateCreated;
        }

        public void CopyPropertiesToSubscription(Subscription subscription)
        {
            subscription.SubscriptionId = this.SubscriptionId;
            subscription.OrganizationId = this.OrganizationId;
            subscription.SubscriberId = this.SubscriberId;
            subscription.Enabled = this.Enabled;
            subscription.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}