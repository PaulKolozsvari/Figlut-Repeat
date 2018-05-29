namespace Figlut.Spread.ORM.Csv
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class OrganizationCsv
    {
        #region Properties

        public Guid OrganizationId { get; set; }

        public string Name { get; set; }

        public string Identifier { get; set; }

        public string EmailAddress { get; set; }

        public string Address { get; set; }

        public long SmsCreditsBalance { get; set; }

        public bool AllowSmsCreditsDebt { get; set; }

        public Nullable<Guid> OrganizationSubscriptionTypeId { get; set; }

        public bool OrganizationSubscriptionEnabled { get; set; }

        public int BillingDayOfTheMonth { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromOrganization(Organization organization)
        {
            this.OrganizationId = organization.OrganizationId;
            this.Name = organization.Name;
            this.Identifier = organization.Identifier;
            this.EmailAddress = organization.EmailAddress;
            this.Address = organization.Address;
            this.SmsCreditsBalance = organization.SmsCreditsBalance;
            this.AllowSmsCreditsDebt = organization.AllowSmsCreditsDebt;
            this.OrganizationSubscriptionTypeId = organization.OrganizationSubscriptionTypeId;
            this.OrganizationSubscriptionEnabled = organization.OrganizationSubscriptionEnabled;
            this.BillingDayOfTheMonth = organization.BillingDayOfTheMonth;
            this.DateCreated = organization.DateCreated;
        }

        public void CopyPropertiesToOrganization(Organization organization)
        {
            organization.OrganizationId = this.OrganizationId;
            organization.Name = this.Name;
            organization.Identifier = this.Identifier;
            organization.EmailAddress = this.EmailAddress;
            organization.Address = this.Address;
            organization.SmsCreditsBalance = this.SmsCreditsBalance;
            organization.AllowSmsCreditsDebt = this.AllowSmsCreditsDebt;
            organization.OrganizationSubscriptionTypeId = this.OrganizationSubscriptionTypeId;
            organization.OrganizationSubscriptionEnabled = this.OrganizationSubscriptionEnabled;
            organization.BillingDayOfTheMonth = this.BillingDayOfTheMonth;
            organization.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}