namespace Figlut.Repeat.SMS
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SmsReceivedParsed
    {
        #region Constructors

        public SmsReceivedParsed(
            string from,
            string messageId,
            string message,
            string date,
            string campaign,
            string dataField,
            string nonce,
            string noncedate,
            string checksum,
            string organizationIdentifierIndicator,
            string subscriberNameIndicator)
        {
            _from = from;
            _messageId = messageId;
            _message = message;
            _date = date;
            _campaign = campaign;
            _dataField = dataField;
            _nonce = nonce;
            _noncedate = noncedate;
            _checksum = checksum;
            _organizationIdentifierIndicator = organizationIdentifierIndicator;
            _subscriberNameIndicator = subscriberNameIndicator;

            ParseMessage();
        }

        #endregion //Constructors

        #region Fields

        #region Received Fields

        private string _from;
        private string _messageId;
        private string _message;
        private string _date;
        private string _campaign;
        private string _dataField;
        private string _nonce;
        private string _noncedate;
        private string _checksum;

        #endregion //Received Fields

        #region Derived Fields

        private string _organizationIdentifierIndicator;
        private string _subscriberNameIndicator;
        private string _organizationIdentifier;
        private string _subscriberName;
        private string _messageToOrganization;

        private bool _parsedSuccessfully;
        private string _errorMessage;
        private string _warningMessage;

        #endregion //Derived Fields

        #endregion //Fields

        #region Properties

        #region Received Properties

        public string From
        {
            get { return _from; }
        }

        public string MessageId
        {
            get { return _messageId; }
        }

        public string Message
        {
            get { return _message; }
        }

        public string Date
        {
            get { return _date; }
        }

        public string Campaign
        {
            get { return _campaign; }
        }

        public string DataField
        {
            get { return _dataField; }
        }

        public string Nonce
        {
            get { return _nonce; }
        }

        public string NonceDate
        {
            get { return _noncedate; }
        }

        public string CheckSum
        {
            get { return _checksum; }
        }

        #endregion //Received Properties

        #region Derived Properties

        public string OrganizationIdentifierIndicator
        {
            get { return _organizationIdentifierIndicator; }
        }

        public string SubscriberNameIndicator
        {
            get { return _subscriberNameIndicator; }
        }

        public string OrganizationIdentifier
        {
            get { return _organizationIdentifier; }
        }

        public string SubscriberName
        {
            get { return _subscriberName; }
        }

        public string MessageToOrganization
        {
            get { return _messageToOrganization; }
        }

        public bool ParsedSuccessfully
        {
            get { return _parsedSuccessfully; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public string WarningMessage
        {
            get { return _warningMessage; }
        }

        #endregion //Derived Properties

        #endregion //Properties

        #region Methods

        private void ValidateFields()
        {
            if (string.IsNullOrEmpty(_organizationIdentifierIndicator))
            {
                throw new NullReferenceException(string.Format("{0} is null or empty in {1}. Message ID: {2}",
                    EntityReader<SmsReceivedParsed>.GetPropertyName(p => p.OrganizationIdentifierIndicator, false),
                    DataShaper.ShapeCamelCaseString(typeof(SmsReceivedParsed).Name),
                    _messageId == null ? string.Empty : _messageId));
            }
            if (string.IsNullOrEmpty(_subscriberNameIndicator))
            {
                throw new NullReferenceException(string.Format("{0} is null or empty in {1}. Message ID: {2}",
                    EntityReader<SmsReceivedParsed>.GetPropertyName(p => p.SubscriberNameIndicator, false),
                    DataShaper.ShapeCamelCaseString(typeof(SmsReceivedParsed).Name),
                    _messageId == null ? string.Empty : _messageId));
            }
            if (string.IsNullOrEmpty(_from))
            {
                throw new NullReferenceException(string.Format("{0} is null or empty in {1}. Message ID: {2}",
                    EntityReader<SmsReceivedParsed>.GetPropertyName(p => p.From, false),
                    DataShaper.ShapeCamelCaseString(typeof(SmsReceivedParsed).Name),
                    _messageId == null ? string.Empty : _messageId));
            }
            if (string.IsNullOrEmpty(_message))
            {
                throw new NullReferenceException(string.Format("{0} is null or empty in {1}. Message ID: {2}",
                    EntityReader<SmsReceivedParsed>.GetPropertyName(p => p.Message, false),
                    DataShaper.ShapeCamelCaseString(typeof(SmsReceivedParsed).Name),
                    _messageId == null ? string.Empty : _messageId));
            }
        }

        private void ParseMessage()
        {
            try
            {
                ValidateFields();
                _organizationIdentifierIndicator = _organizationIdentifierIndicator.ToLower();
                StringBuilder messageToOrganization = new StringBuilder();

                string[] messageWords = _message.Trim().Split(' ');
                foreach (string word in messageWords)
                {
                    string keyWord = word;
                    if (keyWord.StartsWith(_organizationIdentifierIndicator)) //This word indicates that the organization identifier is being specified in the message.
                    {
                        _organizationIdentifier = keyWord.Replace(_organizationIdentifierIndicator, string.Empty);
                    }
                    else if (keyWord.StartsWith(_subscriberNameIndicator))
                    {
                        _subscriberName = keyWord.Replace(_subscriberNameIndicator, string.Empty);
                    }
                    else
                    {
                        messageToOrganization.Append(string.Format("{0} ", word));
                    }
                }
                _messageToOrganization = messageToOrganization.Length > 0 ? messageToOrganization.Remove(messageToOrganization.Length - 1, 1).ToString() : messageToOrganization.ToString(); //Removes the trailing space.
                StringBuilder warningMessage = new StringBuilder();
                if (string.IsNullOrEmpty(_organizationIdentifier))
                {
                    warningMessage.AppendLine(string.Format("Warning: Could not read {0} on {1}. Message ID: {2}. Message: {3}",
                        EntityReader<SmsReceivedParsed>.GetPropertyName(p => p.OrganizationIdentifier, true),
                        DataShaper.ShapeCamelCaseString(typeof(SmsReceivedParsed).Name),
                        (_messageId == null ? string.Empty : _messageId),
                        _message));
                }
                else if (string.IsNullOrEmpty(_subscriberName))
                {
                    warningMessage.AppendLine(string.Format("Warning: No {0} on {1}. Message ID: {2}. Message: {3}. Subscriber Name Indicator = {4}",
                        EntityReader<SmsReceivedParsed>.GetPropertyName(p => p.SubscriberName, true),
                        DataShaper.ShapeCamelCaseString(typeof(SmsReceivedParsed).Name),
                        (_messageId == null ? string.Empty : _messageId),
                        _message,
                        _subscriberNameIndicator));
                }
                if (warningMessage.Length > 0)
                {
                    _warningMessage = warningMessage.ToString();
                }
                _parsedSuccessfully = true;
            }
            catch (Exception ex)
            {
                _parsedSuccessfully = false;
                _errorMessage = ex.Message;
            }
        }

        #endregion //Methods
    }
}