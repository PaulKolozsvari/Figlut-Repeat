﻿namespace Figlut.Spread.SMS.Clickatell
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class ClickatellSmsRequest : SmsRequest
    {
        #region Constructors

        public ClickatellSmsRequest()
        {
        }

        public ClickatellSmsRequest(string recipientNumber, string message, int maxSmsSendMessageLength, string smsSendMessageSuffix, string organizationIdentifier, string organizationIdentifierIndicator)
            : base(recipientNumber, message, maxSmsSendMessageLength, smsSendMessageSuffix, organizationIdentifier, organizationIdentifierIndicator)
        {
        }

        #endregion //Constructors
    }
}
