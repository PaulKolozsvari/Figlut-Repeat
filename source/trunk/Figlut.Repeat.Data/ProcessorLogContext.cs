namespace Figlut.Repeat.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class RepeatEntityContext : EntityContext
    {
        #region Processor Log

        public ProcessorLog GetProcessorLog(Guid processorLogId, bool throwExceptionOnNotFound)
        {
            List<ProcessorLog> q = (from s in DB.GetTable<ProcessorLog>()
                                    where s.ProcessorLogId == processorLogId
                                    select s).ToList();
            ProcessorLog result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(ProcessorLog).Name,
                    EntityReader<ProcessorLog>.GetPropertyName(p => p.ProcessorLogId, false),
                    processorLogId.ToString()));
            }
            return result;
        }

        public long GetAllProcessorLogsCount()
        {
            return DB.GetTable<ProcessorLog>().LongCount();
        }

        public List<ProcessorLogView> GetProcessorLogViewsByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> processorId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<ProcessorLogView> result = null;
            if (processorId.HasValue) //For specified Processor only.
            {
                result = (from processorLog in DB.GetTable<ProcessorLog>()
                          join processor in DB.GetTable<Processor>() on processorLog.ProcessorId equals processor.ProcessorId into set
                          from sub in set.DefaultIfEmpty()
                          where (processorLog.ProcessorId == processorId.Value) &&
                          (processorLog.DateCreated.Date >= startDate.Date && processorLog.DateCreated.Date <= endDate.Date) &&
                          ((processorLog.LogMessageType.ToLower().Contains(searchFilterLower)) ||
                          (processorLog.Message.Contains(searchFilterLower)) ||
                          (sub.Name.ToLower().Contains(searchFilterLower)))
                          orderby processorLog.DateCreated descending
                          select new ProcessorLogView()
                          {
                              ProcessorLogId = processorLog.ProcessorLogId,
                              ProcessorId = processorLog.ProcessorId,
                              LogMessageType = processorLog.LogMessageType,
                              Message = processorLog.Message,
                              DateCreated = processorLog.DateCreated,
                              ProcessorName = sub.Name
                          }).ToList();
            }
            else //For all Processors.
            {
                result = (from processorLog in DB.GetTable<ProcessorLog>()
                          join processor in DB.GetTable<Processor>() on processorLog.ProcessorId equals processor.ProcessorId into set
                          from sub in set.DefaultIfEmpty()
                          where (processorLog.DateCreated.Date >= startDate.Date && processorLog.DateCreated.Date <= endDate.Date) &&
                          ((processorLog.LogMessageType.ToLower().Contains(searchFilterLower)) ||
                          (processorLog.Message.Contains(searchFilterLower)) ||
                          (sub.Name.ToLower().Contains(searchFilterLower)))
                          orderby processorLog.DateCreated descending
                          select new ProcessorLogView()
                          {
                              ProcessorLogId = processorLog.ProcessorLogId,
                              ProcessorId = processorLog.ProcessorId,
                              LogMessageType = processorLog.LogMessageType,
                              Message = processorLog.Message,
                              DateCreated = processorLog.DateCreated,
                              ProcessorName = sub.Name
                          }).ToList();
            }
            return result;
        }

        public List<ProcessorLog> GetProcessorLogsByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> processorId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<ProcessorLog> result = null;
            if (processorId.HasValue) //For specified Processor only.
            {
                result = (from processorLog in DB.GetTable<ProcessorLog>()
                          join processor in DB.GetTable<Processor>() on processorLog.ProcessorId equals processor.ProcessorId into set
                          from sub in set.DefaultIfEmpty()
                          where (processorLog.ProcessorId == processorId.Value) &&
                          (processorLog.DateCreated.Date >= startDate.Date && processorLog.DateCreated.Date <= endDate.Date) &&
                          ((processorLog.LogMessageType.ToLower().Contains(searchFilterLower)) ||
                          (processorLog.Message.Contains(searchFilterLower)) ||
                          (sub.Name.ToLower().Contains(searchFilterLower)))
                          orderby processorLog.DateCreated descending
                          select processorLog).ToList();
            }
            else //For all Processors.
            {
                result = (from processorLog in DB.GetTable<ProcessorLog>()
                          join processor in DB.GetTable<Processor>() on processorLog.ProcessorId equals processor.ProcessorId into set
                          from sub in set.DefaultIfEmpty()
                          where (processorLog.DateCreated.Date >= startDate.Date && processorLog.DateCreated.Date <= endDate.Date) &&
                          ((processorLog.LogMessageType.ToLower().Contains(searchFilterLower)) ||
                          (processorLog.Message.Contains(searchFilterLower)) ||
                          (sub.Name.ToLower().Contains(searchFilterLower)))
                          orderby processorLog.DateCreated descending
                          select processorLog).ToList();
            }
            return result;
        }

        public void DeleteProcessorLogByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> processorId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<ProcessorLog> q = GetProcessorLogsByFilter(searchFilter, startDate, endDate, processorId);
                DB.GetTable<ProcessorLog>().DeleteAllOnSubmit(q);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Processor Log
    }
}