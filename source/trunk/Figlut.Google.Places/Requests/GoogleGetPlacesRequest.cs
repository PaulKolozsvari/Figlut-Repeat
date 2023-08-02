namespace Figlut.Google.Places.Requests
{
    #region Using Directives

    using System.Text;

    #endregion //Using Directives

    public class GoogleGetPlacesRequest
    {
        #region Constructors

        public GoogleGetPlacesRequest(double currentLatitude, double currentLogitude, string type, string keyword, int radius)
        {
            this.CurrentLatitude = currentLatitude;
            this.CurrentLongitude = currentLogitude;
            this.Type = type;
            this.Keyword = keyword;
            this.Radius = radius;
        }

        #endregion //Constructors

        #region Constants

        public const string LOCATION = "location";
        public const string TYPE = "type";
        public const string RADIUS = "radius";
        public const string KEY = "key";

        #endregion //Constants

        #region Properties

        public string Key { get; set; }

        public double CurrentLatitude { get; set; }

        public double CurrentLongitude { get; set; }

        public string Type { get; set; }

        public string Keyword { get; set; }

        public int Radius { get; set; }

        #endregion //Properties

        #region Methods

        public string GetQueryString()
        {
            StringBuilder result = new StringBuilder("nearbysearch/json?");
            string location = $"{this.CurrentLatitude}%2C{this.CurrentLongitude}";
            if (!string.IsNullOrEmpty(this.Keyword))
            {
                result.Append($"{Keyword}={this.Key}&");
            }
            result.Append($"{LOCATION}={location}&");
            if (!string.IsNullOrEmpty(this.Type))
            {
                result.Append($"{TYPE}={this.Type}&");
            }
            result.Append($"{RADIUS}={this.Radius}&");
            result.Append($"{KEY}={this.Key}");
            string resultString = result.ToString();
            return resultString;
        }

        #endregion //Methods
    }
}
