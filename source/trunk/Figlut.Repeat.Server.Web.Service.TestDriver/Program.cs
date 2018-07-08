namespace Figlut.Repeat.Web.Service.TestDriver
{
    #region Using Directives

    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Web;
    using Figlut.Server.Toolkit.Web.Client;
    using Figlut.Repeat.ORM;
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
            HttpStatusCode httpStatusCode = HttpStatusCode.OK;
            string httpStatusDescription = null;
            string rawOutput = null;
            try
            {
                string cellPhoneNumber = "0617410107";
                //string cellPhoneNumber = "0118882862";
                string message = "Motion Footwear! Finial days of sale on winter shoes. Up to 100% off. Shop 33 Randburg City Hill Street Mall 0714607825";
                int messageLength = message.Length;
                string userName = "DanR";
                string password = "p@ssD@nR";
                string webServiceBaseUrl = "http://www.figlut.com:2983/FiglutRepeat";
                string queryString = "/send-sms";

                JsonWebServiceClient webClient = new JsonWebServiceClient(webServiceBaseUrl);
                webClient.NetworkCredential = new NetworkCredential(userName, password);
                SmsSentLog smsResult = webClient.CallService<SmsSentLog>(
                    queryString,
                    new SmsRequest(cellPhoneNumber, message),
                    HttpVerb.POST,
                    out rawOutput,
                    true,
                    true,
                    30000,
                    out httpStatusCode,
                    out httpStatusDescription,
                    true);
                Console.WriteLine("SMS Sending Success: {0}", smsResult.Success);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("HTTP Status Code: {0}", httpStatusCode.ToString());
                Console.WriteLine("HTTP Status Description: {0}", httpStatusDescription.ToString());
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }

        //private static void SendReplyMessageToService()
        //{
        //    //string url = "http://127.0.0.1:2983/Figlut.Repeat/receive-sms-zoomconnect/&from=+27725551234&messageId=124567&message=REPLY&date=201404282055&campaign=shoe%20campaign&dataField=custom&nonce=89430dc5-cac1-4795-9dd0-46c9b8233e52&nonce-date=20140428210027&checksum=180f27d80dd05886e81da0c2ccebf830bc85c7ae";
        //    string url = "http://www.figlut.com:2983/FiglutRepeat/delivery-report-sms-zoomconnect/&from=+27725551234&messageId=124567&message=REPLY&date=201404282055&campaign=shoe%20campaign&dataField=custom&nonce=89430dc5-cac1-4795-9dd0-46c9b8233e52&nonce-date=20140428210027&checksum=180f27d80dd05886e81da0c2ccebf830bc85c7ae";
        //    WebServiceClient client = new WebServiceClient(url);
        //    Dictionary<string, string> requestHeaders = new Dictionary<string, string>();
        //    HttpStatusCode statusCode;
        //    string statusDescription = null;
        //    string result = client.CallService(
        //        string.Empty,
        //        null,
        //        HttpVerb.POST,
        //        MimeContentType.APPLICATION_JSON,
        //        60000,
        //        MimeContentType.APPLICATION_JSON,
        //        out statusCode,
        //        out statusDescription,
        //        false);
        //}

        #endregion //Methods
    }
}
