namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using Figlut.Repeat.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class ProcessorModel
    {
        #region Properties

        public Guid ProcessorId { get; set; }

        public string Name { get; set; }

        public int ExecutionInterval { get; set; }

        public Nullable<DateTime> LastExecutionDate { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromProcessor(Processor processor)
        {
            this.ProcessorId = processor.ProcessorId;
            this.Name = processor.Name;
            this.ExecutionInterval = processor.ExecutionInterval;
            this.LastExecutionDate = processor.LastExecutionDate;
            this.DateCreated = processor.DateCreated;
        }

        public void CopyPropertiesToProcessor(Processor processor)
        {
            processor.ProcessorId = this.ProcessorId;
            processor.Name = this.Name;
            processor.ExecutionInterval = this.ExecutionInterval;
            processor.LastExecutionDate = this.LastExecutionDate;
            processor.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}