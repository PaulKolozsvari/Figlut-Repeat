namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Views;
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

        public string CustomerFullName { get; set; }

        public string CustomerEmailAddress { get; set; }

        public string CustomerIdentifier { get; set; }

        public string CustomerPhysicalAddress { get; set; }

        public string CustomerNotes { get; set; }

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
            if (!string.IsNullOrEmpty(this.CustomerEmailAddress) && !DataShaper.IsValidEmail(this.CustomerEmailAddress))
            {
                errorMessage = string.Format("{0} is not a valid email address.", EntityReader<OrganizationSubscriptionModel>.GetPropertyName(p => p.CustomerEmailAddress, true));
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
            this.CustomerFullName = view.CustomerFullName;
            this.CustomerEmailAddress = view.CustomerEmailAddress;
            this.CustomerIdentifier = view.CustomerIdentifier;
            this.CustomerPhysicalAddress = view.CustomerPhysicalAddress;
            this.CustomerNotes = view.CustomerNotes;
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
            view.CustomerFullName = this.CustomerFullName;
            view.CustomerEmailAddress = this.CustomerEmailAddress;
            view.CustomerIdentifier = this.CustomerIdentifier;
            view.CustomerPhysicalAddress = this.CustomerPhysicalAddress;
            view.CustomerNotes = this.CustomerNotes;
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
            subscription.CustomerFullName = this.CustomerFullName;
            subscription.CustomerEmailAddress = this.CustomerEmailAddress;
            subscription.CustomerIdentifier = this.CustomerIdentifier;
            subscription.CustomerPhysicalAddress = this.CustomerPhysicalAddress;
            subscription.CustomerNotes = this.CustomerNotes;
            subscription.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}