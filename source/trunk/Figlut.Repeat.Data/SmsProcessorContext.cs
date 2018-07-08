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
        #region Sms Processor

        public List<SmsProcessor> GetAllSmsProcessors()
        {
            List<SmsProcessor> result = (from s in DB.GetTable<SmsProcessor>()
                                         select s).ToList();
            return result;
        }

        public SmsProcessor GetSmsProcessor(Guid smsProcessorid, bool throwExceptionOnNotFound)
        {
            List<SmsProcessor> q = (from s in DB.GetTable<SmsProcessor>()
                                    where s.SmsProcessorId == smsProcessorid
                                    select s).ToList();
            SmsProcessor result = q.Count < 1 ? null : q[0];
            if (throwExceptionOnNotFound && result == null)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(SmsProcessor).Name,
                    EntityReader<SmsProcessor>.GetPropertyName(p => p.SmsProcessorId, false),
                    smsProcessorid.ToString()));
            }
            return result;
        }

        public List<SmsProcessor> GetSmsProcessorsByFilter(string searchFilter)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<SmsProcessor> result = (from s in DB.GetTable<SmsProcessor>()
                                         where s.Name.ToLower().Contains(searchFilterLower)
                                         orderby s.Name
                                         select s).ToList();
            return result;
        }

        public long GetAllSmsProcessorCount()
        {
            return DB.GetTable<SmsProcessor>().LongCount();
        }

        public void DeleteSmsProcessorsByFilter(string searchFilter)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<SmsProcessor> smsProcessors = GetSmsProcessorsByFilter(searchFilter);
                DB.GetTable<SmsProcessor>().DeleteAllOnSubmit(smsProcessors);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        public SmsProcessorLog LogSmsProcesorAction(Guid smsProcessorId, string message, string logMessageType)
        {
            if (smsProcessorId == Guid.Empty)
            {
                throw new ArgumentException(string.Format("{0} may not be empty when creating a {1} entry.",
                    EntityReader<SmsProcessorLog>.GetPropertyName(p => p.SmsProcessorId, false),
                    typeof(SmsProcessorLog).Name));
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(string.Format("{0} may not be null or empty when creating a {1} entry.",
                    EntityReader<SmsProcessorLog>.GetPropertyName(p => p.Message, false),
                    typeof(SmsProcessorLog).Name));
            }
            if (string.IsNullOrEmpty(logMessageType))
            {
                throw new ArgumentException(string.Format("{0} may not be null or empty when creating a {1} entry.",
                    EntityReader<SmsProcessorLog>.GetPropertyName(p => p.LogMessageType, false),
                    typeof(SmsProcessorLog).Name));
            }
            SmsProcessorLog result = null;
            using (TransactionScope t = new TransactionScope())
            {
                result = new SmsProcessorLog()
                {
                    SmsProcessorLogId = Guid.NewGuid(),
                    SmsProcessorId = smsProcessorId,
                    LogMessageType = logMessageType,
                    Message = message,
                    DateCreated = DateTime.Now
                };
                DB.GetTable<SmsProcessorLog>().InsertOnSubmit(result);
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        #endregion //Sms Processor
    }
}
