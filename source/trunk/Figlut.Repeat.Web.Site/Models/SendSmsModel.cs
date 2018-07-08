namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using Figlut.Server.Toolkit.Data;

    #endregion //Using Directives

    public class SendSmsModel
    {
        #region Properties

        [DataType(DataType.PhoneNumber)]
        public string CellPhoneNumberSendSmsDialog { get; set; }

        public string MessageContentsSendSmsDialog { get; set; }

        public int MaxSmsSendMessageLength { get; set; }

        public string SmsMessageTemplateIdSendSms { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage, int maxSmsSendMessageLength)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.CellPhoneNumberSendSmsDialog))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SendSmsModel>.GetPropertyName(p => p.CellPhoneNumberSendSmsDialog, true));
            }
            string formattedPhoneNumber = null;
            if (!DataShaper.IsValidPhoneNumber(this.CellPhoneNumberSendSmsDialog, out formattedPhoneNumber))
            {
                errorMessage = string.Format("{0} is not a valid cell phone number.", this.CellPhoneNumberSendSmsDialog);
            }
            else if (string.IsNullOrEmpty(this.MessageContentsSendSmsDialog))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SendSmsModel>.GetPropertyName(p => p.MessageContentsSendSmsDialog, true));
            }
            else if (this.MessageContentsSendSmsDialog.Length > maxSmsSendMessageLength)
            {
                errorMessage = string.Format("{0} may not be greater than {1} characters.", EntityReader<SendSubscriberSmsModel>.GetPropertyName(p => p.MessageContentsSendSubscriberSmsDialog, true), maxSmsSendMessageLength);
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        #endregion //Methods
    }
}