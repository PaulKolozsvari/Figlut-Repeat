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
        #region Sms Processor Log

        public SmsProcessorLog GetSmsProcessorLog(Guid smsProcessorLogId, bool throwExceptionOnNotFound)
        {
            List<SmsProcessorLog> q = (from s in DB.GetTable<SmsProcessorLog>()
                                       where s.SmsProcessorLogId == smsProcessorLogId
                                       select s).ToList();
            SmsProcessorLog result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(SmsProcessorLog).Name,
                    EntityReader<SmsProcessorLog>.GetPropertyName(p => p.SmsProcessorLogId, false),
                    smsProcessorLogId.ToString()));
            }
            return result;
        }

        public long GetAllSmsProcessorLogsCount()
        {
            return DB.GetTable<SmsProcessorLog>().LongCount();
        }

        public List<SmsProcessorLogView> GetSmsProcessorLogViewsByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> smsProcessorId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<SmsProcessorLogView> result = null;
            if (smsProcessorId.HasValue) //For specified SMS Processor only.
            {
                result = (from smsProcessorLog in DB.GetTable<SmsProcessorLog>()
                          join smsProcessor in DB.GetTable<SmsProcessor>() on smsProcessorLog.SmsProcessorId equals smsProcessor.SmsProcessorId into set
                          from sub in set.DefaultIfEmpty()
                          where (smsProcessorLog.SmsProcessorId == smsProcessorId.Value) &&
                          (smsProcessorLog.DateCreated.Date >= startDate.Date && smsProcessorLog.DateCreated.Date <= endDate.Date) &&
                          ((smsProcessorLog.LogMessageType.ToLower().Contains(searchFilterLower)) ||
                          (smsProcessorLog.Message.Contains(searchFilterLower)) ||
                          (sub.Name.ToLower().Contains(searchFilterLower)))
                          orderby smsProcessorLog.DateCreated descending
                          select new SmsProcessorLogView()
                          {
                              SmsProcessorLogId = smsProcessorLog.SmsProcessorLogId,
                              SmsProcessorId = smsProcessorLog.SmsProcessorId,
                              LogMessageType = smsProcessorLog.LogMessageType,
                              Message = smsProcessorLog.Message,
                              DateCreated = smsProcessorLog.DateCreated,
                              SmsProcessorName = sub.Name
                          }).ToList();
            }
            else //For all Sms Processors.
            {
                result = (from smsProcessorLog in DB.GetTable<SmsProcessorLog>()
                          join smsProcessor in DB.GetTable<SmsProcessor>() on smsProcessorLog.SmsProcessorId equals smsProcessor.SmsProcessorId into set
                          from sub in set.DefaultIfEmpty()
                          where (smsProcessorLog.DateCreated.Date >= startDate.Date && smsProcessorLog.DateCreated.Date <= endDate.Date) &&
                          ((smsProcessorLog.LogMessageType.ToLower().Contains(searchFilterLower)) ||
                          (smsProcessorLog.Message.Contains(searchFilterLower)) ||
                          (sub.Name.ToLower().Contains(searchFilterLower)))
                          orderby smsProcessorLog.DateCreated descending
                          select new SmsProcessorLogView()
                          {
                              SmsProcessorLogId = smsProcessorLog.SmsProcessorLogId,
                              SmsProcessorId = smsProcessorLog.SmsProcessorId,
                              LogMessageType = smsProcessorLog.LogMessageType,
                              Message = smsProcessorLog.Message,
                              DateCreated = smsProcessorLog.DateCreated,
                              SmsProcessorName = sub.Name
                          }).ToList();
            }
            return result;
        }

        public List<SmsProcessorLog> GetSmsProcessorLogsByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> smsProcessorId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<SmsProcessorLog> result = null;
            if (smsProcessorId.HasValue) //For specified SMS Processor only.
            {
                //fix this!!!

                result = (from smsProcessorLog in DB.GetTable<SmsProcessorLog>()
                          join smsProcessor in DB.GetTable<SmsProcessor>() on smsProcessorLog.SmsProcessorId equals smsProcessor.SmsProcessorId into set
                          from sub in set.DefaultIfEmpty()
                          where (smsProcessorLog.SmsProcessorId == smsProcessorId.Value) &&
                          (smsProcessorLog.DateCreated.Date >= startDate.Date && smsProcessorLog.DateCreated.Date <= endDate.Date) &&
                          ((smsProcessorLog.LogMessageType.ToLower().Contains(searchFilterLower)) ||
                          (smsProcessorLog.Message.Contains(searchFilterLower)) ||
                          (sub.Name.ToLower().Contains(searchFilterLower)))
                          orderby smsProcessorLog.DateCreated descending
                          select smsProcessorLog).ToList();
            }
            else //For all Sms Processors.
            {
                result = (from smsProcessorLog in DB.GetTable<SmsProcessorLog>()
                          join smsProcessor in DB.GetTable<SmsProcessor>() on smsProcessorLog.SmsProcessorId equals smsProcessor.SmsProcessorId into set
                          from sub in set.DefaultIfEmpty()
                          where (smsProcessorLog.DateCreated.Date >= startDate.Date && smsProcessorLog.DateCreated.Date <= endDate.Date) &&
                          ((smsProcessorLog.LogMessageType.ToLower().Contains(searchFilterLower)) ||
                          (smsProcessorLog.Message.Contains(searchFilterLower)) ||
                          (sub.Name.ToLower().Contains(searchFilterLower)))
                          orderby smsProcessorLog.DateCreated descending
                          select smsProcessorLog).ToList();
            }
            return result;
        }

        public void DeleteSmsProcessorLogByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> smsProcessorId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<SmsProcessorLog> q = GetSmsProcessorLogsByFilter(searchFilter, startDate, endDate, smsProcessorId);
                DB.GetTable<SmsProcessorLog>().DeleteAllOnSubmit(q);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Sms Processor Log
    }
}