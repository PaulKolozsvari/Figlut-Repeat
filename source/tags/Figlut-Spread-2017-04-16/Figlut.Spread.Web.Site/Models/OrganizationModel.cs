namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class OrganizationModel
    {
        #region Properties

        public Guid OrganizationId { get; set; }

        public string Name { get; set; }

        public string Identifier { get; set; }

        public string EmailAddress { get; set; }

        public string Address { get; set; }

        public long SmsCreditsBalance { get; set; }

        public bool AllowSmsCreditsDebt { get; set; }

        public double SmsPrice { get; set; }

        public string SMSPriceString { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.Name))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationModel>.GetPropertyName(p => p.Name, true));
            }
            else if (string.IsNullOrEmpty(this.Identifier))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationModel>.GetPropertyName(p => p.Name, true));
            }
            else if (this.Identifier.Length > 20)
            {
                errorMessage = string.Format("{0} must be shorter than 20 characters.", EntityReader<OrganizationModel>.GetPropertyName(p => p.Identifier, true));
            }
            else if (this.Identifier.Contains(' '))
            {
                errorMessage = string.Format("{0} must not contain spaces.", EntityReader<OrganizationModel>.GetPropertyName(p => p.Identifier, true));
            }
            else if (string.IsNullOrEmpty(this.EmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationModel>.GetPropertyName(p => p.EmailAddress, true));
            }
            else if (string.IsNullOrEmpty(this.Address))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationModel>.GetPropertyName(p => p.Address, true));
            }
            else if (this.SmsPrice < 0.0)
            {
                errorMessage = string.Format("{0} may not be a negative number.", EntityReader<OrganizationModel>.GetPropertyName(p => p.SmsPrice, true));
            }
            else if (this.SmsCreditsBalance < 0 && !this.AllowSmsCreditsDebt)
            {
                errorMessage = string.Format("SMS Credits Balance may not be a negative number if SMS Credits Debt is not allowed.");
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromOrganization(Organization organization, string currencySymbol)
        {
            this.OrganizationId = organization.OrganizationId;
            this.Name = organization.Name;
            this.Identifier = organization.Identifier;
            this.EmailAddress = organization.EmailAddress;
            this.Address = organization.Address;
            this.SmsCreditsBalance = organization.SmsCreditsBalance;
            this.AllowSmsCreditsDebt = organization.AllowSmsCreditsDebt;
            this.SmsPrice = organization.SmsPrice;
            this.SMSPriceString = DataShaper.GetCurrencyValueString(organization.SmsPrice, currencySymbol);
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
            organization.SmsPrice = this.SmsPrice;
            organization.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}