namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Views;

    #endregion //Using Directives

    public class SendSubscriberSmsModel
    {
        #region Properties

        public Guid SubscriberId { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string CellPhoneNumberSendSubscriberSmsDialog { get; set; }

        public string MessageContentsSendSubscriberSmsDialog { get; set; }

        public int MaxSmsSendMessageLength { get; set; }

        public string SmsMessageTemplateIdSendSubsccriberSmsDialog { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage, int maxSmsSendMessageLength)
        {
            errorMessage = null;
            if (this.SubscriberId == Guid.NewGuid())
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SendSubscriberSmsModel>.GetPropertyName(p => p.SubscriberId, true));
            }
            if (string.IsNullOrEmpty(this.CellPhoneNumberSendSubscriberSmsDialog))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SendSubscriberSmsModel>.GetPropertyName(p => p.CellPhoneNumberSendSubscriberSmsDialog, true));
            }
            string formattedPhoneNumber = null;
            if (!DataShaper.IsValidPhoneNumber(this.CellPhoneNumberSendSubscriberSmsDialog, out formattedPhoneNumber))
            {
                errorMessage = string.Format("{0} is not a valid number.", EntityReader<SendSubscriberSmsModel>.GetPropertyName(p => p.CellPhoneNumberSendSubscriberSmsDialog, true));
            }
            else if (string.IsNullOrEmpty(this.MessageContentsSendSubscriberSmsDialog))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SendSubscriberSmsModel>.GetPropertyName(p => p.MessageContentsSendSubscriberSmsDialog, true));
            }
            else if (this.MessageContentsSendSubscriberSmsDialog.Length > maxSmsSendMessageLength)
            {
                errorMessage = string.Format("{0} may not be greater than {1} characters.", EntityReader<SendSubscriberSmsModel>.GetPropertyName(p => p.MessageContentsSendSubscriberSmsDialog, true), maxSmsSendMessageLength);
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromSubscriber(Subscriber subscriber)
        {
            this.SubscriberId = subscriber.SubscriberId;
            this.CellPhoneNumberSendSubscriberSmsDialog = subscriber.CellPhoneNumber;
        }

        #endregion //Methods
    }
}