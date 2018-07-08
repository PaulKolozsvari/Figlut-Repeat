namespace Figlut.Repeat.ORM.Views
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class OrganizationSubscriptionView
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

        #region Subscriber Properties

        public string SubscriberCellPhoneNumber { get; set; }

        public string SubscriberName { get; set; }

        public bool SubscriberEnabled { get; set; }

        public DateTime SubscriberDateCreated { get; set; }

        #endregion //Subscriber Properties

        #endregion //Properties

        #region Methods

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
