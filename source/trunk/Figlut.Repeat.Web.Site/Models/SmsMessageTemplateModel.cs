namespace Figlut.Repeat.Web.Site.Models
{
    using Figlut.Server.Toolkit.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Views;
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class SmsMessageTemplateModel
    {
        #region Sms Message Template Properties

        public Guid SmsMessageTemplateId { get; set; }

        public Guid OrganizationId { get; set; }

        public string Message { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Sms Message Template Properties

        #region Organization Properties

        public string OrganizationName { get; set; }

        #endregion //Organization Properties

        #region Settings

        public int MaxSmsSendMessageLength { get; set; }

        #endregion //Settings

        #region Methods

        public bool IsValid(out string errorMessage, int maxSmsSendMessageLength)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.Message))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SmsMessageTemplateModel>.GetPropertyName(p => p.Message, true));
            }
            else if (this.Message.Length > maxSmsSendMessageLength)
            {
                errorMessage = string.Format("{0} may not be longer {1} characters.",
                    EntityReader<SmsMessageTemplateModel>.GetPropertyName(p => p.Message, true),
                    maxSmsSendMessageLength);
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromSmsMessageTemplateView(SmsMessageTemplateView view)
        {
            this.SmsMessageTemplateId = view.SmsMessageTemplateId;
            this.OrganizationId = view.OrganizationId;
            this.Message = view.Message;
            this.DateCreated = view.DateCreated;

            this.OrganizationName = view.OrganizationName;
        }

        public void CopyPropertiesToSmsMessageTemplateView(SmsMessageTemplateView view)
        {
            view.SmsMessageTemplateId = this.SmsMessageTemplateId;
            view.OrganizationId = this.OrganizationId;
            view.Message = this.Message;
            view.DateCreated = this.DateCreated;

            view.OrganizationName = this.OrganizationName;
        }

        public void CopyPropertiesToSmsMessageTemplate(SmsMessageTemplate smsMessageTemplate)
        {
            smsMessageTemplate.SmsMessageTemplateId = this.SmsMessageTemplateId;
            smsMessageTemplate.OrganizationId = this.OrganizationId;
            smsMessageTemplate.Message = this.Message;
            smsMessageTemplate.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}