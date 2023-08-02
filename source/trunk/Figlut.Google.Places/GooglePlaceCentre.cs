namespace Figlut.Google.Places
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class GooglePlaceCentre
    {
        #region Constructors

        public GooglePlaceCentre(string name, double latitude, double longitude, int radius)
        {
            this.Name = name;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Radius = radius;
        }

        #endregion //Constructors

        #region Properties

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int Radius { get; set; }

        #endregion //Properties
    }
}
