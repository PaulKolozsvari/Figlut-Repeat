namespace Figlut.Repeat.Web.Service.Configuration
{
    #region Using Directives

    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Server.Toolkit.Utilities.SettingsFile;
    using Figlut.Repeat.Email;
    using Figlut.Repeat.SMS;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class RepeatSettings : Settings
    {
        #region Constructors

        public RepeatSettings()
            : base()
        {
        }

        public RepeatSettings(string name)
            : base(name)
        {
        }

        public RepeatSettings(string name, string filePath)
            : base(name, filePath)
        {
        }

        #endregion //Constructors

        #region Properties

        #region Application

        public string ApplicationName { get; set; }

        public string ICalendarUrl { get; set; }

        #endregion //Application

        #region Sms Sending

        public bool SmsNotificationsEnabled { get; set; }

        public bool SmsDatabaseLoggingEnabled { get; set; }

        public bool TrimSmsSentLogResponseTag { get; set; }

        public int TrimSmsSentLogResponseTagLength { get; set; }

        public SmsProvider SmsProvider { get; set; }

        public string ZoomUrl { get; set; }

        public string ZoomAccountEmailAddress { get; set; }

        public string ZoomAccountToken { get; set; }

        public string ClickatellUrl { get; set; }

        public string ClickatellUser { get; set; }

        public string ClickatellPassword { get; set; }

        public string ClickatellApiId { get; set; }

        #endregion //Sms Sending

        #region Sms Processors

        public Guid SmsReceivedQueueProcessorId { get; set; }

        public Guid SmsSentQueueProcessorId { get; set; }

        public Guid SmsDeliveryReportQueueProcessorId { get; set; }

        public Guid ScheduleEntriesProcessorId { get; set; }

        #endregion //Sms Processors

        #region Email

        public bool EmailNotificationsEnabled { get; set; }

        public bool EmailDatabaseLoggingEnabled { get; set; }

        public bool EmailDatabaseLogMessageContents { get; set; }

        public bool ThrowEmailFailExceptions { get; set; }

        public EmailProvider EmailProvider { get; set; }

        public string GMailSMTPServer { get; set; }

        public string GMailSmtpUserName { get; set; }

        public string GMailSmtpPassword { get; set; }

        public int GMailSmtpPort { get; set; }

        public string SenderEmailAddress { get; set; }

        public string SenderDisplayName { get; set; }

        public bool IncludeDefaultEmailRecipients { get; set; }

        public List<EmailNotificationRecipient> DefaultEmailRecipients { get; set; }

        #endregion //Email

        #region Logging

        public bool LogToFile { get; set; }

        public bool LogToWindowsEventLog { get; set; }

        public bool LogToConsole { get; set; }

        public string LogFileName { get; set; }

        public string EventSourceName { get; set; }

        public string EventLogName { get; set; }

        public LoggingLevel LoggingLevel { get; set; }

        #endregion //Logging

        #region Database

        public string DatabaseConnectionString { get; set; }

        public int DatabaseCommandTimeout { get; set; }

        public string LinqToSQLClassesAssemblyFileName { get; set; }

        public string LinqToSQLClassesNamespace { get; set; }

        #endregion //Database

        #region Service

        public string HostAddressSuffix { get; set; }

        public int PortNumber { get; set; }

        public bool UseAuthentication { get; set; }

        public bool IncludeExceptionDetailInResponse { get; set; }

        public TextEncodingType TextResponseEncoding { get; set; }

        public long MaxBufferPoolSize { get; set; }

        public long MaxBufferSize { get; set; }

        public long MaxReceivedMessageSize { get; set; }

        public bool TraceHttpMessages { get; set; }

        public bool TraceHttpMessageHeaders { get; set; }

        public bool AuditServiceCalls { get; set; }

        #endregion //Service

        #endregion //Properties
    }
}