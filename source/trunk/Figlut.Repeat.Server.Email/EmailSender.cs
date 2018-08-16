namespace Figlut.Repeat.Email
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Repeat.Data;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using Figlut.Repeat.ORM.Views;
    using HtmlAgilityPack;
    using Figlut.Repeat.ORM;

    #endregion //Using Directives

    public class EmailSender
    {
        #region Constructors

        public EmailSender(
            bool emailNotificationsEnabled,
            bool emailDatabaseLoggingEnabled,
            bool emailDatabaseLogMessageContents,
            bool throwEmailFailExceptions,
            EmailProvider emailProvider,
            string server,
            string userName,
            string password,
            int port,
            string senderEmailAddress,
            string senderDisplayName,
            bool includeDefaultEmailRecipients,
            List<EmailNotificationRecipient> defaultEmailRecipients)
        {
            if (string.IsNullOrEmpty(server))
            {
                throw new NullReferenceException(string.Format("{0} not specified on {1}.",
                    EntityReader<EmailSender>.GetPropertyName(p => p.Server, true), typeof(EmailSender).Name));
            }
            if (string.IsNullOrEmpty(userName))
            {
                throw new NullReferenceException(string.Format("{0} not specified on {1}.",
                    EntityReader<EmailSender>.GetPropertyName(p => p.UserName, true), typeof(EmailSender).Name));
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new NullReferenceException(string.Format("{0} not specified on {1}.",
                    EntityReader<EmailSender>.GetPropertyName(p => p.Password, true), typeof(EmailSender).Name));
            }
            if (port < 1)
            {
                throw new NullReferenceException(string.Format("{0} may not be less than 0 on {1}.",
                    EntityReader<EmailSender>.GetPropertyName(p => p.Port, true), typeof(EmailSender).Name));
            }
            if (string.IsNullOrEmpty(senderEmailAddress))
            {
                throw new NullReferenceException(string.Format("{0} not specified on {1}.",
                    EntityReader<EmailSender>.GetPropertyName(p => p.SenderEmailAddress, true), typeof(EmailSender).Name));
            }
            if (string.IsNullOrEmpty(senderDisplayName))
            {
                throw new NullReferenceException(string.Format("{0} not specified on {1}.",
                    EntityReader<EmailSender>.GetPropertyName(p => p.SenderDisplayName, true), typeof(EmailSender).Name));
            }
            _emailNotificationsEnabled = emailNotificationsEnabled;
            _emailDatabaseLoggingEnabled = emailDatabaseLoggingEnabled;
            _emailDatabaseLogMessageContents = emailDatabaseLogMessageContents;
            _throwEmailFailExceptions = throwEmailFailExceptions;

            _emailProvider = emailProvider;
            _server = server;
            _userName = userName;
            _password = password;
            _port = port;
            _senderEmailAddress = senderEmailAddress;
            _senderDisplayName = senderDisplayName;
            _includeDefaultEmailRecipients = includeDefaultEmailRecipients;
            _defaultEmailRecipients = defaultEmailRecipients;
        }

        #endregion //Constructors

        #region Constants

        public const string DAILY_SCHEDULE_ENTRIES_HTML_EMAIL_FILE_NAME = "DailyScheduleEntriesEmail.htm";
        public const string USER_PASSWORD_RESET_EMAIL_FILE_NAME = "UserPasswordResetEmail.htm";
        public const string NEW_USER_CREATED_EMAIL_FILE_NAME = "NewUserCreated.htm";
        public const string NEW_USER_REGISTRATION_EMAIL_FILE_NAME = "NewUserRegistration.htm";
        public const string HTML_LOGO_FILE_NAME = "image002.png";

        #endregion //Constants

        #region Fields

        private bool _emailNotificationsEnabled;
        private bool _emailDatabaseLoggingEnabled;
        private bool _emailDatabaseLogMessageContents;
        private bool _throwEmailFailExceptions;

        private EmailProvider _emailProvider;
        private string _server;
        private string _userName;
        private string _password;
        private int _port;
        private string _senderEmailAddress;
        private string _senderDisplayName;
        private bool _includeDefaultEmailRecipients;
        private List<EmailNotificationRecipient> _defaultEmailRecipients;

        #endregion //Fields

        #region Properties

        public string Server
        {
            get { return _server; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public string Password
        {
            get { return _password; }
        }

        public int Port
        {
            get { return _port; }
        }

        public string SenderEmailAddress
        {
            get { return _senderEmailAddress; }
        }

        public string SenderDisplayName
        {
            get { return _senderDisplayName; }
        }

        public bool IncludeDefaultEmailRecipients
        {
            get { return _includeDefaultEmailRecipients; }
        }

        public List<EmailNotificationRecipient> DefaultEmailRecipients
        {
            get { return _defaultEmailRecipients; }
        }

        #endregion //Properties

        #region Methods

        private SmtpClient GetSmtpClient()
        {
            SmtpClient result = new SmtpClient(_server, _port);
            result.Credentials = new NetworkCredential(_userName, _password);
            result.EnableSsl = true;
            return result;
        }

        private MailMessage GetMailMessage(string subject, string body, bool isHtml)
        {
            MailMessage result = new MailMessage()
            {
                Sender = new MailAddress(_senderEmailAddress, _senderDisplayName),
                From = new MailAddress(_senderEmailAddress, _senderDisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            return result;
        }

        private void AddAttachments(
            MailMessage email,
            List<string> attachmentFileNames,
            List<MemoryStream> attachmentStreams,
            string logoImageFilePath,
            bool isHtml,
            string bodyOriginal,
            out string bodyModified)
        {
            bodyModified = bodyOriginal;
            foreach (string filePath in attachmentFileNames)
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    continue;
                }
                FileSystemHelper.ValidateFileExists(filePath);
                byte[] fileBytes = File.ReadAllBytes(filePath);
                MemoryStream ms = new MemoryStream(fileBytes);
                attachmentStreams.Add(ms);
                string fileName = Path.GetFileName(filePath);
                if (!string.IsNullOrEmpty(logoImageFilePath) && (filePath == logoImageFilePath) && isHtml)
                {
                    //: http://stackoverflow.com/questions/1212838/c-sharp-sending-mails-with-images-inline-using-smtpclient
                    //: http://blog.devexperience.net/en/12/Send_an_Email_in_CSharp_with_Inline_attachments.aspx
                    //: Try this: http://brutaldev.com/post/sending-html-e-mail-with-embedded-images-(the-correct-way)
                    string parentDirectory = Path.GetFileName(Path.GetDirectoryName(filePath));
                    LinkedResource logo = new LinkedResource(filePath);
                    logo.ContentId = fileName;
                    bodyModified = bodyOriginal.Replace("image002.png", "cid:" + logo.ContentId); //Replace the logo file name in the email body with the Content ID.
                    bodyModified = bodyModified.Replace(string.Format(@"{0}/", parentDirectory), string.Empty);
                    AlternateView view = AlternateView.CreateAlternateViewFromString(bodyModified, null, "text/html");
                    view.LinkedResources.Add(logo);
                    email.AlternateViews.Add(view);
                    continue;
                }
                Attachment attachment = new Attachment(ms, fileName, MediaTypeNames.Text.Plain);
                email.Attachments.Add(attachment);
            }
        }

        private void AddRecipientToEmail(string emailAddress, string displayName, MailMessage email, EmailRecipientType recipientType)
        {
            string emailAddressLower = emailAddress.Trim().ToLower();
            foreach (MailAddress a in email.To.ToList())
            {
                if (a.Address.Trim().ToLower() == emailAddressLower)
                {
                    return; //Email address already exists in the recipient's list of the email.
                }
            }
            switch (recipientType)
            {
                case EmailRecipientType.To:
                    email.To.Add(new MailAddress(emailAddress, displayName));
                    break;
                case EmailRecipientType.CC:
                    email.CC.Add(new MailAddress(emailAddress, displayName));
                    break;
                case EmailRecipientType.BCC:
                    email.Bcc.Add(new MailAddress(emailAddress, displayName));
                    break;
                default:
                    break;
            }
        }

        public bool SendEmail(
            EmailCategory category,
            string subject,
            string body,
            List<string> attachmentFileNames,
            bool isHtml,
            List<EmailNotificationRecipient> emailRecipients,
            string logoImageFilePath,
            Nullable<Guid> organizationId)
        {
            if (!_emailNotificationsEnabled)
            {
                return false;
            }
            try
            {
                using (SmtpClient client = GetSmtpClient())
                {
                    using (MailMessage email = GetMailMessage(subject, body, isHtml))
                    {
                        if (_includeDefaultEmailRecipients && (_defaultEmailRecipients != null))
                        {
                            _defaultEmailRecipients.ForEach(p => AddRecipientToEmail(p.EmailAddress, p.DisplayName, email, EmailRecipientType.BCC));
                        }
                        if (emailRecipients != null)
                        {
                            emailRecipients.ForEach(p => AddRecipientToEmail(p.EmailAddress, p.DisplayName, email, EmailRecipientType.To));
                        }
                        List<MemoryStream> attachmentStreams = null;
                        try
                        {
                            if (attachmentFileNames != null)
                            {
                                attachmentStreams = new List<MemoryStream>();
                                AddAttachments(email, attachmentFileNames, attachmentStreams, logoImageFilePath, isHtml, body, out body);
                            }
                            client.Send(email);
                            LogEmailNotification(email, subject);
                            LogEmailNotificationToDatabase(category.ToString(), email, subject, organizationId);
                        }
                        finally
                        {
                            if (attachmentStreams != null)
                            {
                                attachmentStreams.ForEach(p =>
                                {
                                    p.Close();
                                    p.Dispose();
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_throwEmailFailExceptions)
                {
                    throw;
                }
                ExceptionHandler.HandleException(ex);
                return false;
            }
            return true;
        }

        public bool SendExceptionEmailNotification(Exception exception)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine(exception.Message);
            if (exception.InnerException != null)
            {
                message.AppendLine(string.Format("Inner Exception : {0}", exception.InnerException.ToString()));
            }
            message.AppendLine(exception.StackTrace);
            string errorMessage = message.ToString();
            return SendEmail(EmailCategory.Error, "Figlut - Technical Error Notification", errorMessage, null, false, null, null, null);
        }

        public bool SendScheduleEntriesListEmail(
            Organization organization,
            List<User> organizationUserRecipients,
            DateTime entriesDate,
            List<ScheduleEntryView> scheduleEntries,
            string homePageUrl)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Hi,");
            message.AppendLine();
            message.AppendLine(string.Format("The following repeats are scheduled for {0} on {1}:", organization.Name, DataShaper.GetDefaultDateString(entriesDate)));
            message.AppendLine();

            int padLengthDefault = 25;
            message.Append("Customer".PadRight(padLengthDefault));
            message.Append("Schedule".PadRight(padLengthDefault));
            message.Append("Email".PadRight(padLengthDefault));
            message.Append("Cell Phone".PadRight(padLengthDefault));
            message.Append("Notification Date".PadRight(padLengthDefault));
            message.Append("Notification Day".PadRight(padLengthDefault));
            message.Append("Entry Date".PadRight(padLengthDefault));
            message.Append("Entry Day".PadRight(padLengthDefault));
            message.Append("Entry Time".PadRight(padLengthDefault));
            message.AppendLine();
            message.AppendLine();
            foreach (ScheduleEntryView e in scheduleEntries)
            {
                message.Append(e.CustomerFullName.Length <= padLengthDefault ? e.CustomerFullName.PadRight(padLengthDefault) : e.CustomerFullName.Substring(0, padLengthDefault));
                message.Append(e.ScheduleName.Length <= padLengthDefault ? e.ScheduleName.PadRight(padLengthDefault) : e.ScheduleName.Substring(0, padLengthDefault));
                message.Append(e.CustomerEmailAddress.Length <= padLengthDefault ? e.CustomerEmailAddress.PadRight(padLengthDefault) : e.CustomerEmailAddress.Substring(0, padLengthDefault));
                message.Append(e.CellPhoneNumber.Length <= padLengthDefault ? e.CellPhoneNumber.PadRight(padLengthDefault) : e.CellPhoneNumber.Substring(0, padLengthDefault));
                message.Append(DataShaper.GetDefaultDateString(e.NotificationDate).PadRight(padLengthDefault));
                message.Append(e.NotificationDateDayOfWeek.PadRight(padLengthDefault));
                message.Append(DataShaper.GetDefaultDateString(e.EntryDate).PadRight(padLengthDefault));
                message.Append(e.EntryDateDayOfWeek.PadRight(padLengthDefault));
                message.Append(e.EntryTime);
                message.AppendLine();
            }
            message.AppendLine();
            message.AppendLine(string.Format("Visit {0} to manage these Schedule Entries and send out the notifications to your subscribers.", homePageUrl));
            message.AppendLine();
            List<EmailNotificationRecipient> emailRecipients = new List<EmailNotificationRecipient>();
            organizationUserRecipients.ForEach(p => emailRecipients.Add(new EmailNotificationRecipient() { DisplayName = p.UserName, EmailAddress = p.EmailAddress }));
            return SendEmail(
                EmailCategory.DailyScheduleEntries,
                string.Format("Figlut Repeat - Schedule Entries {0}", DataShaper.GetDefaultDateString(entriesDate)),
                message.ToString(),
                null,
                false,
                emailRecipients,
                null,
                organization.OrganizationId);
        }

        public bool SendScheduleEntriesListEmailHtml(
            Organization organization,
            List<User> organizationUserRecipients,
            DateTime entriesDate,
            List<ScheduleEntryView> scheduleEntries,
            string homePageUrl,
            string dailyScheduleEntriesEmailDirectory,
            string dailyScheduleEntriesEmailFilesDirectory)
        {
            if (organization == null)
            {
                throw new Exception(string.Format("No {0} supplied for sending Schedule Entries email.", typeof(Organization).Name));
            }
            if (organizationUserRecipients == null || organizationUserRecipients.Count < 1)
            {
                throw new Exception(string.Format("No {0} {1}s supplied for sending Schedule Entries email.", typeof(Organization).Name, typeof(User).Name));
            }
            FileSystemHelper.ValidateDirectoryExists(dailyScheduleEntriesEmailDirectory);
            FileSystemHelper.ValidateDirectoryExists(dailyScheduleEntriesEmailFilesDirectory);
            string logoImageFilePath = Path.Combine(dailyScheduleEntriesEmailFilesDirectory, HTML_LOGO_FILE_NAME);
            string emailHtmlFilePath = Path.Combine(dailyScheduleEntriesEmailDirectory, DAILY_SCHEDULE_ENTRIES_HTML_EMAIL_FILE_NAME);
            FileSystemHelper.ValidateFileExists(logoImageFilePath);
            FileSystemHelper.ValidateFileExists(emailHtmlFilePath);

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load(emailHtmlFilePath);
            HtmlNode table = htmlDocument.DocumentNode.Descendants("table").FirstOrDefault();
            if (table == null)
            {
                throw new NullReferenceException(string.Format("No table node found in {0}.", emailHtmlFilePath));
            }
            List<HtmlNode> rows = table.Descendants("tr").ToList();
            if (rows.Count != 2)
            {
                throw new Exception(string.Format("Expected 2 rows but parsed {0} rows in {1}.", rows.Count, emailHtmlFilePath));
            }
            HtmlNode headerRow = rows[0];
            HtmlNode templateRow = rows[1].CloneNode(true);
            rows[1].Remove();

            HtmlNode lastRowNode = headerRow;
            foreach (ScheduleEntryView e in scheduleEntries)
            {
                HtmlNode rowNode = templateRow.CloneNode(true);
                rowNode.InnerHtml = rowNode.InnerHtml.Replace("customer", e.CustomerFullName);
                rowNode.InnerHtml = rowNode.InnerHtml.Replace("schedule", e.ScheduleName);
                rowNode.InnerHtml = rowNode.InnerHtml.Replace("email", e.CustomerEmailAddress);
                rowNode.InnerHtml = rowNode.InnerHtml.Replace("cell_phone", e.CellPhoneNumber);
                rowNode.InnerHtml = rowNode.InnerHtml.Replace("notification_date", DataShaper.GetDefaultDateString(e.NotificationDate));
                rowNode.InnerHtml = rowNode.InnerHtml.Replace("notification_day", e.NotificationDateDayOfWeek);
                rowNode.InnerHtml = rowNode.InnerHtml.Replace("entry_date", DataShaper.GetDefaultDateString(e.EntryDate));
                rowNode.InnerHtml = rowNode.InnerHtml.Replace("entry_day", e.EntryDateDayOfWeek);
                rowNode.InnerHtml = rowNode.InnerHtml.Replace("entry_time", e.EntryTime.ToString());
                table.InsertAfter(rowNode, lastRowNode);
                lastRowNode = rowNode;
                List<HtmlNode> rowsTest = table.Descendants("tr").ToList();
                int rowsCount = rowsTest.Count;
            }
            StringBuilder emailBody = new StringBuilder(htmlDocument.DocumentNode.OuterHtml);
            emailBody = emailBody.Replace("organization_name", organization.Name);
            emailBody = emailBody.Replace("entries_date", DataShaper.GetDefaultDateString(entriesDate));
            List<EmailNotificationRecipient> emailRecipients = new List<EmailNotificationRecipient>();
            organizationUserRecipients.ForEach(p => emailRecipients.Add(new EmailNotificationRecipient() { DisplayName = p.UserName, EmailAddress = p.EmailAddress }));
            string emailContents = emailBody.ToString();
            return SendEmail(
                EmailCategory.DailyScheduleEntries,
                string.Format("Figlut Repeat - Schedule Entries {0}", DataShaper.GetDefaultDateString(entriesDate)),
                emailBody.ToString(),
                new List<string>() { logoImageFilePath },
                true,
                emailRecipients,
                logoImageFilePath,
                organization.OrganizationId);
        }

        public bool SendUserResetPasswordNotification(
            string userName, 
            string emailAddress, 
            string cellPhoneNumber, 
            string newPassword,
            string organizationName,
            string homePageUrl,
            Nullable<Guid> organizationId)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine(string.Format("Hi {0},", userName));
            message.AppendLine();
            message.AppendLine("Your new Figlut Repeat password is:");
            message.AppendLine();
            message.AppendLine(newPassword);
            message.AppendLine();
            message.AppendLine(string.Format("You can change your password after logging in at: {0}", homePageUrl));
            message.AppendLine();
            message.AppendLine("Regards,");
            message.AppendLine();
            message.AppendLine("Figlut Repeat team");
            List<string> emailRecipients = new List<string>() { emailAddress };
            return SendEmail(
                EmailCategory.UserPasswordReset,
                "Figlut Repeat - Reset Password Notification",
                message.ToString(),
                null,
                false,
                new List<EmailNotificationRecipient>() { new EmailNotificationRecipient() { DisplayName = userName, EmailAddress = emailAddress } },
                null,
                organizationId);
        }

        public bool SendUserResetPasswordNotificationHtml(
            string userName,
            string emailAddress,
            string cellPhoneNumber,
            string newPassword,
            string organizationName,
            string homePageUrl,
            Nullable<Guid> organizationId,
            string userPasswordResetEmailDirectory,
            string userPasswordResetEmailFilesDirectory)
        {
            FileSystemHelper.ValidateDirectoryExists(userPasswordResetEmailDirectory);
            FileSystemHelper.ValidateDirectoryExists(userPasswordResetEmailFilesDirectory);
            string logoImageFilePath = Path.Combine(userPasswordResetEmailFilesDirectory, HTML_LOGO_FILE_NAME);
            string emailHtmlFilePath = Path.Combine(userPasswordResetEmailDirectory, USER_PASSWORD_RESET_EMAIL_FILE_NAME);
            FileSystemHelper.ValidateFileExists(logoImageFilePath);
            FileSystemHelper.ValidateFileExists(emailHtmlFilePath);

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load(emailHtmlFilePath);
            StringBuilder emailBody = new StringBuilder(htmlDocument.DocumentNode.OuterHtml);

            emailBody = emailBody.Replace("user_name", userName);
            emailBody = emailBody.Replace("password_value", newPassword);
            emailBody = emailBody.Replace("home_page", homePageUrl);
            string emailContents = emailBody.ToString();
            List<string> emailRecipients = new List<string>() { emailAddress };
            return SendEmail(
                EmailCategory.UserPasswordReset,
                "Figlut Repeat - Reset Password Notification",
                emailBody.ToString(),
                new List<string>() { logoImageFilePath },
                true,
                new List<EmailNotificationRecipient>() { new EmailNotificationRecipient() { DisplayName = userName, EmailAddress = emailAddress } },
                logoImageFilePath,
                organizationId);
        }

        public bool SendUserRegistrationEmailNotification(
            string organizationEmailAddress, 
            string organizationName,
            string userEmailAddress,
            string userName,
            Nullable<Guid> organizationId,
            string homePageUrl)
        {
            List<EmailNotificationRecipient> recipients = new List<EmailNotificationRecipient>()
            {
                new EmailNotificationRecipient() { EmailAddress = organizationEmailAddress, DisplayName = organizationName },
                new EmailNotificationRecipient() { EmailAddress = userEmailAddress, DisplayName = userName }
            };
            string subject = "Welcome to Figlut Repeat";
            StringBuilder body = new StringBuilder();
            body.AppendLine(string.Format("Hi {0},", userName));
            body.AppendLine();
            body.AppendLine("Welcome to Figlut Repeat!");
            body.AppendLine("Someone will be in contact with you shortly to help you get started.");    
            body.AppendLine();
            body.AppendLine(string.Format("To access your account visit {0}.", homePageUrl));
            body.AppendLine("Regards,");
            body.AppendLine();
            body.AppendLine("The Figlut Repeat Team");
            return SendEmail(
                EmailCategory.NewUserRegistration, 
                subject, 
                body.ToString(), 
                null, 
                false, 
                recipients, 
                null, organizationId);
        }

        public bool SendUserRegistrationEmailNotificationHtml(
            string organizationEmailAddress,
            string organizationName,
            string userEmailAddress,
            string userName,
            string homePageUrl,
            Nullable<Guid> organizationId,
            string newUserRegistrationEmailDirectory,
            string newUserRegistrationEmailFilesDirectory)
        {
            FileSystemHelper.ValidateDirectoryExists(newUserRegistrationEmailDirectory);
            FileSystemHelper.ValidateDirectoryExists(newUserRegistrationEmailFilesDirectory);
            string logoImageFilePath = Path.Combine(newUserRegistrationEmailFilesDirectory, HTML_LOGO_FILE_NAME);
            string emailHtmlFilePath = Path.Combine(newUserRegistrationEmailDirectory, NEW_USER_REGISTRATION_EMAIL_FILE_NAME);
            FileSystemHelper.ValidateFileExists(logoImageFilePath);
            FileSystemHelper.ValidateFileExists(emailHtmlFilePath);

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load(emailHtmlFilePath);
            StringBuilder emailBody = new StringBuilder(htmlDocument.DocumentNode.OuterHtml);

            emailBody = emailBody.Replace("user_name", userName);
            emailBody = emailBody.Replace("home_page", homePageUrl);
            string emailContents = emailBody.ToString();
            List<string> emailRecipients = new List<string>() { userEmailAddress };
            return SendEmail(
                EmailCategory.NewUserRegistration,
                "Figlut Repeat - Welcome",
                emailBody.ToString(),
                new List<string>() { logoImageFilePath },
                true,
                new List<EmailNotificationRecipient>() { new EmailNotificationRecipient() { DisplayName = userName, EmailAddress = userEmailAddress } },
                logoImageFilePath,
                organizationId);
        }

        public bool SendUserCreatedEmail(
            string createdByUserName,
            string userName,
            string emailAddress,
            string cellPhoneNumber,
            string newPassword,
            string organizationName,
            string homePageUrl,
            Nullable<Guid> organizationId)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine(string.Format("Hi {0},", userName));
            message.AppendLine();
            message.AppendLine("Welcome to Figlut Repeat!");
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(organizationName))
            {
                message.AppendLine(string.Format("{0} has added you as a user to the organization {1}.", createdByUserName, organizationName));
            }
            message.AppendLine(string.Format("To access your account visit {0} and login with the following username and password:", homePageUrl));
            message.AppendLine();
            message.AppendLine(string.Format("Username: {0}", userName));
            message.AppendLine(string.Format("Password: {0}", newPassword));
            message.AppendLine();
            message.AppendLine(string.Format("You can change your password after logging in at: {0}", homePageUrl));
            message.AppendLine();
            message.AppendLine("Regards,");
            message.AppendLine();
            message.AppendLine("Figlut Repeat team");
            return SendEmail(
                EmailCategory.NewUserCreated,
                "Figlut Repeat - Welcome",
                message.ToString(),
                null,
                false,
                new List<EmailNotificationRecipient>() { new EmailNotificationRecipient() { DisplayName = userName, EmailAddress = emailAddress } },
                null,
                organizationId);
        }

        public bool SendUserCreatedEmailHtml(
            string createdByUserName,
            string userName,
            string emailAddress,
            string cellPhoneNumber,
            string newPassword,
            string organizationName,
            string homePageUrl,
            Nullable<Guid> organizationId,
            string newUserCreatedEmailDirectory,
            string newUserCreatedEmailFilesDirectory)
        {
            FileSystemHelper.ValidateDirectoryExists(newUserCreatedEmailDirectory);
            FileSystemHelper.ValidateDirectoryExists(newUserCreatedEmailFilesDirectory);
            string logoImageFilePath = Path.Combine(newUserCreatedEmailFilesDirectory, HTML_LOGO_FILE_NAME);
            string emailHtmlFilePath = Path.Combine(newUserCreatedEmailDirectory, NEW_USER_CREATED_EMAIL_FILE_NAME);
            FileSystemHelper.ValidateFileExists(logoImageFilePath);
            FileSystemHelper.ValidateFileExists(emailHtmlFilePath);

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load(emailHtmlFilePath);
            StringBuilder emailBody = new StringBuilder(htmlDocument.DocumentNode.OuterHtml);
            
            emailBody = emailBody.Replace("organization_admin_user_name", createdByUserName);
            emailBody = emailBody.Replace("organization_name", organizationName);
            emailBody = emailBody.Replace("home_page", homePageUrl);
            emailBody = emailBody.Replace("user_name", userName);
            emailBody = emailBody.Replace("password_value", newPassword);
            string emailContents = emailBody.ToString();
            List<string> emailRecipients = new List<string>() { emailAddress };
            return SendEmail(
                EmailCategory.NewUserCreated,
                "Figlut Repeat - Welcome",
                emailBody.ToString(),
                new List<string>() { logoImageFilePath },
                true,
                new List<EmailNotificationRecipient>() { new EmailNotificationRecipient() { DisplayName = userName, EmailAddress = emailAddress } },
                logoImageFilePath,
                organizationId);
        }

        private void LogEmailNotification(MailMessage email, string subject)
        {
            StringBuilder logMessage = new StringBuilder();
            logMessage.AppendLine("Email notification Sent:");
            logMessage.AppendLine(string.Format("Subject: {0}", subject));
            email.To.ToList().ForEach(p => logMessage.AppendLine(p.Address));
            string logMessageText = logMessage.ToString();
            GOC.Instance.Logger.LogMessage(new LogMessage(logMessageText, LogMessageType.Information, LoggingLevel.Maximum));
        }

        private string GetEmailRecipientsCsv(MailMessage email)
        {
            StringBuilder result = new StringBuilder();
            foreach (MailAddress a in email.To.ToList())
            {
                result.Append(string.Format("{0},", a.Address));
            }
            if (result.Length > 0)
            {
                result = result.Remove(result.Length - 1, 1);
            }
            return result.ToString();
        }

        private bool LogEmailNotificationToDatabase(string category, MailMessage email, string subject, Nullable<Guid> organizationId)
        {
            if (!_emailDatabaseLoggingEnabled)
            {
                return false;
            }
            string emailRecipientsCsv = GetEmailRecipientsCsv(email);
            string emailContents = _emailDatabaseLogMessageContents ? email.Body : null;
            RepeatEntityContext context = RepeatEntityContext.Create();
            foreach (MailAddress a in email.To.ToList()) //Log each recipient as a separate EmailLog entry.
            {
                context.LogEmailNotification(
                    0,
                    null,
                    category,
                    a.Address,
                    subject,
                    emailContents,
                    null,
                    null,
                    emailRecipientsCsv,
                    (int)_emailProvider,
                    organizationId);
            }
            return true;
        }

        #endregion //Methods
    }
}
