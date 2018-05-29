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
            this.DateCreated = organization.DateCreated;
        }

        public void CopyPropertiesTo(Organization organization)
        {
            organization.OrganizationId = this.OrganizationId;
            organization.Name = this.Name;
            organization.Identifier = this.Identifier;
            organization.EmailAddress = this.EmailAddress;
            organization.Address = this.Address;
            organization.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}