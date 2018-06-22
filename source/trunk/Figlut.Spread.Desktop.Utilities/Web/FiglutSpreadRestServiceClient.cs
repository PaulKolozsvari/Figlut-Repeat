namespace Figlut.Spread.Desktop.Utilities.Web
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Server.Toolkit.Web.Client;
    using Figlut.Server.Toolkit.Web.Client.REST;
    using System.Net;
    using Figlut.Spread.ORM;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;

    #endregion //Using Directives

    public class FiglutSpreadRestServiceClient : RestWebServiceClient
    {
        #region Constructors

        public FiglutSpreadRestServiceClient(IMimeWebServiceClient webServiceClient, int timeout)
            : base(webServiceClient, timeout)
        {
        }

        #endregion //Constructors

        #region Methods

        public User Login()
        {
            try
            {
                HttpStatusCode statusCode;
                string description = null;
                string rawOutput = null;
                User result = _webServiceClient.CallService<User>(
                    "Login",
                    null,
                    HttpVerb.GET,
                    out rawOutput,
                    false,
                    true,
                    _timeout,
                    out statusCode,
                    out description,
                    false);
                return result;
            }
            catch (WebException wex)
            {
                HttpWebResponse response = (HttpWebResponse)wex.Response;
                if (response == null)
                {
                    throw wex;
                }
                throw new UserThrownException(response.StatusDescription, LoggingLevel.Maximum);
            }
        }

        #endregion //Methods
    }
}
