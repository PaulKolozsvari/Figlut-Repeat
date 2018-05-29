namespace Figlut.Spread.Web.Site.Models
{
    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM;
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class CountryModel
    {
        #region Constants

        public const int MAX_COUNTRY_CODE_LENGTH = 3;
        public const int MAX_COUNTRY_NAME_LENGTH = 100;

        #endregion //Constants

        #region Country Properties

        public Guid CountryId { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Country Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.CountryCode))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CountryModel>.GetPropertyName(p => p.CountryCode, true));
            }
            if (this.CountryCode.Length != MAX_COUNTRY_CODE_LENGTH)
            {
                errorMessage = string.Format("{0} must be {1} characters long as per ISO 3166-1 alpha-3 standard.",
                    EntityReader<CountryModel>.GetPropertyName(p => p.CountryCode, true),
                    MAX_COUNTRY_CODE_LENGTH);
            }
            if (string.IsNullOrEmpty(this.CountryName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CountryModel>.GetPropertyName(p => p.CountryName, true));
            }
            if (this.CountryName.Length > MAX_COUNTRY_NAME_LENGTH)
            {
                errorMessage = string.Format("{0} may not be longer than {1} characters.",
                    EntityReader<CountryModel>.GetPropertyName(p => p.CountryName, true),
                    MAX_COUNTRY_NAME_LENGTH);
            }
            return string.IsNullOrEmpty(errorMessage);
        }

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