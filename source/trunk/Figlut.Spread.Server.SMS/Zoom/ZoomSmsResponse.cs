namespace Figlut.Spread.SMS.Zoom
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    /// <summary>
    /// The output response by ZoomConnect.com after sending a request to send an SMS.
    /// </summary>
    public class ZoomSmsResponse : SmsResponse
    {
        #region Constructors

        public ZoomSmsResponse()
        {
        }

        public ZoomSmsResponse(
            bool success,
            string messageId,
            string error,
            string errorCode)
            : base(success, messageId, error, errorCode)
        {
        }

        #endregion //Constructors
    }
}
