namespace Figlut.Google.Places.Requests
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class GoogleGetPlacesNextPageRequest
    {
        #region Constructors

        public GoogleGetPlacesNextPageRequest(string pageToken)
        {
            this.PageToken = pageToken;
        }

        #endregion //Constructors

        #region Constants

        public const string PAGE_TOKEN = "pagetoken";
        public const string KEY = "key";

        #endregion //Constants

        #region Properties

        public string PageToken { get; set; }

        public string Key { get; set; }

        #endregion //Properties

        #region Methods

        public string GetQueryString()
        {
            StringBuilder result = new StringBuilder("nearbysearch/json?");
            result.Append($"{PAGE_TOKEN}={this.PageToken}&");
            result.Append($"{KEY}={this.Key}");
            string resultString = result.ToString();
            return resultString;
        }

        #endregion //Methods
    }
}
