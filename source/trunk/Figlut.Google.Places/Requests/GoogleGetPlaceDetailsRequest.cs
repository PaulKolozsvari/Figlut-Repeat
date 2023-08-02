namespace Figlut.Google.Places.Requests
{
    #region Using Directives

    using System.Text;

    #endregion //Using Directives

    public class GoogleGetPlaceDetailsRequest
    {
        #region Constructors

        public GoogleGetPlaceDetailsRequest(string placeId)
        {
            this.PlaceId = placeId;
        }

        #endregion //Constructors

        #region Constants

        public const string PLACE_ID = "placeid";
        public const string KEY = "key";

        #endregion //Constants

        #region Properties

        public string PlaceId { get; set; }

        public string Key { get; set; }

        #endregion //Properties

        #region Methods

        public string GetQueryString()
        {
            StringBuilder result = new StringBuilder("details/json?");
            result.Append($"{PLACE_ID}={this.PlaceId}&");
            result.Append($"{KEY}={this.Key}");
            string resultString = result.ToString();
            return resultString;
        }

        #endregion //Methods
    }
}
