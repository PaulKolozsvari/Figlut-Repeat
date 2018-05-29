namespace Figlut.Spread.Email
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
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

        private void LogEmailNotification(MailMessage email, string subject)
        {
            StringBuilder logMessage = new StringBuilder();
            logMessage.AppendLine("Email notification Sent:");
            logMessage.AppendLine(string.Format("Subject: {0}", subject));
            email.To.ToList().ForEach(p => logMessage.AppendLine(p.Address));
            string logMessageText = logMessage.ToString();
            GOC.Instance.Logger.LogMessage(new LogMessage(logMessageText, LogMessageType.Information, LoggingLevel.Maximum));
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

        public void SendEmail(
            string subject,
            string body,
            List<string> attachmentFileNames,
            bool isHtml,
            List<EmailNotificationRecipient> emailRecipients,
            string logoImageFilePath)
        {
            using (SmtpClient client = GetSmtpClient())
            {
                using (MailMessage email = GetMailMessage(subject, body, isHtml))
                {
                    if (_includeDefaultEmailRecipients && (_defaultEmailRecipients != null))
                    {
                        _defaultEmailRecipients.ForEach(p => email.To.Add(new MailAddress(p.EmailAddress, p.DisplayName)));
                    }
                    if(emailRecipients != null)
                    {
                        emailRecipients.ForEach(p => email.To.Add(new MailAddress(p.EmailAddress, p.DisplayName)));
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
                    }
                    finally
                    {
                        if(attachmentStreams != null)
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

        #endregion //Methods
    }
}
