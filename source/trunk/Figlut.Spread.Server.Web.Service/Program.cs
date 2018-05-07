namespace Figlut.Spread.Web.Service
{
    using Figlut.Server.Toolkit.Data.iCalendar;
    #region Using Directives

    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Spread.Data;
    using Figlut.Spread.Email;
    using Figlut.Spread.ORM;
    using Figlut.Spread.SMS;
    using Figlut.Spread.Web.Service.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;

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
                    default:
                        throw new ArgumentException(string.Format("Invalid argument '{0}'.", a));
                }
            }
            return true;
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("*** Figlut.Spread REST API Usage ***");
            Console.WriteLine();
            Console.WriteLine("{0} or {1} : Display usage (service is not started).", HELP_ARGUMENT, HELP_QUESTION_MARK_ARGUMENT);
            Console.WriteLine("{0} : Resets the service's settings file with the default settings (server is not started).", RESET_SETTINGS_ARGUMENT);
            Console.WriteLine("{0} : Starts the service as a console application instead of a windows service.", TEST_MODE_ARGUMENT);
            Console.WriteLine("{0} : Sends an sms to the specified number with the specified message e.g. {0} 0821235432 \"Hello world.\"", SEND_SMS);
            Console.WriteLine("{0} : Downloads a .ics iCalendar file for a specific country and year e.g. {0} zaf South Africa 2018", DOWNLOAD_COUNTRY_PUBLIC_HOLIDAYS);
            Console.WriteLine("{0} : Downloads a .ics iCalendar file for all countries in the database for a specific year e.g. {0} 2018", DOWNLOAD_ALL_COUNTRIES_PUBLIC_HOLIDAYS);
            Console.WriteLine();
            Console.WriteLine("N.B. Executing without any parameters runs the server as a windows service.");
        }

        private static void ResetSettings()
        {
            SpreadSettings s = new SpreadSettings()
            {
                ShowMessageBoxOnException = false,
                ApplicationName = "Figlut.Spread REST API",
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
                SenderEmailAddress = "notifications@Figlut.Spread.com",
                SenderDisplayName = "Figlut.Spread Notification",
                IncludeDefaultEmailRecipients = true,
                DefaultEmailRecipients = new List<EmailNotificationRecipient>()
                {
                    new EmailNotificationRecipient() { EmailAddress = "paul.kolozsvari@binarychef.com", DisplayName = "Paul Kolozsvari" },
                    new EmailNotificationRecipient() { EmailAddress  = "pkolozsvari@gmail.com", DisplayName = "Paul Kolozsvari" }
                },

                LogToFile = false,
                LogToWindowsEventLog = true,
                LogToConsole = true,
                LogFileName = "Figlut.Spread.Log.txt",
                EventSourceName = "Figlut.Spread.Source",
                EventLogName = "Figlut.Spread.Log",
                LoggingLevel = LoggingLevel.Normal,
                DatabaseConnectionString = "<Enter DB connection string here>",
                DatabaseCommandTimeout = 30000,
                LinqToSQLClassesAssemblyFileName = "Figlut.Spread.Server.ORM.dll",
                LinqToSQLClassesNamespace = "Figlut.Spread.Server.ORM",
                HostAddressSuffix = "Figlut.Spread",
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
                SpreadService.Start(false);
            }
            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Sending SMS to {0}: {1}", recipientNumber, message), LogMessageType.Information, LoggingLevel.Maximum));
            SmsResponse smsResponse = SpreadApp.Instance.SmsSender.SendSms(new SmsRequest(recipientNumber, message, 130, null, null, null));
            if (smsResponse != null)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine(string.Format("Sms Response to {0}", recipientNumber));
                logMessage.AppendLine(smsResponse.ToString());
                GOC.Instance.Logger.LogMessage(new LogMessage(logMessage.ToString(), LogMessageType.SuccessAudit, LoggingLevel.Maximum));
            }
            SpreadApp.Instance.LogSmsSentToDB(recipientNumber, message, smsResponse, null, true);
        }

        private static void DownloadAllCountriesPublicHolidays(bool initializeService, int year)
        {
            if (initializeService)
            {
                SpreadService.Start(false);
            }
            GOC.Instance.Logger.LogMessage(new LogMessage("Querying countries in database.", LogMessageType.Information, LoggingLevel.Maximum));
            SpreadEntityContext context = SpreadEntityContext.Create();
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
                SpreadService.Start(false);
            }
            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Downloading public holidays calendar for {0}.", countryName), LogMessageType.Information, LoggingLevel.Maximum));
            ICalCalendar calendar = SpreadApp.Instance.CalendarDownloader.DownloadICalCalendar(countryCode, countryName, year, null, true);

            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Saving public holidays calendar to database for {0}.", countryName), LogMessageType.Information, LoggingLevel.Maximum));
            SpreadEntityContext context = SpreadEntityContext.Create();
            context.SavePublicHolidaysFromICalCalendar(calendar);

            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Downloaded and saved public holidays calendar successfully for {0}.", countryName), LogMessageType.SuccessAudit, LoggingLevel.Normal));
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
                    Console.WriteLine(string.Format("Starting {0} ... ", SpreadApp.Instance.Settings.ApplicationName));
                    SpreadService.Start(true);

                    Console.WriteLine();
                    Console.WriteLine("Initialization complete.");
                    Console.WriteLine("Press any key to stop the service.");
                    Console.ReadLine();

                    SpreadService.Stop();
                    return;
                }
                ServiceBase.Run(new ServiceBase[] { new SpreadService() });
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
