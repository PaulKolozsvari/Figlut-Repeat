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
        private const string DOWNLOAD_PUBLIC_HOLIDAYS = "/download_public_holidays";

        #endregion //Constants

        #region Fields

        private static bool _testMode;

        #endregion //Fields

        #region Methods

        private static bool ParseArguments(string[] args)
        {
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
                        SendSms(recipientNumber, message);
                        return false;
                    case DOWNLOAD_PUBLIC_HOLIDAYS:
                        if (args.Length < i + 4)
                        {
                            throw new ArgumentException(string.Format(
                                @"{0} requires 4 additional parameters: country Code, country Name, start date and end date e.g. /download_icalendar zaf 'South Africa' 01-01-2018 31-12-2018",
                                DOWNLOAD_PUBLIC_HOLIDAYS));
                        }
                        string countryCode = args[i + 1];
                        string countryName = args[i + 2];
                        string startDate = args[i + 3];
                        string endDate = args[i + 4];
                        DownloadPublicHolidays(countryCode, countryName, startDate, endDate);
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
            Console.WriteLine("{0} : Downloads a .ics iCalendar file for a specific country and date range e.g. {0} zaf South Africa 01-01-2018 31-12-2018", DOWNLOAD_PUBLIC_HOLIDAYS);
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

        private static void SendSms(string recipientNumber, string message)
        {
            SpreadService.Start(false);
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

        private static void DownloadPublicHolidays(string countryCode, string countryName, string startDate, string endDate)
        {
            SpreadService.Start(false);
            GOC.Instance.Logger.LogMessage(new LogMessage("Downloading public holidays Calendar.", LogMessageType.Information, LoggingLevel.Maximum));
            ICalCalendar calendar = SpreadApp.Instance.CalendarDownloader.DownloadICalCalendar(countryCode, countryName, startDate, endDate, null, true);

            GOC.Instance.Logger.LogMessage(new LogMessage("Saving public holidays calendar to database.", LogMessageType.Information, LoggingLevel.Maximum));
            SpreadEntityContext context = SpreadEntityContext.Create();
            context.SavePublicHolidaysFromICalCalendar(calendar);

            GOC.Instance.Logger.LogMessage(new LogMessage("Downloaded and saved public holidays calendar successfully.", LogMessageType.SuccessAudit, LoggingLevel.Normal));
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
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
