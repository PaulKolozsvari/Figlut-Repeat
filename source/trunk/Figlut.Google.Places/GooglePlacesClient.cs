namespace Figlut.Google.Places
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Google.Places.Requests;
    using Figlut.Google.Places.Responses;
    using Figlut.Google.Places.Responses.List;
    using Figlut.Server.Toolkit.Web.Client;

    #endregion //Using Directives

    public class GooglePlacesClient : JsonWebServiceClient
    {
        #region Constructors

        public GooglePlacesClient(string webServiceBaseUrl, int timeout, string key) : base(webServiceBaseUrl)
        {
            _key = key;
            _timeout = timeout;
        }

        #endregion //Constructors

        #region Fields

        private string _key;
        private JsonWebServiceClient _webClient;
        private int _timeout;

        #endregion //Fields

        #region Properties

        public string Key
        {
            get { return _key; }
        }

        #endregion //Properties

        #region Methods

        public GooglePlacesResponse GetPlaces(GoogleGetPlacesRequest request)
        {
            request.Key = this.Key;
            GooglePlacesResponse result = this.CallService<GooglePlacesResponse>(
                request.GetQueryString(),
                null,
                HttpVerb.GET,
                serializePostObject: false,
                _timeout,
                out HttpStatusCode statusCode,
                out string statusDescription,
                wrapWebException: true,
                new Dictionary<string, string>());
            return result;
        }

        public GooglePlaceDetailResponse GetPlaceDetail(GoogleGetPlaceDetailsRequest request)
        {
            request.Key = this.Key;
            GooglePlaceDetailResponse result = this.CallService<GooglePlaceDetailResponse>(
                request.GetQueryString(),
                null,
                HttpVerb.GET,
                serializePostObject: false,
                _timeout,
                out HttpStatusCode statusCode,
                out string statusDescription,
                wrapWebException: true,
                new Dictionary<string, string>());
            return result;
        }

        public GooglePlacesResponse GetPlacesNextPage(GoogleGetPlacesNextPageRequest request)
        {
            request.Key = this.Key;
            GooglePlacesResponse result = this.CallService<GooglePlacesResponse>(
                request.GetQueryString(),
                null,
                HttpVerb.GET,
                serializePostObject: false,
                _timeout,
                out HttpStatusCode statusCode,
                out string statusDescription,
                wrapWebException: true,
                new Dictionary<string, string>());
            return result;
        }

        #endregion //Methods
    }
}
