namespace Figlut.Repeat.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Repeat.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class RepeatEntityContext : EntityContext
    {
        #region Processor

        public List<Processor> GetAllProcessors()
        {
            List<Processor> result = (from s in DB.GetTable<Processor>()
                                      select s).ToList();
            return result;
        }

        public Processor GetProcessor(Guid processorId, bool throwExceptionOnNotFound)
        {
            List<Processor> q = (from s in DB.GetTable<Processor>()
                                 where s.ProcessorId == processorId
                                 select s).ToList();
            Processor result = q.Count < 1 ? null : q[0];
            if (throwExceptionOnNotFound && result == null)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Processor).Name,
                    EntityReader<Processor>.GetPropertyName(p => p.ProcessorId, false),
                    processorId.ToString()));
            }
            return result;
        }

        public List<Processor> GetProcessorsByFilter(string searchFilter)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<Processor> result = (from s in DB.GetTable<Processor>()
                                      where s.Name.ToLower().Contains(searchFilterLower)
                                      orderby s.Name
                                      select s).ToList();
            return result;
        }

        public long GetAllProcessorCount()
        {
            return DB.GetTable<Processor>().LongCount();
        }

        public void DeleteProcessorsByFilter(string searchFilter)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<Processor> processors = GetProcessorsByFilter(searchFilter);
                DB.GetTable<Processor>().DeleteAllOnSubmit(processors);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        public ProcessorLog LogProcesorAction(Guid processorId, string message, string logMessageType)
        {
            if (processorId == Guid.Empty)
            {
                throw new ArgumentException(string.Format("{0} may not be empty when creating a {1} entry.",
                    EntityReader<Processor>.GetPropertyName(p => p.ProcessorId, false),
                    typeof(ProcessorLog).Name));
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(string.Format("{0} may not be null or empty when creating a {1} entry.",
                    EntityReader<ProcessorLog>.GetPropertyName(p => p.Message, false),
                    typeof(ProcessorLog).Name));
            }
            if (string.IsNullOrEmpty(logMessageType))
            {
                throw new ArgumentException(string.Format("{0} may not be null or empty when creating a {1} entry.",
                    EntityReader<ProcessorLog>.GetPropertyName(p => p.LogMessageType, false),
                    typeof(ProcessorLog).Name));
            }
            ProcessorLog result = null;
            using (TransactionScope t = new TransactionScope())
            {
                result = new ProcessorLog()
                {
                    ProcessorLogId = Guid.NewGuid(),
                    ProcessorId = processorId,
                    LogMessageType = logMessageType,
                    Message = message,
                    DateCreated = DateTime.Now
                };
                DB.GetTable<ProcessorLog>().InsertOnSubmit(result);
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        #endregion //Processor
    }
}
