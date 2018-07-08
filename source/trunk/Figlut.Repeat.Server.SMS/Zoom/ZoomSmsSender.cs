namespace Figlut.Repeat.SMS.Zoom
{
    #region Using Directives

    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Web;
    using Figlut.Server.Toolkit.Web.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    /// <summary>
    /// Utility for sending a request to ZoomConnect.com for an SMS to be sent.
    /// </summary>
    public class ZoomSmsSender : SmsSender
    {
        #region Constructors

        public ZoomSmsSender(string url, string accountEmailAddress, string accountToken, bool smsNotificationsEnabled) : base(SmsProvider.Zoom)
        {
            _url = url;
            _accountEmailAddress = accountEmailAddress;
            _accountToken = accountToken;
            _smsNotificationsEnabled = smsNotificationsEnabled;
        }

        #endregion //Constructors

        #region Fields

        private string _url;
        private string _accountEmailAddress;
        private string _accountToken;

        #endregion //Fields

        #region Properties

        /// <summary>
        /// The REST URL to send the SMS request to.
        /// </summary>
        public string Url
        {
            get { return _url; }
        }

        /// <summary>
        /// The email addressed used to login to ZoomConnect.com.
        /// </summary>
        public string AccountEmailAddress
        {
            get { return _accountEmailAddress; }
        }

        /// <summary>
        /// The developer token generated on ZoomConnect.com.
        /// </summary>
        public string AccountToken
        {
            get { return _accountToken; }
        }

        #endregion //Properties

        #region Constants

        private const string EMAIL_REQUEST_HEADER_NAME = "email";
        private const string TOKEN_REQUEST_HEADER_NAME = "token";

        #endregion //Constants

        #region Methods

        public override SmsResponse SendSms(SmsRequest request)
        {
            if (!_smsNotificationsEnabled)
            {
                return null;
            }
            WebServiceClient client = new WebServiceClient(_url);
            Dictionary<string, string> requestHeaders = new Dictionary<string, string>();
            requestHeaders.Add(EMAIL_REQUEST_HEADER_NAME, _accountEmailAddress);
            requestHeaders.Add(TOKEN_REQUEST_HEADER_NAME, _accountToken);
            string textOutput = null;
            HttpStatusCode statusCode;
            string statusDescription = null;
            //The Zoom web service only populates the messageId and error fields.
            ZoomSmsResponse result = client.CallService<ZoomSmsResponse>(
                string.Empty,
                request,
                HttpVerb.POST,
                out textOutput,
                30000,
                GOC.Instance.JsonSerializer,
                null,
                MimeContentType.APPLICATION_JSON,
                MimeContentType.APPLICATION_JSON,
                out statusCode,
                out statusDescription,
                false,
                requestHeaders);
            result.smsProvider = SmsProvider.Zoom;
            if (string.IsNullOrEmpty(result.error))
            {
                result.success = true;
            }
            return result;
        }

        #endregion //Methods
    }
}
