namespace Figlut.Spread.Web.Service.TestDriver
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

        public SmsRequest(string recipientNumber, string message)
        {
            if (string.IsNullOrEmpty(recipientNumber))
            {
                throw new NullReferenceException(string.Format("{0} may not be null or empty.", EntityReader<SmsRequest>.GetPropertyName(p => p.recipientNumber, true)));
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new NullReferenceException(string.Format("{0} may not be null or empty.", EntityReader<SmsRequest>.GetPropertyName(p => p.message, true)));
            }
            _recipientNumber = recipientNumber;
            _message = message;
        }

        #endregion //Constructors

        #region Fields

        protected string _recipientNumber;
        protected string _message;

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
    }
}