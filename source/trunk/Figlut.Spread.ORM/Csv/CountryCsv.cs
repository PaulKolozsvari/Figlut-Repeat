namespace Figlut.Spread.ORM.Csv
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class CountryCsv
    {
        #region Country Properties

        public Guid CountryId { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Country Properties

        #region Methods

        public void CopyPropertiesFromCountry(Country country)
        {
            this.CountryId = country.CountryId;
            this.CountryCode = country.CountryCode;
            this.CountryName = country.CountryName;
            this.DateCreated = country.DateCreated;
        }

        public void CopyPropertiesToCountry(Country country)
        {
            country.CountryId = this.CountryId;
            country.CountryCode = this.CountryCode;
            country.CountryName = this.CountryName;
            country.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}
