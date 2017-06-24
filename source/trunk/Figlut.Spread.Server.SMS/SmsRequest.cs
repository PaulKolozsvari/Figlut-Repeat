namespace Figlut.Spread.SMS
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SmsRequest
    {
        #region Constructors

        public SmsRequest()
        {
        }

        public SmsRequest(string recipientNumber, string message, int maxSmsSendMessageLength, string smsSendMessageSuffix, string organizationIdentifier, string organizationIdentifierIndicator)
        {
            _recipientNumber = recipientNumber;
            _message = message;
            ValidateMaxSmsSendMessageLength(maxSmsSendMessageLength, smsSendMessageSuffix, organizationIdentifier, organizationIdentifierIndicator, true);
        }

        #endregion //Constructors

        #region Fields

        protected string _recipientNumber;
        protected string _message;
        protected int _maxSmsSendMessageLength;
        protected string _smsSendMessageSuffix;
        protected string _organizationIdentifier;
        protected string _organizationIdentifierIndicator;

        #endregion //Fields

        #region Properties

        /// <summary>
        /// //Mandatory: the cell phone number to send the SMS to.
        /// </summary>
        public string recipientNumber
        {
            get { return _recipientNumber; }
            set { _recipientNumber = value; }
        }

        /// <summary>
        /// //Mandatory: the SMS message to send.
        /// </summary>
        public string message
        {
            get { return _message; }
            set { _message = value; }
        }

        #endregion //Properties

        #region Methods

        public void ValidateMaxSmsSendMessageLength(
            int maxSmsSendMessageLength, 
            string smsSendMessageSuffix, 
            string organizationIdentifier, 
            string organizationIdentifierIndicator,
            bool appendSuffix)
        {
            if (string.IsNullOrEmpty(this.recipientNumber))
            {
                throw new NullReferenceException(string.Format("{0} may not be null or empty.", EntityReader<SmsRequest>.GetPropertyName(p => p.recipientNumber, true)));
            }
            if (string.IsNullOrEmpty(this.message))
            {
                throw new NullReferenceException(string.Format("{0} may not be null or empty.", EntityReader<SmsRequest>.GetPropertyName(p => p.message, true)));
            }
            if ((maxSmsSendMessageLength > 0) && (_message.Length > maxSmsSendMessageLength))
            {
                throw new ArgumentOutOfRangeException(string.Format("{0} {1} may not be greater than {2} characters.",
                    typeof(SmsRequest),
                    EntityReader<SmsRequest>.GetPropertyName(p => p.message, true),
                    maxSmsSendMessageLength));
            }
            _maxSmsSendMessageLength = maxSmsSendMessageLength;
            _smsSendMessageSuffix = smsSendMessageSuffix;
            _organizationIdentifier = organizationIdentifier;
            _organizationIdentifierIndicator = organizationIdentifierIndicator;
            if (appendSuffix)
            {
                AppendSuffix();
            }
        }

        private void AppendSuffix()
        {
            if (!string.IsNullOrEmpty(_smsSendMessageSuffix))
            {
                string messageToAppend = _smsSendMessageSuffix;
                if (!string.IsNullOrEmpty(_organizationIdentifier) && messageToAppend.Contains("{0}")) //Substitute {0} with the organziation identifier.
                {
                    string organizationAnchor = string.Format("{0}{1}", _organizationIdentifierIndicator, _organizationIdentifier);
                    messageToAppend = string.Format(messageToAppend, organizationAnchor);
                }
                _message += messageToAppend;
            }
        }

        #endregion //Methods
    }
}
