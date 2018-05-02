namespace Figlut.Spread.Web.Site.Models
{
    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Views;
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class PublicHolidayModel
    {
        #region Public Holiday Properties

        public Guid PublicHolidayId { get; set; }

        public Guid CountryId { get; set; }

        public string EventName { get; set; }

        public string DateIdentifier { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public int Day { get; set; }

        public DateTime HolidayDate { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Public Holiday Properties

        #region Country Properties

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        #endregion //Country Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (this.CountryId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<PublicHolidayModel>.GetPropertyName(p => p.CountryId, true));
            }
            if (string.IsNullOrEmpty(this.EventName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<PublicHolidayModel>.GetPropertyName(p => p.EventName, true));
            }
            if (string.IsNullOrEmpty(this.DateIdentifier))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<PublicHolidayModel>.GetPropertyName(p => p.DateIdentifier, true));
            }
            if (this.Year == 0)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<PublicHolidayModel>.GetPropertyName(p => p.Year, true));
            }
            if (this.Month == 0)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<PublicHolidayModel>.GetPropertyName(p => p.Month, true));
            }
            if (this.Day == 0)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<PublicHolidayModel>.GetPropertyName(p => p.Day, true));
            }
            if (this.HolidayDate == new DateTime())
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<PublicHolidayModel>.GetPropertyName(p => p.HolidayDate, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromPublicHolidayView(PublicHolidayView view)
        {
            this.PublicHolidayId = view.PublicHolidayId;
            this.CountryId = view.CountryId;
            this.EventName = view.EventName;
            this.DateIdentifier = view.DateIdentifier;
            this.Year = view.Year;
            this.Month = view.Month;
            this.Day = view.Day;
            this.HolidayDate = view.HolidayDate;
            this.DateCreated = view.DateCreated;

            this.CountryCode = view.CountryCode;
            this.CountryName = view.CountryName;
        }

        public void CopyPropertiesToPublicHolidayView(PublicHolidayView view)
        {
            view.PublicHolidayId = this.PublicHolidayId;
            view.CountryId = this.CountryId;
            view.EventName = this.EventName;
            view.DateIdentifier = this.DateIdentifier;
            view.Year = this.Year;
            view.Month = this.Month;
            view.Day = this.Day;
            view.HolidayDate = this.HolidayDate;
            view.DateCreated = this.DateCreated;

            view.CountryCode = this.CountryCode;
            view.CountryName = this.CountryName;
        }

        public void CopyPropertiesToPublicHoliday(PublicHoliday publicHoliday)
        {
            publicHoliday.PublicHolidayId = this.PublicHolidayId;
            publicHoliday.CountryId = this.CountryId;
            publicHoliday.EventName = this.EventName;
            publicHoliday.DateIdentifier = this.DateIdentifier;
            publicHoliday.Year = this.Year;
            publicHoliday.Month = this.Month;
            publicHoliday.Day = this.Day;
            publicHoliday.HolidayDate = this.HolidayDate;
            publicHoliday.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}