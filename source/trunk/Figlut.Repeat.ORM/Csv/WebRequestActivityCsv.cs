namespace Figlut.Repeat.ORM.Csv
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class WebRequestActivityCsv
    {
        #region Web Request Activity Properties

        public Guid WebRequestActivityId { get; set; }

        public string RequestVerb { get; set; }

        public string RequestUrl { get; set; }

        public string RequestReferrerUrl { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string UserAgent { get; set; }

        public string UserHostAddress { get; set; }

        public string UserHostName { get; set; }

        public string ClientType { get; set; }

        public Nullable<bool> IsCrawler { get; set; }

        public Nullable<bool> IsMobileDevice { get; set; }

        public string MobileDeviceManufacturer { get; set; }

        public string MobileDeviceModel { get; set; }

        public string Platform { get; set; }

        public Nullable<Guid> UserId { get; set; }

        public string UserName { get; set; }

        public string SourceApplication { get; set; }

        public string Comments { get; set; }

        public string WhoIsStatus { get; set; }

        public string WhoIsCountry { get; set; }

        public string WhoIsCountryCode { get; set; }

        public string WhoIsRegion { get; set; }

        public string WhoIsRegionName { get; set; }

        public string WhoIsCity { get; set; }

        public string WhoIsZip { get; set; }

        public string WhoIsLatitude { get; set; }

        public string WhoIsLongitude { get; set; }

        public string WhoIsTimeZone { get; set; }

        public string WhoIsISP { get; set; }

        public string WhoIsOrg { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Web Request Activity Properties

        #region Methods

        public void CopyPropertiesFromWebRequestActivity(WebRequestActivity webRequestActivity)
        {
            this.WebRequestActivityId = webRequestActivity.WebRequestActivityId;
            this.RequestVerb = webRequestActivity.RequestVerb;
            this.RequestUrl = webRequestActivity.RequestUrl;
            this.RequestReferrerUrl = webRequestActivity.RequestReferrerUrl;
            this.Controller = webRequestActivity.Controller;
            this.Action = webRequestActivity.Action;
            this.UserAgent = webRequestActivity.UserAgent;
            this.UserHostAddress = webRequestActivity.UserHostAddress;
            this.UserHostName = webRequestActivity.UserHostName;
            this.ClientType = webRequestActivity.ClientType;
            this.IsCrawler = webRequestActivity.IsCrawler;
            this.IsMobileDevice = webRequestActivity.IsMobileDevice;
            this.MobileDeviceManufacturer = webRequestActivity.MobileDeviceManufacturer;
            this.MobileDeviceModel = webRequestActivity.MobileDeviceModel;
            this.Platform = webRequestActivity.Platform;
            this.UserId = webRequestActivity.UserId;
            this.UserName = webRequestActivity.UserName;
            this.SourceApplication = webRequestActivity.SourceApplication;
            this.Comments = webRequestActivity.Comments;

            this.WhoIsStatus = webRequestActivity.WhoIsStatus;
            this.WhoIsCountry = webRequestActivity.WhoIsCountry;
            this.WhoIsCountryCode = webRequestActivity.WhoIsCountryCode;
            this.WhoIsRegion = webRequestActivity.WhoIsRegion;
            this.WhoIsRegionName = webRequestActivity.WhoIsRegionName;
            this.WhoIsCity = webRequestActivity.WhoIsCity;
            this.WhoIsZip = webRequestActivity.WhoIsZip;
            this.WhoIsLatitude = webRequestActivity.WhoIsLatitude;
            this.WhoIsLongitude = webRequestActivity.WhoIsLongitude;
            this.WhoIsTimeZone = webRequestActivity.WhoIsTimeZone;
            this.WhoIsISP = webRequestActivity.WhoIsISP;
            this.WhoIsOrg = webRequestActivity.WhoIsOrg;

            this.DateCreated = webRequestActivity.DateCreated;
        }

        public void CopyPropertiesToWebRequestActivity(WebRequestActivity webRequestActivity)
        {
            webRequestActivity.WebRequestActivityId = this.WebRequestActivityId;
            webRequestActivity.RequestVerb = this.RequestVerb;
            webRequestActivity.RequestUrl = this.RequestUrl;
            webRequestActivity.RequestReferrerUrl = this.RequestReferrerUrl;
            webRequestActivity.Controller = this.Controller;
            webRequestActivity.Action = this.Action;
            webRequestActivity.UserAgent = this.UserAgent;
            webRequestActivity.UserHostAddress = this.UserHostAddress;
            webRequestActivity.UserHostName = this.UserHostName;
            webRequestActivity.ClientType = this.ClientType;
            webRequestActivity.IsCrawler = this.IsCrawler;
            webRequestActivity.IsMobileDevice = this.IsMobileDevice;
            webRequestActivity.MobileDeviceManufacturer = this.MobileDeviceManufacturer;
            webRequestActivity.MobileDeviceModel = this.MobileDeviceModel;
            webRequestActivity.Platform = this.Platform;
            webRequestActivity.UserId = this.UserId;
            webRequestActivity.UserName = this.UserName;
            webRequestActivity.SourceApplication = this.SourceApplication;
            webRequestActivity.Comments = this.Comments;

            webRequestActivity.WhoIsStatus = this.WhoIsStatus;
            webRequestActivity.WhoIsCountry = this.WhoIsCountry;
            webRequestActivity.WhoIsCountryCode = this.WhoIsCountryCode;
            webRequestActivity.WhoIsRegion = this.WhoIsRegion;
            webRequestActivity.WhoIsRegionName = this.WhoIsRegionName;
            webRequestActivity.WhoIsCity = this.WhoIsCity;
            webRequestActivity.WhoIsZip = this.WhoIsZip;
            webRequestActivity.WhoIsLatitude = this.WhoIsLatitude;
            webRequestActivity.WhoIsLongitude = this.WhoIsLongitude;
            webRequestActivity.WhoIsTimeZone = this.WhoIsTimeZone;
            webRequestActivity.WhoIsISP = this.WhoIsISP;
            webRequestActivity.WhoIsOrg = this.WhoIsOrg;

            webRequestActivity.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}
