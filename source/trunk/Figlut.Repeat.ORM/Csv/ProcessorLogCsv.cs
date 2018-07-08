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

    public class ProcessorLogCsv
    {
        #region Processor Log Properties

        public Guid ProcessorLogId { get; set; }

        public Guid ProcessorId { get; set; }

        public string LogMessageType { get; set; }

        public string Message { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Processor Log Properties

        #region Process Properties

        public string ProcessorName { get; set; }

        #endregion //Process Properties

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
