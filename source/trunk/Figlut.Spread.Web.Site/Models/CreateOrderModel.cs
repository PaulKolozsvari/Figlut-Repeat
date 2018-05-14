namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Figlut.Spread.ORM;

    #endregion //Using Directives

    public class CreateOrderModel
    {
        #region Properties

        #region Organization

        public Guid OrganizationId { get; set; }

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

        #endregion //Organization

        #region Organization Subscription Type

        public string OrganizationSubscriptionTypeName { get; set; }

        public decimal OrganizationSubscriptionTypeMonthlySubscriptionPrice { get; set; }

        public decimal OrganizationSubscriptionTypeSmsUnitPrice { get; set; }

        public DateTime OrganizationSubscriptionTypeDateCreated { get; set; }

        #endregion //Organization Subscription Type

        #region Order

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        #endregion //Order

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromOrganization(Organization organization, OrganizationSubscriptionType organizationSubscriptionType)
        {
            this.OrganizationId = organization.OrganizationId;
            this.OrganizationName = organization.Name;
            this.OrganizationIdentifier = organization.Identifier;
            this.OrganizationEmailAddress = organization.EmailAddress;
            this.OrganizationSmsCreditsBalance = organization.SmsCreditsBalance;
            this.OrganizationAllowSmsCreditsDebt = organization.AllowSmsCreditsDebt;
            this.OrganizationSubscriptionTypeId = organization.OrganizationSubscriptionTypeId;
            this.OrganizationSubscriptionEnabled = organization.OrganizationSubscriptionEnabled;
            this.OrganizationBillingDayOfTheMonth = organization.BillingDayOfTheMonth;
            this.OrganizationDateCreated = organization.DateCreated;

            if (organizationSubscriptionType != null)
            {
                this.OrganizationSubscriptionTypeName = organizationSubscriptionType.Name;
                this.OrganizationSubscriptionTypeMonthlySubscriptionPrice = organizationSubscriptionType.MonthlySubscriptionPrice;
                this.OrganizationSubscriptionTypeSmsUnitPrice = organizationSubscriptionType.SmsUnitPrice;
                this.OrganizationSubscriptionTypeDateCreated = organizationSubscriptionType.DateCreated;
            }
        }

        #endregion //Methods
    }
}