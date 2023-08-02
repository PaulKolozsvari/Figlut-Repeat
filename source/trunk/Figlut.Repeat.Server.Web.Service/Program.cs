namespace Figlut.Repeat.Web.Service
{
    #region Using Directives

    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.Email;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.SMS;
    using Figlut.Repeat.Web.Service.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Server.Toolkit.Data.iCalendar;
    using Figlut.Google.Places;
    using Figlut.Google.Places.Responses.List;
    using Figlut.Google.Places.Requests;
    using Figlut.Google.Places.Responses;
    using Figlut.Server.Toolkit.Data;
    using System.IO;

    #endregion //Using Directives

    static class Program
    {
        #region Constants

        private const string HELP_ARGUMENT = "/help";
        private const string HELP_QUESTION_MARK_ARGUMENT = "/?";
        private const string RESET_SETTINGS_ARGUMENT = "/reset_settings";
        private const string TEST_MODE_ARGUMENT = "/start";
        private const string SEND_SMS = "/send_sms";
        private const string DOWNLOAD_COUNTRY_PUBLIC_HOLIDAYS = "/download_country_public_holidays";
        private const string DOWNLOAD_ALL_COUNTRIES_PUBLIC_HOLIDAYS = "/download_all_countries_public_holidays";
        private const string IMPORT_GOOGLE_PLACES_LEADS = "/import_google_places_leads";

        #endregion //Constants

        #region Fields

        private static bool _testMode;

        #endregion //Fields

        #region Methods

        private static bool ParseArguments(string[] args)
        {
            string countryCode = null;
            string countryName = null;
            string yearString = null;
            int year = 0;
            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i];
                string aLower = a.ToLower();
                switch (aLower)
                {
                    case HELP_ARGUMENT:
                        DisplayHelp();
                        return false;
                    case HELP_QUESTION_MARK_ARGUMENT:
                        DisplayHelp();
                        return false;
                    case RESET_SETTINGS_ARGUMENT:
                        ResetSettings();
                        return false;
                    case TEST_MODE_ARGUMENT:
                        _testMode = true;
                        return true;
                    case SEND_SMS:
                        if (args.Length < (i + 3))
                        {
                            throw new ArgumentException(string.Format(
                                "{0} requires two extra parameters: recipient number followed by the message e.g. {0} 0821235432 \"Hello world.\"", 
                                SEND_SMS));
                        }
                        string recipientNumber = args[i + 1];
                        string message = args[i + 2];
                        SendSms(true, recipientNumber, message);
                        return false;
                    case DOWNLOAD_COUNTRY_PUBLIC_HOLIDAYS:
                        if (args.Length < i + 3)
                        {
                            throw new ArgumentException(string.Format(
                                @"{0} requires 4 additional parameters: country Code, country Name and year e.g. /{0} zaf 'South Africa' 2018",
                                DOWNLOAD_COUNTRY_PUBLIC_HOLIDAYS));
                        }
                        countryCode = args[i + 1];
                        countryName = args[i + 2];
                        yearString = args[i + 3];
                        if (!int.TryParse(yearString, out year))
                        {
                            throw new InvalidCastException(string.Format("Could not parse {0} to an integer for the year parameter.", yearString));
                        }
                        DownloadCountryPublicHolidays(true, countryCode, countryName, year);
                        return false;
                    case DOWNLOAD_ALL_COUNTRIES_PUBLIC_HOLIDAYS:
                        if (args.Length < i + 1)
                        {
                            throw new ArgumentException(string.Format(
                                @"{0} requires 1 additional parameter: year e.g. /{0} 'South Africa' 2018",
                                DOWNLOAD_ALL_COUNTRIES_PUBLIC_HOLIDAYS));
                        }
                        yearString = args[i + 1];
                        if (!int.TryParse(yearString, out year))
                        {
                            throw new InvalidCastException(string.Format("Could not parse {0} to an integer for the year parameter.", yearString));
                        }
                        DownloadAllCountriesPublicHolidays(true, year);
                        return false;
                    case IMPORT_GOOGLE_PLACES_LEADS:
                        ImportGooglePlacesLeads(true);
                        return false;
                    default:
                        throw new ArgumentException(string.Format("Invalid argument '{0}'.", a));
                }
            }
            return true;
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("*** Figlut.Repeat REST API Usage ***");
            Console.WriteLine();
            Console.WriteLine("{0} or {1} : Display usage (service is not started).", HELP_ARGUMENT, HELP_QUESTION_MARK_ARGUMENT);
            Console.WriteLine("{0} : Resets the service's settings file with the default settings (server is not started).", RESET_SETTINGS_ARGUMENT);
            Console.WriteLine("{0} : Starts the service as a console application instead of a windows service.", TEST_MODE_ARGUMENT);
            Console.WriteLine("{0} : Sends an sms to the specified number with the specified message e.g. {0} 0821235432 \"Hello world.\"", SEND_SMS);
            Console.WriteLine("{0} : Downloads a .ics iCalendar file for a specific country and year e.g. {0} zaf South Africa 2018", DOWNLOAD_COUNTRY_PUBLIC_HOLIDAYS);
            Console.WriteLine("{0} : Downloads a .ics iCalendar file for all countries in the database for a specific year e.g. {0} 2018", DOWNLOAD_ALL_COUNTRIES_PUBLIC_HOLIDAYS);
            Console.WriteLine("{0} : Imports leads from Google Places e.g. {0} 2018", DOWNLOAD_ALL_COUNTRIES_PUBLIC_HOLIDAYS);
            Console.WriteLine();
            Console.WriteLine("N.B. Executing without any parameters runs the server as a windows service.");
        }

        private static void ResetSettings()
        {
            RepeatSettings s = new RepeatSettings()
            {
                ShowMessageBoxOnException = false,
                ApplicationName = "Figlut.Repeat REST API",
                SmsNotificationsEnabled = false,
                SmsDatabaseLoggingEnabled = true,
                SmsProvider = SmsProvider.Zoom,
                ZoomUrl = "https://zoomconnect.com/zoom/api/rest/v1/sms/send",
                ZoomAccountEmailAddress = "enter email address here",
                ZoomAccountToken = "enter accoun token here",
                ClickatellUrl = "http://api.clickatell.com/http/sendmsg",
                ClickatellUser = "enter user name here",
                ClickatellPassword = "enter password here",
                ClickatellApiId = "enter API ID here",
                EmailNotificationsEnabled = false,
                EmailDatabaseLoggingEnabled = true,
                EmailDatabaseLogMessageContents = true,
                EmailProvider = EmailProvider.GMail,
                GMailSMTPServer = "smtp.gmail.com",
                GMailSmtpUserName = "enter email address here",
                GMailSmtpPassword = "enter password here",
                GMailSmtpPort = 587,
                SenderEmailAddress = "notifications@Figlut.Repeat.com",
                SenderDisplayName = "Figlut.Repeat Notification",
                IncludeDefaultEmailRecipients = true,
                DefaultEmailRecipients = new List<EmailNotificationRecipient>()
                {
                    new EmailNotificationRecipient() { EmailAddress = "paul.kolozsvari@binarychef.com", DisplayName = "Paul Kolozsvari" },
                    new EmailNotificationRecipient() { EmailAddress  = "pkolozsvari@gmail.com", DisplayName = "Paul Kolozsvari" }
                },

                LogToFile = false,
                LogToWindowsEventLog = true,
                LogToConsole = true,
                LogFileName = "Figlut.Repeat.Log.txt",
                EventSourceName = "Figlut.Repeat.Source",
                EventLogName = "Figlut.Repeat.Log",
                LoggingLevel = LoggingLevel.Normal,
                DatabaseConnectionString = "<Enter DB connection string here>",
                DatabaseCommandTimeout = 30000,
                LinqToSQLClassesAssemblyFileName = "Figlut.Repeat.Server.ORM.dll",
                LinqToSQLClassesNamespace = "Figlut.Repeat.Server.ORM",
                HostAddressSuffix = "Figlut.Repeat",
                PortNumber = 2983,
                UseAuthentication = false,
                IncludeExceptionDetailInResponse = false,
                MaxBufferPoolSize = 2147483647,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
            };
            Console.Write("Reset settings file {0}, are you sure (Y/N)?", s.FilePath);
            string response = Console.ReadLine();
            if (response.Trim().ToLower() != "y")
            {
                return;
            }
            s.SaveToFile();
            Console.WriteLine("Settings file reset successfully.");
            Console.Read();
        }

        private static void SendSms(bool initializeService, string recipientNumber, string message)
        {
            if (initializeService)
            {
                RepeatService.Start(false);
            }
            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Sending SMS to {0}: {1}", recipientNumber, message), LogMessageType.Information, LoggingLevel.Maximum));
            SmsResponse smsResponse = RepeatApp.Instance.SmsSender.SendSms(new SmsRequest(recipientNumber, message, 130, null, null, null));
            if (smsResponse != null)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine(string.Format("Sms Response to {0}", recipientNumber));
                logMessage.AppendLine(smsResponse.ToString());
                GOC.Instance.Logger.LogMessage(new LogMessage(logMessage.ToString(), LogMessageType.SuccessAudit, LoggingLevel.Maximum));
            }
            RepeatApp.Instance.LogSmsSentToDB(
                recipientNumber,
                message,
                smsResponse,
                null,
                null,
                true,
                null,
                null,
                null,
                null);
        }

        private static void DownloadAllCountriesPublicHolidays(bool initializeService, int year)
        {
            if (initializeService)
            {
                RepeatService.Start(false);
            }
            GOC.Instance.Logger.LogMessage(new LogMessage("Querying countries in database.", LogMessageType.Information, LoggingLevel.Maximum));
            RepeatEntityContext context = RepeatEntityContext.Create();
            List<Country> countries = context.GetCountriesByFilter(string.Empty);
            foreach (Country c in countries)
            {
                DownloadCountryPublicHolidays(false, c.CountryCode, c.CountryName, year);
            }
        }

        private static void DownloadCountryPublicHolidays(bool initializeService, string countryCode, string countryName, int year)
        {
            if (initializeService)
            {
                RepeatService.Start(false);
            }
            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Downloading {0} public holidays calendar for {1}.", year, countryName), LogMessageType.Information, LoggingLevel.Maximum));
            ICalCalendar calendar = RepeatApp.Instance.CalendarDownloader.DownloadICalCalendar(countryCode, countryName, year, null, true);

            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Saving {0} public holidays calendar to database for {1}.", year, countryName), LogMessageType.Information, LoggingLevel.Maximum));
            RepeatEntityContext context = RepeatEntityContext.Create();
            context.SavePublicHolidaysFromICalCalendar(calendar);

            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Downloaded and saved {0} public holidays calendar successfully for {1}.", year, countryName), LogMessageType.SuccessAudit, LoggingLevel.Normal));
        }

        private static void ImportGooglePlacesLeads(bool initializeService)
        {
            if (initializeService)
            {
                RepeatService.Start(false);
            }
            int radius = 50000;
            List<GooglePlaceCentre> centres = new List<GooglePlaceCentre>()
            {
                new GooglePlaceCentre("Viro Solutions Cape Town", -33.9652741174112, 18.59194739700472, radius),
                new GooglePlaceCentre("Waterfront Cape Town", -33.90674584713001, 18.42266177001582, radius),
                new GooglePlaceCentre("Glen Marais", -26.079597, 28.247506, radius),
                new GooglePlaceCentre("Victory Park", -26.13724554839237, 28.009024645981018, radius),
                new GooglePlaceCentre("Sandton City", -26.10863472758328, 28.052730525562627, radius),
                new GooglePlaceCentre("Clearwater Mall Roodeport", -26.122994582651142, 27.903901241533937, radius),
                new GooglePlaceCentre("Melville", -26.17669497260382, 27.997583069815622, radius),
                new GooglePlaceCentre("East Rand Mall Boksburg", -26.181537847699772, 28.240395212071544, radius),
                new GooglePlaceCentre("Bedfordview", -26.176624536574266, 28.13697606229055, radius),
                new GooglePlaceCentre("Bryanston", -26.075343008367188, 28.027774135582316, radius),
                new GooglePlaceCentre("Greenstone", -26.118463112657444, 28.141951140905014, radius),
            };

            EntityCache<Guid, GooglePlaceInfo> cache = new EntityCache<Guid, GooglePlaceInfo>();
            GooglePlacesClient client = new GooglePlacesClient("https://maps.googleapis.com/maps/api/place", 30000, key: "AIzaSyD-PwB2sxuK31DnRYXEJO0l58xrzScZzkc");
            foreach (var centre in centres)
            {
                GOC.Instance.Logger.LogMessage(new LogMessage($"Getting places in {centre.Name} in {radius} radius ...", LogMessageType.Information, LoggingLevel.Normal));
                GooglePlacesResponse placesResponse = client.GetPlaces(new GoogleGetPlacesRequest(centre.Latitude, centre.Longitude, type: "restaurant", null, radius));
                GOC.Instance.Logger.LogMessage(new LogMessage($"Downloaded {placesResponse.results.Length} places ...", LogMessageType.SuccessAudit, LoggingLevel.Normal));
                foreach (var place in placesResponse.results)
                {
                    GooglePlaceInfo placeInfo = new GooglePlaceInfo(centre);
                    ParseGooglePlacesResult(client, place, placeInfo);
                    cache.Add(Guid.NewGuid(), placeInfo);
                }
                int pageIndex = 2;
                while (!string.IsNullOrEmpty(placesResponse.next_page_token))
                {
                    GOC.Instance.Logger.LogMessage(new LogMessage($"Getting places in page {pageIndex} of {centre.Name}  in {radius} radius ...", LogMessageType.Information, LoggingLevel.Normal));
                    placesResponse = client.GetPlacesNextPage(new GoogleGetPlacesNextPageRequest(pageToken: placesResponse.next_page_token));
                    GOC.Instance.Logger.LogMessage(new LogMessage($"Downloaded {placesResponse.results.Length} places in page {pageIndex} ...", LogMessageType.SuccessAudit, LoggingLevel.Normal));
                    foreach (var place in placesResponse.results)
                    {
                        GooglePlaceInfo placeInfo = new GooglePlaceInfo(centre);
                        ParseGooglePlacesResult(client, place, placeInfo);
                        cache.Add(Guid.NewGuid(), placeInfo);
                    }
                }
            }
            string filePath = @"C:\Docs\temp\Restaurants.csv";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            cache.ExportToCsv(filePath, null, false, false);
        }

        private static void ParseGooglePlacesResult(
            GooglePlacesClient client,
            Figlut.Google.Places.Responses.List.Result place,
            GooglePlaceInfo placeInfo)
        {
            placeInfo.PlaceId = place.place_id;
            placeInfo.Name = place.name;
            placeInfo.Latitude = place.geometry.location.lat;
            placeInfo.Longitude = place.geometry.location.lng;
            place.vicinity = place.vicinity;
            GooglePlaceDetailResponse detailResponse = GetGooglePlaceDetails(client, placeInfo.PlaceId, placeInfo);
        }

        private static GooglePlaceDetailResponse GetGooglePlaceDetails(GooglePlacesClient client, string placeId, GooglePlaceInfo placeInfo)
        {
            GOC.Instance.Logger.LogMessage(new LogMessage($"Getting details place {placeInfo.Name} ...", LogMessageType.Information, LoggingLevel.Normal));
            GooglePlaceDetailResponse detailResponse = client.GetPlaceDetail(new GoogleGetPlaceDetailsRequest(placeId));
            if (detailResponse.result == null)
            {
                return null;
            }
            placeInfo.PhoneNumber = detailResponse.result.formatted_phone_number;
            placeInfo.InternationalPhoneNumber = detailResponse.result.international_phone_number;
            placeInfo.IsMobilePhoneNumber = !IsInternationPhoneNumberLandline(placeInfo.InternationalPhoneNumber);
            placeInfo.Website = detailResponse.result.website;
            return detailResponse;
        }

        public static bool IsInternationPhoneNumberLandline(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }
            phoneNumber = phoneNumber.Trim().Replace(" ", string.Empty);
            if (phoneNumber.Contains("+2721") ||
                phoneNumber.Contains("+2711"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                ConsoleApplication.Maximize();
                if (!ParseArguments(args))
                {
                    return;
                }
                if (_testMode)
                {
                    Console.WriteLine(string.Format("Starting {0} ... ", RepeatApp.Instance.Settings.ApplicationName));
                    RepeatService.Start(true);

                    Console.WriteLine();
                    Console.WriteLine("Initialization complete.");
                    Console.WriteLine("Press any key to stop the service.");
                    Console.ReadLine();

                    RepeatService.Stop();
                    return;
                }
                ServiceBase.Run(new ServiceBase[] { new RepeatService() });
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                throw ex;
            }
        }

        #endregion //Methods
    }
}
