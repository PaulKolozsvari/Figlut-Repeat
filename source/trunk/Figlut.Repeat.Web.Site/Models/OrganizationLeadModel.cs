namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Views;
    using Figlut.Server.Toolkit.Data;

    #endregion //Using Directives

    public class OrganizationLeadModel
    {
        #region Properties

        public Guid OrganizationLeadId { get; set; }

        public Guid OrganizationId { get; set; }

        public string SearchLocationCentreName { get; set; }

        public double SearchLocationCentreLatitude { get; set; }

        public double SearchLocationCentreLongitude { get; set; }

        public int SearchLocationRadius { get; set; }

        public string GooglePlaceId { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Address { get; set; }

        public string Vicinity { get; set; }

        public string PhoneNumber { get; set; }

        public string InternationPhoneNumber { get; set; }

        public bool IsMobilePhoneNumber { get; set; }

        public string WebsiteUrl { get; set; }

        public string BusinessStatus { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            string formattedPhoneNumber = null;
            if (string.IsNullOrEmpty(this.Name))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationLeadModel>.GetPropertyName(p => p.Name, true));
            }
            else if (string.IsNullOrEmpty(this.PhoneNumber))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationLeadModel>.GetPropertyName(p => p.PhoneNumber, true));
            }
            else if (string.IsNullOrEmpty(this.InternationPhoneNumber))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<OrganizationLeadModel>.GetPropertyName(p => p.InternationPhoneNumber, true));
            }
            else if (!DataShaper.IsValidPhoneNumber(this.PhoneNumber, out formattedPhoneNumber))
            {
                errorMessage = string.Format("{0} is not a valid phone number.", this.InternationPhoneNumber);
            }
            else if (!DataShaper.IsValidPhoneNumber(this.InternationPhoneNumber, out formattedPhoneNumber))
            {
                errorMessage = string.Format("{0} is not a valid phone number.", this.InternationPhoneNumber);
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromOrganizationLead(OrganizationLead organizationLead)
        {
            EntityReader.CopyProperties(organizationLead, this, true);
        }

        public void CopyPropertiesToOrganizationLead(OrganizationLead organizationLead)
        {
            EntityReader.CopyProperties(this, organizationLead, true);
        }

        #endregion //Methods
    }
}