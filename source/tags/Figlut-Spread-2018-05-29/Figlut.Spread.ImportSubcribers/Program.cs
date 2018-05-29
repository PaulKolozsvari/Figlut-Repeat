namespace Figlut.Spread.ImportSubcribers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Web.Client;
    using Figlut.Spread.ORM;
    using System;
    using System.Collections.Generic;
    using System.IO;
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
            string userName = "paulk";
            string password = "27830529";
            try
            {
                string webServiceBaseUrl = "http://127.0.0.1:2983/FiglutSpread";
                //string webServiceBaseUrl = "http://127.0.0.1:2983/FiglutSpread";
                JsonWebServiceClient webClient = new JsonWebServiceClient(webServiceBaseUrl);
                webClient.NetworkCredential = new NetworkCredential(userName, password);
                List<string> cellPhoneNumbers = ReadCellPhoneNumbers(@"C:\Docs\Work\FiglutSpreadRepo\customers\Motion Footwear\Randburg Custmer list.csv");

                Guid organizationId = Guid.Parse("f5729872-87e2-4271-9799-e994d1bafa2d");
                Organization organization = GetOrganizationById(organizationId, webClient, out httpStatusCode, out httpStatusDescription, out rawOutput);
                SaveSubscribers(organization, cellPhoneNumbers, webClient, out httpStatusCode, out httpStatusDescription, out rawOutput);

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                if (httpStatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine("HTTP Status Code: {0}", httpStatusCode.ToString());
                }
                if (!string.IsNullOrEmpty(httpStatusDescription))
                {
                    Console.WriteLine("HTTP Status Description: {0}", httpStatusDescription.ToString());
                }
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }

        public static Organization GetOrganizationById(
            Guid organizationId,
            JsonWebServiceClient webClient,
            out HttpStatusCode httpStatusCode,
            out string httpStatusDescription,
            out string rawOutput)
        {
            Organization result = webClient.CallService<Organization>(
                string.Format("{0}/{1}", typeof(Organization).Name, organizationId.ToString()),
                null,
                HttpVerb.GET,
                out rawOutput,
                false,
                true,
                30000,
                out httpStatusCode,
                out httpStatusDescription,
                false);
            return result;
        }


        public static Subscriber GetSubscriberByCellPhoneNumber(
            string cellPhoneNumber,
            JsonWebServiceClient webClient,
            out HttpStatusCode httpStatusCode,
            out string httpStatusDescription,
            out string rawOutput)
        {
            string queryString = string.Format("/{0}?searchBy={1}&searchValueOf={2}", typeof(Subscriber).Name, EntityReader<Subscriber>.GetPropertyName(p => p.CellPhoneNumber, false), cellPhoneNumber);
            Subscriber result = webClient.CallService<List<Subscriber>>(
                queryString,
                null,
                HttpVerb.GET,
                out rawOutput,
                false,
                true,
                30000,
                out httpStatusCode,
                out httpStatusDescription,
                false).FirstOrDefault();
            return result;
        }

        private static void SaveSubscribers(
            Organization organization,
            List<string> cellPhoneNumbers,
            JsonWebServiceClient webClient,
            out HttpStatusCode httpStatusCode,
            out string httpStatusDescription,
            out string rawOutput)
        {
            httpStatusCode = HttpStatusCode.OK;
            httpStatusDescription = null;
            rawOutput = null;

            int subscribersCreated = 0;
            foreach (string cell in cellPhoneNumbers)
            {
                Subscriber subscriber = GetSubscriberByCellPhoneNumber(cell, webClient, out httpStatusCode, out httpStatusDescription, out rawOutput);
                if (subscriber == null)
                {
                    subscriber = new Subscriber()
                    {
                        SubscriberId = Guid.NewGuid(),
                        CellPhoneNumber = cell,
                        Name = null,
                        Enabled = true,
                        DateCreated = DateTime.Now
                    };
                    Console.WriteLine("{0} Saving Subscriber: {1}", subscribersCreated, subscriber.CellPhoneNumber);
                    SaveSubscriber(subscriber, webClient, out httpStatusCode, out httpStatusDescription, out rawOutput);
                    subscribersCreated++;
                }
                else
                {
                    int stop = 0;
                }
                Subscription subscription = new Subscription()
                {
                    SubscriptionId = Guid.NewGuid(),
                    OrganizationId = organization.OrganizationId,
                    SubscriberId = subscriber.SubscriberId,
                    Enabled = true,
                    DateCreated = DateTime.Now
                };
                Console.WriteLine("Saving Subscription: {0}", subscription.SubscriptionId);
                SaveSubscription(subscription, webClient, out httpStatusCode, out httpStatusDescription, out rawOutput);
            }
        }

        private static string SaveSubscriber(
            Subscriber subscriber,
            JsonWebServiceClient webClient,
            out HttpStatusCode httpStatusCode,
            out string httpStatusDescription,
            out string rawOutput)
        {
            string resultMessage = webClient.CallService<string>(
                typeof(Subscriber).Name,
                subscriber,
                HttpVerb.POST,
                out rawOutput,
                true,
                false,
                30000,
                out httpStatusCode,
                out httpStatusDescription,
                false);
            return resultMessage;
        }

        private static string SaveSubscription(
            Subscription subscription,
            JsonWebServiceClient webClient,
            out HttpStatusCode httpStatusCode,
            out string httpStatusDescription,
            out string rawOutput)
        {
            string resultMessage = webClient.CallService<string>(
                typeof(Subscription).Name,
                subscription,
                HttpVerb.POST,
                out rawOutput,
                true,
                false,
                30000,
                out httpStatusCode,
                out httpStatusDescription,
                false);
            return resultMessage;
        }

        private static List<string> ReadCellPhoneNumbers(string filePath)
        {
            List<string> result = new List<string>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                int index = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (index == 0)
                    {
                        index++;
                        continue;
                    }
                    line = line.Trim().Replace(";", string.Empty);
                    if(!string.IsNullOrEmpty(line))
                    {
                        if (line.Length == 10)
                        {
                            result.Add(line);
                        }
                        else
                        {
                            Console.WriteLine("Line {0}: wrong cell phone number length on: {1}", index, line);
                        }
                    }
                    index++;
                }
                reader.Close();
            }
            return result;
        }

        #endregion //Methods
    }
}
