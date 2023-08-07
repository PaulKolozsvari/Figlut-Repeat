namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class OrganizationLeadFilterModel : FilterModel<OrganizationLeadModel>
    {
        #region Properties

        public string CentreName { get; set; }

        public double CentreLatitude { get; set; }

        public double CentreLongitude { get; set; }

        public int CentreRadius { get; set; }

        public int ZoomLevel { get; set; }

        #endregion //Properties

        #region Methods

        public string GetCentreLatitudeString()
        {
            return CentreLatitude.ToString(CultureInfo.InvariantCulture);
        }

        public string GetCentreLongitudeString()
        {
            return CentreLongitude.ToString(CultureInfo.InvariantCulture);
        }

        public void SetCentreLocation(string centreName)
        {
            OrganizationLeadModel model = this.DataModel.Where(p => p.SearchLocationCentreName == centreName).FirstOrDefault();
            if (model == null || string.IsNullOrEmpty(model.SearchLocationCentreName)) //Centre of South Africa
            {
                this.CentreLatitude = -30.072096;
                this.CentreLongitude = 24.464407;
                this.CentreRadius = 0;
                this.ZoomLevel = 5;
                return;
            }
            this.CentreName = model.SearchLocationCentreName;
            this.CentreLatitude = model.SearchLocationCentreLatitude;
            this.CentreLongitude = model.SearchLocationCentreLongitude;
            //this.CentreRadius = model.SearchLocationRadius;
            this.CentreRadius = 5000;
            this.ZoomLevel = 15;
        }

        #endregion //Methods
    }
}