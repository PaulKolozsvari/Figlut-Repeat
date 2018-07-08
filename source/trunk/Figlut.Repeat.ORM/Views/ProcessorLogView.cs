namespace Figlut.Repeat.ORM.Views
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class ProcessorLogView
    {
        #region Properties

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

        #endregion //Properties
    }
}
