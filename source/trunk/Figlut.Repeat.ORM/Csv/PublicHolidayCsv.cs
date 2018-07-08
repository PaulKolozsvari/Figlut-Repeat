namespace Figlut.Repeat.ORM.Csv
{
    #region Using Directives

    using Figlut.Repeat.ORM.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class PublicHolidayCsv
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

        #endregion //Methods
    }
}
