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

    public class OrganizationProfileModel
    {
        #region Properties

        public Guid OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationIdentifier { get; set; }

        public string OrganizationEmailAddress { get; set; }

        public string OrganizationAddress { get; set; }

        public long SmsCreditsBalance { get; set; }

        public bool AllowSmsCreditsDebt { get; set; }

        public double SmsPrice { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.OrganizationName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationProfileModel>.GetPropertyName(p => p.OrganizationName, true));
            }
            else if (string.IsNullOrEmpty(this.OrganizationIdentifier))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationProfileModel>.GetPropertyName(p => p.OrganizationIdentifier, true));
            }
            else if (this.OrganizationIdentifier.Length > 20)
            {
                errorMessage = string.Format("{0} must be shorter than 20 characters.", EntityReader<RegisterModel>.GetPropertyName(p => p.OrganizationIdentifier, true));
            }
            else if (this.OrganizationIdentifier.Contains(' '))
            {
                errorMessage = string.Format("{0} must not contain spaces.", EntityReader<OrganizationProfileModel>.GetPropertyName(p => p.OrganizationIdentifier, true));
            }
            else if (string.IsNullOrEmpty(this.OrganizationEmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationProfileModel>.GetPropertyName(p => p.OrganizationEmailAddress, true));
            }
            else if (string.IsNullOrEmpty(this.OrganizationAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationProfileModel>.GetPropertyName(p => p.OrganizationAddress, true));
            }
            else if (this.SmsCreditsBalance < 0)
            {
                errorMessage = string.Format("{0} may not be less than 0.", EntityReader<OrganizationProfileModel>.GetPropertyName(p => p.SmsCreditsBalance, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromOrganization(Organization organization)
        {
            this.OrganizationId = organization.OrganizationId;
            this.OrganizationName = organization.Name;
            this.OrganizationIdentifier = organization.Identifier;
            this.OrganizationEmailAddress = organization.EmailAddress;
            this.OrganizationAddress = organization.Address;
            this.SmsCreditsBalance = organization.SmsCreditsBalance;
            this.AllowSmsCreditsDebt = organization.AllowSmsCreditsDebt;
            this.SmsPrice = organization.SmsPrice;
            this.DateCreated = organization.DateCreated;
        }

        public void CopyPropertiesToOrganization(Organization organization)
        {
            organization.OrganizationId = this.OrganizationId;
            organization.Name = this.OrganizationName;
            organization.Identifier = this.OrganizationIdentifier;
            organization.EmailAddress = this.OrganizationEmailAddress;
            organization.Address = this.OrganizationAddress;
            organization.SmsCreditsBalance = this.SmsCreditsBalance;
            organization.AllowSmsCreditsDebt = this.AllowSmsCreditsDebt;
            organization.SmsPrice = this.SmsPrice;
            organization.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}