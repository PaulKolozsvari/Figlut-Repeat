namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class SmsProcessorLogModel
    {
        #region Sms Processor Log Properties

        public Guid SmsProcessorLogId { get; set; }

        public Guid SmsProcessorId { get; set; }

        public string LogMessageType { get; set; }

        public string Message { get; set; }

        public string MessageTrimmed { get; set; }

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

        public void CopyPropertiesToSmsProcessorLogView(SmsProcessorLogView smsProcessorLog)
        {
            smsProcessorLog.SmsProcessorLogId = this.SmsProcessorLogId;
            smsProcessorLog.SmsProcessorId = this.SmsProcessorId;
            smsProcessorLog.LogMessageType = this.LogMessageType;
            smsProcessorLog.Message = this.Message;
            smsProcessorLog.DateCreated = this.DateCreated;

            smsProcessorLog.SmsProcessorName = this.SmsProcessorName;
        }

        #endregion //Methods
    }
}