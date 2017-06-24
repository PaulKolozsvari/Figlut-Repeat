namespace Figlut.Spread.ORM.Views
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

        public DateTime DateCreated { get; set; }

        #endregion //Subscription Properties

        #region Subscriber Properties

        public string SubscriberCellPhoneNumber { get; set; }

        public string SubscriberName { get; set; }

        public bool SubscriberIsEnabled { get; set; }

        public DateTime SubscriberDateCreated { get; set; }

        #endregion //Subscriber Properties

        #endregion //Properties
    }
}
