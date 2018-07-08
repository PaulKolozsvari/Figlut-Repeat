namespace Figlut.Repeat.Web.Site.Configuration
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Server.Toolkit.Data.iCalendar;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Server.Toolkit.Web.Client;
    using Figlut.Server.Toolkit.Web.Client.IP_API;
    using Figlut.Server.Toolkit.Web.Client.REST;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.Email;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Helpers;
    using Figlut.Repeat.SMS;
    using Figlut.Repeat.SMS.Clickatell;
    using Figlut.Repeat.SMS.Zoom;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    #endregion //Using Directives

    public class RepeatWebApp
    {
        #region Singleton Setup

        /* Singleton pattern for web applications http://www.alexzaitzev.pro/2013/02/singleton-pattern.html 
         * 	Using the volatile keyword for the singleton instance object:
         * 	https://msdn.microsoft.com/en-us/library/x13ttww7.aspx
         * 	http://www.c-sharpcorner.com/UploadFile/1d42da/volatile-keyword-in-C-Sharp-threading/
         */

        private static volatile RepeatWebApp _instance;
        private static object _syncRoot = new object();

        public static RepeatWebApp Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new RepeatWebApp();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion //Singleton Setup

        #region Constructors

        private RepeatWebApp()
        {
        }

        #endregion //Constructors

        #region Fields

        private RepeatWebSettings _settings;
        private Dictionary<GlobalSettingName, GlobalSetting> _globalSettings;
        private SmsSender _smsSender;
        private EmailSender _emailSender;
        private IP_API_WebServiceClientXml _whoIsWebClient;
        private ICalCalendarDownloader _calendarDownloader;

        #endregion //Fields

        #region Properties

        public RepeatWebSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = GOC.Instance.GetSettings<RepeatWebSettings>(true, true);
                }
                return _settings;
            }
        }

        public Dictionary<GlobalSettingName, GlobalSetting> GlobalSettings
        {
            get
            {
                if (_globalSettings == null)
                {
                    RefreshGlobalSettingsFromDatabase();
                }
                return _globalSettings;
            }
        }

        public SmsSender SmsSender
        {
            get
            {
                if (_smsSender == null)
                {
                    InitializeSmsSender(Settings);
                }
                return _smsSender;
            }
        }

        public EmailSender EmailSender
        {
            get
            {
                if (_emailSender == null)
                {
                    InitializeEmailSender(Settings);
                }
                return _emailSender;
            }
        }

        public IP_API_WebServiceClientXml WhoIsWebClient
        {
            get
            {
                if (_whoIsWebClient == null)
                {
                    InitializeWhoIsWebClient();
                }
                return _whoIsWebClient;
            }
        }

        public ICalCalendarDownloader CalendarDownloader
        {
            get
            {
                if (_calendarDownloader == null)
                {
                    _calendarDownloader = new ICalCalendarDownloader(Settings.ICalendarUrl);
                }
                return _calendarDownloader;
            }
        }

        #endregion //Properties

        #region Methods

        private void InitializeWhoIsWebClient()
        {
            string whoIsWebServiceUrl = GlobalSettings[GlobalSettingName.WhoIsWebServiceUrl].SettingValue;
            _whoIsWebClient = new IP_API_WebServiceClientXml(whoIsWebServiceUrl);
        }

        public void Initialize()
        {
            RepeatWebSettings settings = Settings;
            GOC.Instance.ApplicationName = settings.ApplicationName;

            GOC.Instance.Logger = new Logger(//Creates a global Logger to be used throughout the application to be stored in the Global Object Cache which is a singleton.
                settings.LogToFile,
                settings.LogToWindowsEventLog,
                settings.LogToConsole,
                settings.LoggingLevel,
                settings.LogFileName,
                settings.EventSourceName,
                settings.EventLogName);
            GOC.Instance.JsonSerializer.IncludeOrmTypeNamesInJsonResponse = true;

            LinqFunnelSettings linqFunnelSettings = new LinqFunnelSettings(settings.DatabaseConnectionString, settings.DatabaseCommandTimeout);
            GOC.Instance.AddByTypeName(linqFunnelSettings); //Adds an object to Global Object Cache with the key being the name of the type.
            string linqToSqlAssemblyFilePath = Path.Combine(Information.GetExecutingDirectory(), settings.LinqToSQLClassesAssemblyFileName);
            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Attemping to load {0}", settings.LinqToSQLClassesAssemblyFileName), LogMessageType.SuccessAudit, LoggingLevel.Maximum));

            //Grab the LinqToSql context from the specified assembly and load it in the GOC to be used from anywhere in the application.
            //The point of doing this is that you can rebuild the context if the schema changes and reload without having to reinstall the web service. You just stop the service and overwrite the EOH.RainMaker.ORM.dll with the new one.
            //It also allows the Figlut Service Toolkit to be business data agnostic.
            GOC.Instance.LinqToClassesAssembly = Assembly.LoadFrom(linqToSqlAssemblyFilePath);
            GOC.Instance.LinqToSQLClassesNamespace = settings.LinqToSQLClassesNamespace;
            GOC.Instance.SetLinqToSqlDataContextType<FiglutRepeatDataContext>(); //This is the wrapper context that can also call the Rain Maker specific sprocs.

            RefreshGlobalSettingsFromDatabase();

            GOC.Instance.Logger.LogMessage(new LogMessage("Application Initialized.", LogMessageType.SuccessAudit, LoggingLevel.Normal));
        }

        public void RefreshGlobalSettingsFromDatabase()
        {
            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Initializing {0}s.", typeof(GlobalSetting).Name), LogMessageType.SuccessAudit, LoggingLevel.Maximum));
            List<GlobalSetting> globalSettingsList = RepeatEntityContext.Create().GetAllGlobalSettings();
            if (_globalSettings == null)
            {
                _globalSettings = new Dictionary<GlobalSettingName, GlobalSetting>();
            }
            _globalSettings.Clear();
            foreach (GlobalSetting g in globalSettingsList)
            {
                GlobalSettingName globalSettingName;
                if (!Enum.TryParse<GlobalSettingName>(g.SettingName, out globalSettingName))
                {
                    throw new Exception(string.Format("Could not parse {0} of '{1}' to a valid {2}.",
                        EntityReader<GlobalSetting>.GetPropertyName(p => p.SettingName, false),
                        g.SettingName,
                        typeof(GlobalSettingName).Name));
                }
                if (_globalSettings.ContainsKey(globalSettingName))
                {
                    throw new Exception(string.Format("More than one {0} with the {1} of '{2}'.",
                        typeof(GlobalSetting).Name,
                        EntityReader<GlobalSetting>.GetPropertyName(p => p.SettingName, false),
                        g.SettingName));
                }
                _globalSettings.Add(globalSettingName, g);
            }
            Array enumValues = EnumHelper.GetEnumValues(typeof(GlobalSettingName));
            foreach (GlobalSettingName n in enumValues)
            {
                if (!_globalSettings.ContainsKey(n))
                {
                    throw new Exception(string.Format("Could not find {0} for {1}.",
                        typeof(GlobalSetting).Name,
                        n.ToString()));
                }
            }
        }

        private void InitializeSmsSender(RepeatWebSettings settings)
        {
            switch (settings.SmsProvider)
            {
                case SmsProvider.Zoom:
                    _smsSender = new ZoomSmsSender(
                        settings.ZoomUrl,
                        settings.ZoomAccountEmailAddress,
                        settings.ZoomAccountToken,
                        settings.SmsNotificationsEnabled);
                    break;
                case SmsProvider.Clickatell:
                    _smsSender = new ClickatellSmsSender(
                        settings.ClickatellUrl,
                        settings.ClickatellUser,
                        settings.ClickatellPassword,
                        settings.ClickatellApiId,
                        settings.SmsNotificationsEnabled);
                    break;
                default:
                    throw new ArgumentException(string.Format("{0} '{1}' not supported.",
                        EntityReader<RepeatWebSettings>.GetPropertyName(p => p.SmsProvider, true),
                        settings.SmsProvider.ToString()));
            }
        }

        private void InitializeEmailSender(RepeatWebSettings settings)
        {
            _emailSender = new EmailSender(
                settings.EmailNotificationsEnabled,
                settings.EmailDatabaseLoggingEnabled,
                settings.EmailDatabaseLogMessageContents,
                settings.ThrowEmailFailExceptions,
                settings.EmailProvider,
                settings.GMailSMTPServer,
                settings.GMailSmtpUserName,
                settings.GMailSmtpPassword,
                settings.GMailSmtpPort,
                settings.SenderEmailAddress,
                settings.SenderDisplayName,
                settings.IncludeDefaultEmailRecipients,
                settings.DefaultEmailRecipients);
        }

        public SmsSentLog LogSmsSentToDB(
            string recipientNumber, 
            string message, 
            SmsResponse smsResponse, 
            User senderUser,
            bool beforeCreditsDeduction,
            Nullable<Guid> subscriberId,
            string subscriberName,
            string smsCampaignName,
            Nullable<Guid> smsCampaignId)
        {
            if (smsResponse == null || !Settings.SmsDatabaseLoggingEnabled)
            {
                return null;
            }
            string messageContents = Settings.SmsDatabaseLogMessageContents ? message : null;
            return RepeatEntityContext.Create().LogSmsSent(
                recipientNumber,
                messageContents,
                smsResponse.success,
                smsResponse.errorCode,
                smsResponse.error,
                smsResponse.messageId,
                message,
                (int)smsResponse.smsProvider,
                senderUser,
                beforeCreditsDeduction,
                subscriberId,
                subscriberName,
                smsCampaignName,
                smsCampaignId);
        }

        #endregion //Methods
    }
}