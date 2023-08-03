using System;
using System.Net.NetworkInformation;

namespace Figlut.Google.Places
{
    public class GooglePlaceInfo
    {
        #region Constructors

        public GooglePlaceInfo(GooglePlaceCentre centre)
        {
            this.CentreName = centre.Name;
            this.CentreLatitude = centre.Latitude;
            this.CentreLongitude = centre.Longitude;
            this.RadiusFromCentre = centre.Radius;
        }

        #endregion //Constructors

        #region Properties

        public string CentreName { get; set; }

        public double CentreLatitude { get; set; }

        public double CentreLongitude { get; set; }

        public int RadiusFromCentre { get; set; }

        public string PlaceId { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Address { get; set; }

        public string Vicinity { get; set; }

        public string PhoneNumber { get; set; }

        public string InternationalPhoneNumber { get; set; }

        public Nullable<bool> IsMobilePhoneNumber { get; set; }

        public string Website { get; set; }

        public string BusinessStatus { get; set; }

        #endregion //Properties
    }
}
