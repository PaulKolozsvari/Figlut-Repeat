namespace Figlut.Spread.SMS.Zoom
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    /// <summary>
    /// Input parameters to send to ZoomConnect.com REST API to send an SMS.
    /// </summary>
    public class ZoomSmsRequest : SmsRequest
    {
        #region Constructors

        public ZoomSmsRequest()
        {
        }

        public ZoomSmsRequest(string recipientNumber, string message, int maxSmsSendMessageLength, string smsSendMessageSuffix, string organizationIdentifier, string organizationIdentifierIndicator)
            : base(recipientNumber, message, maxSmsSendMessageLength, smsSendMessageSuffix, organizationIdentifier, organizationIdentifierIndicator)
        {
        }

        #endregion //Constructors

        #region Fields

        private string _campaign;
        private string _dataField;
        private string _dateToSend;

        #endregion //Fields

        #region Properties

        /// <summary>
        /// Optional: campaign name
        /// </summary>
        public string campaign
        {
            get { return _campaign; }
            set { _campaign = value; }
        }

        /// <summary>
        /// Optional: extra field
        /// </summary>
        public string dataField
        {
            get { return _dataField; }
            set { _dataField = value; }
        }

        /// <summary>
        /// Optional: the date on which the SMS should be sent.
        /// </summary>
        public string dateToSend
        {
            get { return _dateToSend; }
            set { _dateToSend = value; }
        }

        #endregion //Properties
    }
}