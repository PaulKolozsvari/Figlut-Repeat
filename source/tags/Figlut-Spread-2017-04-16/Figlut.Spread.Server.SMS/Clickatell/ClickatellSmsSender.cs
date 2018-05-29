namespace Figlut.Spread.SMS.Clickatell
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class ClickatellSmsSender : SmsSender
    {
        #region Constructors

        public ClickatellSmsSender(string url, string user, string password, string apiId, bool smsNotificationsEnabled) : base(SmsProvider.Clickatell)
        {
            _url = url;
            _user = user;
            _password = password;
            _apiId = apiId;
            _smsNotificationsEnabled = smsNotificationsEnabled;
        }

        #endregion //Constructors

        #region Fields

        private string _url;
        private string _user;
        private string _password;
        private string _apiId;

        #endregion //Fields

        #region Properties

        /// <summary>
        /// The base URL of the Clickatell REST API.
        /// </summary>
        public string Url
        {
            get { return _url; }
        }

        /// <summary>
        /// The username used to login to the Clickatell account.
        /// </summary>
        public string User
        {
            get { return _user; }
        }

        /// <summary>
        /// The password used to login to the Clickatell account.
        /// </summary>
        public string Password
        {
            get { return _password; }
        }

        /// <summary>
        /// The API ID linked to the Clickatell account.
        /// </summary>
        public string ApiId
        {
            get { return _apiId; }
        }

        #endregion //Properties

        #region Constants

        private string MOZILA_FIRE_FOX_USER_AGENT_HEADER = "Mozilla/4.0(compatible; MSIE 6.0; Windows NT 5.2; .NET CLR1.0.3705;)";

        private const string USER_QUERY_STRING_NAME = "user";
        private const string PASSWORD_QUERY_STRING_NAME = "password";
        private const string API_ID_QUERY_STRING_NAME = "api_id";
        private const string TO_QUERY_STRING_NAME = "to";
        private const string TEXT_QUERY_STRING_NAME = "text";

        private const string ID_RESPONSE_PREFIX = "ID:";
        private const string ERROR_RESPONSE_PREFIX = "ERR:";

        #endregion //Constants

        #region Methods

        public override SmsResponse SendSms(SmsRequest request)
        {
            if (!_smsNotificationsEnabled)
            {
                return null;
            }
            string responseText = null;
            using (WebClient client = new WebClient())
            {
                // Add a user agent header in case the requested URI contains a  query.
                client.Headers.Add("user-agent", MOZILA_FIRE_FOX_USER_AGENT_HEADER);

                client.QueryString.Add(USER_QUERY_STRING_NAME, _user);
                client.QueryString.Add(PASSWORD_QUERY_STRING_NAME, _password);
                client.QueryString.Add(API_ID_QUERY_STRING_NAME, _apiId);
                client.QueryString.Add(TO_QUERY_STRING_NAME, request.recipientNumber);
                client.QueryString.Add(TEXT_QUERY_STRING_NAME, request.message);
                //string baseurl = "http://api.clickatell.com/http/sendmsg";
                //string directUrl = "http://api.clickatell.com/http/sendmsg?user=paulkolo&password=XMRDMMgTCbERFP&api_id=3531847&to=0833958283&text=Message";
                using (Stream stream = client.OpenRead(_url))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        responseText = reader.ReadToEnd();
                        stream.Close();
                        reader.Close();
                    }
                }
            }
            ClickatellSmsResponse result = GetResponseFromResponseText(responseText);
            result.smsProvider = SmsProvider.Clickatell;
            return result;
        }

        private ClickatellSmsResponse GetResponseFromResponseText(string responseText)
        {
            if (string.IsNullOrEmpty(responseText))
            {
                throw new NullReferenceException("Clickatell: null or empty response received from Clickatell.");
            }
            if (responseText.StartsWith(ID_RESPONSE_PREFIX))
            {
                responseText = responseText.Replace(ID_RESPONSE_PREFIX, string.Empty).Trim();
                string messageId = responseText;
                return new ClickatellSmsResponse(true, messageId, null, null);
            }
            else if (responseText.StartsWith(ERROR_RESPONSE_PREFIX))
            {
                responseText = responseText.Replace(ERROR_RESPONSE_PREFIX, string.Empty);
                string[] errorParams = responseText.Split(',');
                if (errorParams.Length != 2)
                {
                    throw new ArgumentException(string.Format("Clickatell: Expected error message response to have 2 parameters: '{0}'", responseText));
                }
                string errorCode = errorParams[0].Trim();
                int errorCodeInt;
                if (!int.TryParse(errorCode, out errorCodeInt))
                {
                    throw new ArgumentException(string.Format("Clickatell: Could not parse error code from '{0}'.", responseText));
                }
                string errorMessage = errorParams[1];
                return new ClickatellSmsResponse(false, null, errorMessage, errorCode);
            }
            else
            {
                throw new ArgumentException(string.Format("Clickatell: Unexpected response: '{0}'", responseText));
            }
        }

        #endregion //Methods
    }
}
