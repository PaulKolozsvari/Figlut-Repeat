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

    public class SubscriberSubscriptionCsv
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

        #endregion //Methods
    }
}
