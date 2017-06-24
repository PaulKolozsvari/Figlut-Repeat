namespace Figlut.Spread.Web.Service.Configuration
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Spread.Email;
    using Figlut.Spread.Web.Service.REST;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Spread.ORM;
    using Figlut.Spread.SMS;
    using Figlut.Spread.SMS.Zoom;
    using Figlut.Spread.SMS.Clickatell;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM.Helpers;
using Figlut.Spread.SMS.Processor;
    using Figlut.Server.Toolkit.Web.Service.Inspector;

    #endregion //Using Directives

    public class SpreadApp
    {
        #region Singleton Setup

        /* Singleton pattern for web applications http://www.alexzaitzev.pro/2013/02/singleton-pattern.html 
         * 	Using the volatile keyword for the singleton instance object:
         * 	https://msdn.microsoft.com/en-us/library/x13ttww7.aspx
         * 	http://www.c-sharpcorner.com/UploadFile/1d42da/volatile-keyword-in-C-Sharp-threading/
         */

        private static volatile SpreadApp _instance;
        private static object _syncRoot = new object();

        public static SpreadApp Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot) //Ensures only a single thread can enter the critical area at a given time when no instances of the singleton have been created.
                    {
                        if (_instance == null)
                        {
                            _instance = new SpreadApp();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion //Singleton Setup

        #region Fields

        private SpreadSettings _settings;
        private Dictionary<GlobalSettingName, GlobalSetting> _globalSettings;
        private SmsSender _smsSender;
        private EmailSender _emailSender;

        #region SMS Processors

        private SmsSentQueueProcessor _smsSentQueueProcessor;

        #endregion //SMS Processors

        #endregion //Fields

        #region Properties

        public SpreadSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = GOC.Instance.GetSettings<SpreadSettings>(true, true);
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

        #endregion //Properties

        #region Methods

        internal void Initialize(
            bool initializeServiceHost)
        {
            SpreadSettings settings = Settings;
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
            GOC.Instance.SetEncoding(settings.TextResponseEncoding);

            LinqFunnelSettings linqFunnelSettings = new LinqFunnelSettings(settings.DatabaseConnectionString, settings.DatabaseCommandTimeout);
            GOC.Instance.AddByTypeName(linqFunnelSettings); //Adds an object to Global Object Cache with the key being the name of the type.
            string linqToSqlAssemblyFilePath = Path.Combine(Information.GetExecutingDirectory(), settings.LinqToSQLClassesAssemblyFileName);
            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Attemping to load {0}", settings.LinqToSQLClassesAssemblyFileName), LogMessageType.SuccessAudit, LoggingLevel.Maximum));

            //Grab the LinqToSql context from the specified assembly and load it in the GOC to be used from anywhere in the application.
            //The point of doing this is that you can rebuild the context if the schema changes and reload without having to reinstall the web service. You just stop the service and overwrite the EOH.RainMaker.ORM.dll with the new one.
            //It also allows the Figlut Service Toolkit to be business data agnostic.
            GOC.Instance.LinqToClassesAssembly = Assembly.LoadFrom(linqToSqlAssemblyFilePath);
            GOC.Instance.LinqToSQLClassesNamespace = settings.LinqToSQLClassesNamespace;
            GOC.Instance.SetLinqToSqlDataContextType<FiglutSpreadDataContext>(); //This is the wrapper context that can also call the Rain Maker specific sprocs.

            if (initializeServiceHost)
            {
                InitializeServiceHost(settings);
            }
            InitializeSmsSentQueueProcessor(true);
            GOC.Instance.Logger.LogMessage(new LogMessage("Application Initialized.", LogMessageType.SuccessAudit, LoggingLevel.Normal));
        }

        private void InitializeServiceHost(SpreadSettings settings)
        {
            WebHttpBinding binding = new WebHttpBinding()
            {
                MaxBufferPoolSize = settings.MaxBufferPoolSize,
                MaxBufferSize = Convert.ToInt32(settings.MaxBufferSize),
                MaxReceivedMessageSize = settings.MaxReceivedMessageSize
            };
            if (settings.UseAuthentication)
            {
                binding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            }
            ServiceHost serviceHost = new ServiceHost(typeof(SpreadRestService));
            string address = string.Format("http://127.0.0.1:{0}/{1}", settings.PortNumber, settings.HostAddressSuffix);
            ServiceDebugBehavior debugBehaviour = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (debugBehaviour == null) //This should never be, but just in case.
            {
                debugBehaviour = new ServiceDebugBehavior();
                serviceHost.Description.Behaviors.Add(debugBehaviour);
            }
            debugBehaviour.IncludeExceptionDetailInFaults = settings.IncludeExceptionDetailInResponse;
            ServiceEndpoint httpEndpoint = serviceHost.AddServiceEndpoint(typeof(ISpreadRestService), binding, address);
            httpEndpoint.Behaviors.Add(new WebHttpBehavior());
            httpEndpoint.EndpointBehaviors.Add(new ServiceMessageInspectorBehavior(settings.TraceHttpMessages, settings.TraceHttpMessageHeaders));
            if (settings.UseAuthentication)
            {
                serviceHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = System.ServiceModel.Security.UserNamePasswordValidationMode.Custom;
                serviceHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new UserValidator();
            }
            if (GOC.Instance.GetByTypeName<ServiceHost>() != null)
            {
                GOC.Instance.DeleteByTypeName<ServiceHost>();
            }
            GOC.Instance.AddByTypeName(serviceHost); //The service's stop method will access it from the GOC to close the service host.
            serviceHost.Open();

            GOC.Instance.Logger.LogMessage(new LogMessage(
                string.Format("{0} started at address {1}", settings.ApplicationName, address),
                LogMessageType.SuccessAudit,
                LoggingLevel.Minimum));
        }

        private void InitializeSmsSender(SpreadSettings settings)
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
                        EntityReader<SpreadSettings>.GetPropertyName(p => p.SmsProvider, true),
                        settings.SmsProvider.ToString()));
            }
        }

        private void InitializeEmailSender(SpreadSettings settings)
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

        private void InitializeSmsSentQueueProcessor(bool startImmediately)
        {
            string organizationIdentifierIndicator = GlobalSettings[GlobalSettingName.OrganizationIdentifierIndicator].SettingValue;
            string subscriberNameIndicator = GlobalSettings[GlobalSettingName.SubscriberNameIndicator].SettingValue;
            int maxSmsSendMessageLength = Convert.ToInt32(GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
            string smsSendMessageSuffix = GlobalSettings[GlobalSettingName.SmsSendMessageSuffix].SettingValue;
            int organizationIdentifierMaxLength = Convert.ToInt32(GlobalSettings[GlobalSettingName.OrganizationIdentifierMaxLength].SettingValue);
            SmsProcessor processor = SpreadEntityContext.Create().GetSmsProcessor(Settings.SmsSentQueueProcessorId, true);
            _smsSentQueueProcessor = new SmsSentQueueProcessor(
                this.SmsSender,
                maxSmsSendMessageLength,
                smsSendMessageSuffix,
                organizationIdentifierMaxLength,
                processor.SmsProcessorId,
                processor.ExecutionInterval,
                startImmediately,
                organizationIdentifierIndicator,
                subscriberNameIndicator,
                this.EmailSender);
        }

        public SmsSentLog LogSmsSentToDB(
            string recipientNumber, 
            string message, 
            SmsResponse smsResponse, 
            User senderUser,
            bool beforeCreditsDeduction)
        {
            if (smsResponse == null || !Settings.SmsDatabaseLoggingEnabled)
            {
                return null;
            }
            SmsSentLog result = SpreadEntityContext.Create().LogSmsSent(
                recipientNumber,
                message,
                smsResponse.success,
                smsResponse.errorCode,
                smsResponse.error,
                smsResponse.messageId,
                message,
                (int)smsResponse.smsProvider,
                senderUser,
                beforeCreditsDeduction);
            return result;
        }

        public SmsReceivedQueueItem EnqueueSmsReceived(
            string fromValue,
            string messageIdValue,
            string messageValue,
            string dateValue,
            string campaignValue,
            string dataFieldValue,
            string nonceValue,
            string noncedateValue,
            string checksumValue,
            SmsProvider smsProvider)
        {
            if (!Settings.SmsDatabaseLoggingEnabled)
            {
                return null;
            }
            SmsReceivedQueueItem result = SpreadEntityContext.Create().EnqueueSmsReceived(
                fromValue,
                messageIdValue,
                messageValue,
                dateValue,
                campaignValue,
                dataFieldValue,
                nonceValue,
                noncedateValue,
                checksumValue,
                (int)smsProvider);
            return result;
        }

        public SmsDeliveryReportLog LogSmsDeliveryReportToDB(
             string fromValue,
             string messageIdValue,
             string messageValue,
             string dateValue,
             string campaignValue,
             string dataFieldValue,
             string nonceValue,
             string noncedateValue,
             string checksumValue,
             SmsProvider smsProvider)
        {
            if (string.IsNullOrEmpty(messageIdValue) || !Settings.SmsDatabaseLoggingEnabled)
            {
                return null;
            }
            SpreadEntityContext context = SpreadEntityContext.Create();
            SmsDeliveryReportLog result = context.LogSmsDeliveryReport(
                fromValue,
                messageIdValue,
                messageValue,
                dateValue,
                campaignValue,
                dataFieldValue,
                nonceValue,
                noncedateValue,
                checksumValue,
                (int)smsProvider);
            if (result != null)
            {
                context.FlagSmsSentAsDelivered(messageIdValue, false);
            }
            return result;
        }

        public void RefreshGlobalSettingsFromDatabase()
        {
            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Initializing {0}s.", typeof(GlobalSetting).Name), LogMessageType.SuccessAudit, LoggingLevel.Maximum));
            List<GlobalSetting> globalSettingsList = SpreadEntityContext.Create().GetAllGlobalSettings();
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

        #endregion //Methods
    }
}