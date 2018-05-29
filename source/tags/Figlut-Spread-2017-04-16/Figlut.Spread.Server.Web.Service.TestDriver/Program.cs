namespace Figlut.Spread.Server.Web.Service.TestDriver
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

    class Program
    {
        #region Methods

        static void Main(string[] args)
        {
            try
            {
                SendReplyMessageToService();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        private static void SendReplyMessageToService()
        {
            //string url = "http://127.0.0.1:2983/Figlut.Spread/receive-sms-zoomconnect/&from=+27725551234&messageId=124567&message=REPLY&date=201404282055&campaign=shoe%20campaign&dataField=custom&nonce=89430dc5-cac1-4795-9dd0-46c9b8233e52&nonce-date=20140428210027&checksum=180f27d80dd05886e81da0c2ccebf830bc85c7ae";
            string url = "http://www.figlut.com:2983/FiglutSpread/delivery-report-sms-zoomconnect/&from=+27725551234&messageId=124567&message=REPLY&date=201404282055&campaign=shoe%20campaign&dataField=custom&nonce=89430dc5-cac1-4795-9dd0-46c9b8233e52&nonce-date=20140428210027&checksum=180f27d80dd05886e81da0c2ccebf830bc85c7ae";
            WebServiceClient client = new WebServiceClient(url);
            Dictionary<string, string> requestHeaders = new Dictionary<string, string>();
            HttpStatusCode statusCode;
            string statusDescription = null;
            string result = client.CallService(
                string.Empty,
                null,
                HttpVerb.POST,
                MimeContentType.APPLICATION_JSON,
                60000,
                MimeContentType.APPLICATION_JSON,
                out statusCode,
                out statusDescription,
                false);
        }

        #endregion //Methods
    }
}
