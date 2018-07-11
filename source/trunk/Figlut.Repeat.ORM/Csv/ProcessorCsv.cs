namespace Figlut.Repeat.ORM.Csv
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class ProcessorCsv
    {
        #region Properties

        public Guid ProcessorId { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public int ExecutionInterval { get; set; }

        public Nullable<DateTime> LastExecutionDate { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromProcessor(Processor processor)
        {
            this.ProcessorId = processor.ProcessorId;
            this.Name = processor.Name;
            this.Enabled = processor.Enabled;
            this.ExecutionInterval = processor.ExecutionInterval;
            this.LastExecutionDate = processor.LastExecutionDate;
            this.DateCreated = processor.DateCreated;
        }

        public void CopyPropertiesToProcessor(Processor processor)
        {
            processor.ProcessorId = this.ProcessorId;
            processor.Name = this.Name;
            processor.Enabled = this.Enabled;
            processor.ExecutionInterval = this.ExecutionInterval;
            processor.LastExecutionDate = this.LastExecutionDate;
            processor.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}
