﻿namespace Figlut.Repeat.Web.Site.Configuration
{
    #region Using Directives

    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Server.Toolkit.Utilities.SettingsFile;
    using Figlut.Repeat.Email;
    using Figlut.Repeat.SMS;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class RepeatWebSettings : Settings
    {
        #region Constructors

        public RepeatWebSettings()
            : base()
        {
        }

        public RepeatWebSettings(string filePath)
            : base(filePath)
        {
        }

        public RepeatWebSettings(string name, string filePath)
            : base(name, filePath)
        {
        }

        #endregion //Constructors

        #region Properties

        #region Application

        public string ApplicationName { get; set; }

        public string ICalendarUrl { get; set; }

        public string HowitWorksImageGalleryDirectory { get; set; }

        public string WhyItWorksImageGalleryDirectory { get; set; }

        #endregion //Application

        #region Sms

        public bool SmsNotificationsEnabled { get; set; }

        public bool SmsDatabaseLoggingEnabled { get; set; }

        public bool SmsDatabaseLogMessageContents { get; set; }

        public SmsProvider SmsProvider { get; set; }

        public string ZoomUrl { get; set; }

        public string ZoomAccountEmailAddress { get; set; }

        public string ZoomAccountToken { get; set; }

        public string ClickatellUrl { get; set; }

        public string ClickatellUser { get; set; }

        public string ClickatellPassword { get; set; }

        public string ClickatellApiId { get; set; }

        #endregion //Sms

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

        public string DailyScheduleEntriesEmailDirectory { get; set; }

        public string DailyScheduleEntriesEmailFilesDirectory { get; set; }

        public string NewUserCreatedEmailDirectory { get; set; }

        public string NewUserCreatedEmailFilesDirectory { get; set; }

        public string NewUserRegistrationEmailDirectory { get; set; }

        public string NewUserRegistrationEmailFilesDirectory { get; set; }

        public string UserPasswordResetEmailDirectory { get; set; }

        public string UserPasswordResetEmailFilesDirectory { get; set; }

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

        #region PayFast

        public string MerchantId { get; set; }

        public string MerchantKey { get; set; }

        public string PassPhrase { get; set; }

        public string ProcessUrl { get; set; }

        public string ValidateUrl { get; set; }

        public string ReturnUrl { get; set; }

        public string CancelUrl { get; set; }

        public string NotifyUrl { get; set; }

        #endregion //PayFast

        #endregion //Properties
    }
}