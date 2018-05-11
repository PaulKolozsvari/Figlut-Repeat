namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Figlut.Server.Toolkit.Data;

    #endregion //Using Directives

    public class GenerateCountryPublicHolidaysModel
    {
        #region Properties

        public Guid CountryId { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public int Year { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (this.CountryId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<GenerateCountryPublicHolidaysModel>.GetPropertyName(p => p.CountryId, true));
            }
            if (string.IsNullOrEmpty(this.CountryCode))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<GenerateCountryPublicHolidaysModel>.GetPropertyName(p => p.CountryCode, true));
            }
            if (string.IsNullOrEmpty(this.CountryName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<GenerateCountryPublicHolidaysModel>.GetPropertyName(p => p.CountryName, true));
            }
            if (this.Year == 0)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<GenerateCountryPublicHolidaysModel>.GetPropertyName(p => p.Year, true));
            }
            if (this.Year.ToString().Length != 4)
            {
                errorMessage = string.Format("{0} must be 4 digits long and greater than 2000.", EntityReader<GenerateCountryPublicHolidaysModel>.GetPropertyName(p => p.Year, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        #endregion //Methods
    }
}