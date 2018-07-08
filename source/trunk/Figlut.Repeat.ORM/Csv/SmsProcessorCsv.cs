namespace Figlut.Repeat.ORM.Csv
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SmsProcessorCsv
    {
        #region Properties

        public Guid SmsProcessorId { get; set; }

        public string Name { get; set; }

        public int ExecutionInterval { get; set; }

        public Nullable<DateTime> LastExecutionDate { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromSmsProcessor(SmsProcessor smsProcessor)
        {
            this.SmsProcessorId = smsProcessor.SmsProcessorId;
            this.Name = smsProcessor.Name;
            this.ExecutionInterval = smsProcessor.ExecutionInterval;
            this.LastExecutionDate = smsProcessor.LastExecutionDate;
            this.DateCreated = smsProcessor.DateCreated;
        }

        public void CopyPropertiesTo(SmsProcessor smsProcessor)
        {
            smsProcessor.SmsProcessorId = this.SmsProcessorId;
            smsProcessor.Name = this.Name;
            smsProcessor.ExecutionInterval = this.ExecutionInterval;
            smsProcessor.LastExecutionDate = this.LastExecutionDate;
            smsProcessor.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}
