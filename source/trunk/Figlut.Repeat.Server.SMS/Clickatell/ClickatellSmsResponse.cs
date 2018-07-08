namespace Figlut.Repeat.SMS.Clickatell
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class ClickatellSmsResponse : SmsResponse
    {
        #region Constructors

        public ClickatellSmsResponse()
        {
        }

        public ClickatellSmsResponse(
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
