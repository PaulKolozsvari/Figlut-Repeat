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

                    LinkedResource logo = new LinkedResource(filePath);
                    logo.ContentId = Guid.NewGuid().ToString();
                    bodyModified = bodyOriginal.Replace(fileName, "cid:" + logo.ContentId); //Replace the logo file name in the email body with the Content ID.
                    AlternateView view = AlternateView.CreateAlternateViewFromString(bodyModified, null, "text/html");
                    view.LinkedResources.Add(logo);
                    email.AlternateViews.Add(view);
                    continue;
                }
                Attachment attachment = new Attachment(ms, fileName, MediaTypeNames.Text.Plain);
                email.Attachments.Add(attachment);
            }
        }

        private void AddRecipientToEmail(string emailAddress, string displayName, MailMessage email)
        {
            string emailAddressLower = emailAddress.Trim().ToLower();
            foreach (MailAddress a in email.To.ToList())
            {
                if (a.Address.Trim().ToLower() == emailAddressLower)
                {
                    return; //Email address already exists in the recipient's list of the email.
                }
            }
            email.To.Add(new MailAddress(emailAddress, displayName));
        }

        public bool SendEmail(
            string subject,
            string body,
            List<string> attachmentFileNames,
            bool isHtml,
            List<EmailNotificationRecipient> emailRecipients,
            string logoImageFilePath)
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
                            _defaultEmailRecipients.ForEach(p => AddRecipientToEmail(p.EmailAddress, p.DisplayName, email));
                        }
                        if (emailRecipients != null)
                        {
                            emailRecipients.ForEach(p => AddRecipientToEmail(p.EmailAddress, p.DisplayName, email));
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
                            LogEmailNotificationToDatabase(email, subject);
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
            return SendEmail("Figlut - Technical Error Notification", errorMessage, null, false, null, null);
        }

        public bool SendUserResetPasswordNotification(
            string userName, 
            string emailAddress, 
            string cellPhoneNumber, 
            string newPassword,
            string organizationName,
            string homePageUrl)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine(string.Format("Hi {0},", userName));
            message.AppendLine();
            message.AppendLine("Your new Figlut password is:");
            message.AppendLine();
            message.AppendLine(newPassword);
            message.AppendLine();
            message.AppendLine(string.Format("You can change your password after logging in at: {0}", homePageUrl));
            message.AppendLine();
            message.AppendLine("Regards,");
            message.AppendLine();
            message.AppendLine("Figlut team");
            List<string> emailRecipients = new List<string>() { emailAddress };
            return SendEmail(
                "Figlut - Reset Password Notification", 
                message.ToString(), 
                null, 
                false, 
                new List<EmailNotificationRecipient>() { new EmailNotificationRecipient() { DisplayName = userName, EmailAddress = emailAddress } },
                null);
        }

        public bool SendUserCreatedWelcomeEmail(
            string currentUserName,
            string userName,
            string emailAddress,
            string cellPhoneNumber,
            string newPassword,
            string organizationName,
            string homePageUrl)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine(string.Format("Hi {0},", userName));
            message.AppendLine();
            message.AppendLine("Welcome to Figlut!");
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(organizationName))
            {
                message.AppendLine(string.Format("{0} has added you as a user to the organization {1}.", currentUserName, organizationName));
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
            message.AppendLine("Figlut team");
            List<string> emailRecipients = new List<string>() { emailAddress };
            return SendEmail(
                "Figlut - Welcome",
                message.ToString(),
                null,
                false,
                new List<EmailNotificationRecipient>() { new EmailNotificationRecipient() { DisplayName = userName, EmailAddress = emailAddress } },
                null);
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

        private bool LogEmailNotificationToDatabase(MailMessage email, string subject)
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
                    a.Address,
                    emailContents,
                    null,
                    null,
                    emailRecipientsCsv,
                    (int)_emailProvider);
            }
            return true;
        }

        #endregion //Methods
    }
}
