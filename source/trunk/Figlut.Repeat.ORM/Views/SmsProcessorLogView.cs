namespace Figlut.Repeat.ORM.Views
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SmsProcessorLogView
    {
        #region Properties

        #region Sms Processor Log Properties

        public Guid SmsProcessorLogId { get; set; }

        public Guid SmsProcessorId { get; set; }

        public string LogMessageType { get; set; }

        public string Message { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Sms Processor Log Properties

        #region Sms Process Properties

        public string SmsProcessorName { get; set; }

        #endregion //Sms Process Properties

        #endregion //Properties
    }
}
