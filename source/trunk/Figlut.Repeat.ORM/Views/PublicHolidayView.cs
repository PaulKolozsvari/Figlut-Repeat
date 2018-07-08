namespace Figlut.Repeat.ORM.Views
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class PublicHolidayView
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
    }
}