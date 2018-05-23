namespace Figlut.Spread.Web.Site.Models
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
        public string CellPhoneNumber { get; set; }

        public string MessageContents { get; set; }

        public int MaxSmsSendMessageLength { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage, int maxSmsSendMessageLength)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.CellPhoneNumber))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SendSmsModel>.GetPropertyName(p => p.CellPhoneNumber, true));
            }
            string formattedPhoneNumber = null;
            if (!DataShaper.IsValidPhoneNumber(this.CellPhoneNumber, out formattedPhoneNumber))
            {
                errorMessage = string.Format("{0} is not a valid number.", EntityReader<SendSmsModel>.GetPropertyName(p => p.CellPhoneNumber, true));
            }
            else if (string.IsNullOrEmpty(this.MessageContents))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SendSmsModel>.GetPropertyName(p => p.MessageContents, true));
            }
            else if (this.MessageContents.Length > maxSmsSendMessageLength)
            {
                errorMessage = string.Format("{0} may not be greater than {1} characters.", EntityReader<SendSubscriberSmsModel>.GetPropertyName(p => p.MessageContents, true), maxSmsSendMessageLength);
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        #endregion //Methods
    }
}