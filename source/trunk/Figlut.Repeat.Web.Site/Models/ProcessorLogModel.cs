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

    public class ProcessorLogModel
    {
        #region Processor Log Properties

        public Guid ProcessorLogId { get; set; }

        public Guid ProcessorId { get; set; }

        public string LogMessageType { get; set; }

        public string Message { get; set; }

        public string MessageTrimmed { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Processor Log Properties

        #region Sms Process Properties

        public string ProcessorName { get; set; }

        #endregion //Sms Process Properties

        #region Methods

        public void CopyPropertiesFromProcessorLogView(ProcessorLogView view)
        {
            this.ProcessorLogId = view.ProcessorLogId;
            this.ProcessorId = view.ProcessorId;
            this.LogMessageType = view.LogMessageType;
            this.Message = view.Message;
            this.DateCreated = view.DateCreated;

            this.ProcessorName = view.ProcessorName;
        }

        public void CopyPropertiesToProcessorLogView(ProcessorLogView view)
        {
            view.ProcessorLogId = this.ProcessorLogId;
            view.ProcessorId = this.ProcessorId;
            view.LogMessageType = this.LogMessageType;
            view.Message = this.Message;
            view.DateCreated = this.DateCreated;

            view.ProcessorName = this.ProcessorName;
        }

        #endregion //Methods
    }
}