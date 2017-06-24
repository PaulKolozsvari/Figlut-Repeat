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

    public class SubscriberSubscriptionModel
    {
        #region Properties

        #region Subscription Properties

        public Guid SubscriptionId { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid SubscriberId { get; set; }

        public bool Enabled { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Subscription Properties

        #region Organization Properties

        public string OrganizationName { get; set; }

        public string OrganizationIdentifier { get; set; }

        public string OrganizationEmailAddress { get; set; }

        public string OrganizationAddress { get; set; }

        #endregion //Organization Properties

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (this.SubscriptionId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SubscriberSubscriptionModel>.GetPropertyName(p => p.SubscriptionId, true));
            }
            if (this.OrganizationId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SubscriberSubscriptionModel>.GetPropertyName(p => p.OrganizationId, true));
            }
            if (this.SubscriberId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SubscriberSubscriptionModel>.GetPropertyName(p => p.SubscriberId, true));
            }
            if (this.DateCreated == new DateTime())
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SubscriberSubscriptionModel>.GetPropertyName(p => p.DateCreated, true));
            }
            if (string.IsNullOrEmpty(this.OrganizationName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SubscriberSubscriptionModel>.GetPropertyName(p => p.OrganizationName, true));
            }
            if (string.IsNullOrEmpty(this.OrganizationIdentifier))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SubscriberSubscriptionModel>.GetPropertyName(p => p.OrganizationIdentifier, true));
            }
            if (string.IsNullOrEmpty(this.OrganizationEmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SubscriberSubscriptionModel>.GetPropertyName(p => p.OrganizationEmailAddress, true));
            }
            if (string.IsNullOrEmpty(this.OrganizationAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SubscriberSubscriptionModel>.GetPropertyName(p => p.OrganizationAddress, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromSubscriberSubscriptionView(SubscriberSubscriptionView view)
        {
            this.SubscriptionId = view.SubscriptionId;
            this.OrganizationId = view.OrganizationId;
            this.SubscriberId = view.SubscriberId;
            this.Enabled = view.Enabled;
            this.DateCreated = view.DateCreated;

            this.OrganizationName = view.OrganizationName;
            this.OrganizationIdentifier = view.OrganizationIdentifier;
            this.OrganizationEmailAddress = view.OrganizationEmailAddress;
            this.OrganizationAddress = view.OrganizationAddress;
        }

        public void CopyPropertiesToSubscriberSubscriptionView(SubscriberSubscriptionView view)
        {
            view.SubscriptionId = this.SubscriptionId;
            view.OrganizationId = this.OrganizationId;
            view.SubscriberId = this.SubscriberId;
            view.Enabled = this.Enabled;
            view.DateCreated = this.DateCreated;

            view.OrganizationName = this.OrganizationName;
            view.OrganizationIdentifier = this.OrganizationIdentifier;
            view.OrganizationEmailAddress = this.OrganizationEmailAddress;
            view.OrganizationAddress = this.OrganizationAddress;
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