namespace Figlut.Repeat.ORM.Csv
{
    #region Using Directives

    using Figlut.Repeat.ORM.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SmsProcessorLogCsv
    {
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

        #region Methods

        public void CopyPropertiesFromSmsProcessorLogView(SmsProcessorLogView view)
        {
            this.SmsProcessorLogId = view.SmsProcessorLogId;
            this.SmsProcessorId = view.SmsProcessorId;
            this.LogMessageType = view.LogMessageType;
            this.Message = view.Message;
            this.DateCreated = view.DateCreated;

            this.SmsProcessorName = view.SmsProcessorName;
        }

        public void CopyPropertiesToSmsProcessorLogView(SmsProcessorLogView view)
        {
            view.SmsProcessorLogId = this.SmsProcessorLogId;
            view.SmsProcessorId = this.SmsProcessorId;
            view.LogMessageType = this.LogMessageType;
            view.Message = this.Message;
            view.DateCreated = this.DateCreated;

            view.SmsProcessorName = this.SmsProcessorName;
        }

        #endregion //Methods
    }
}
