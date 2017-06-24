namespace Figlut.Spread.Receiver.Web.Service
{
    #region Using Directives

    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Spread.Email;
    using Figlut.Spread.Receiver.Web.Service.Configuration;
    using Figlut.Spread.SMS;
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
            Console.WriteLine();
            Console.WriteLine("N.B. Executing without any parameters runs the server as a windows service.");
        }

        private static void ResetSettings()
        {
            SpreadReceiverSettings s = new SpreadReceiverSettings()
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
                    Console.WriteLine(string.Format("Starting {0} ... ", SpreadReceiverApp.Instance.Settings.ApplicationName));
                    SpreadReceiverService.Start(true);

                    Console.WriteLine();
                    Console.WriteLine("Initialization complete.");
                    Console.WriteLine("Press any key to stop the service.");
                    Console.ReadLine();

                    SpreadReceiverService.Stop();
                    return;
                }
                ServiceBase.Run(new ServiceBase[] { new SpreadReceiverService() });
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
