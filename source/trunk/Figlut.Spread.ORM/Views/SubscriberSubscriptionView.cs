namespace Figlut.Spread.ORM.Views
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SubscriberSubscriptionView
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

        public DateTime OrganizationDateCreated { get; set; }

        #endregion //Organization Properties

        #endregion //Properties
    }
}
