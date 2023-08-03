namespace Figlut.Repeat.ORM.Csv
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class OrganizationLeadCsv
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

        public void CopyPropertiesFromOrganizationLead(OrganizationLead organizationLead)
        {
            EntityReader.CopyProperties(organizationLead, this, true);
        }

        public void CopyPropertiesToOrganizationLead(OrganizationLead organizationLead)
        {
            EntityReader.CopyProperties(organizationLead, this, true);
        }

        #endregion //Methods
    }
}
