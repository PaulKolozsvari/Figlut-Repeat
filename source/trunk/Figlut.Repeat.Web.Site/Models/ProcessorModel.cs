namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using Figlut.Repeat.ORM;
    using Figlut.Server.Toolkit.Data;
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

        public bool Enabled { get; set; }

        public int ExecutionInterval { get; set; }

        public Nullable<DateTime> LastExecutionDate { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.Name))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<ProcessorModel>.GetPropertyName(p => p.Name, true));
            }
            if (this.ExecutionInterval <= 0)
            {
                errorMessage = string.Format("{0} may not be less than or equal to 0.", EntityReader<ProcessorModel>.GetPropertyName(p => p.Name, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

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