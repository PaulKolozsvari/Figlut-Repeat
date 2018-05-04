namespace Figlut.Spread.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Spread.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class SpreadEntityContext : EntityContext
    {
        #region Sms Received

        public long GetAllSmsReceivedLogCount()
        {
            return DB.GetTable<SmsReceivedLog>().LongCount();
        }

        public List<SmsReceivedLog> GetSmsReceivedLogByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<SmsReceivedLog> result = null;
            if (organizationId.HasValue) //For specified organization only.
            {
                result = (from s in DB.GetTable<SmsReceivedLog>()
                          where (s.OrganizationId.HasValue && s.OrganizationId.Value == organizationId.Value) &&
                          (s.DateCreated.Date >= startDate.Date && s.DateCreated.Date <= endDate.Date) &&
                          (s.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          s.MessageId.ToLower().Contains(searchFilterLower) ||
                          s.MessageContents.ToLower().Contains(searchFilterLower)
                              //s.Nonce.ToLower().Contains(searchFilterLower) ||
                              //s.Checksum.ToLower().Contains(searchFilterLower)
                          )
                          orderby s.DateCreated descending
                          select s).ToList();
            }
            else //For all organizations.
            {
                result = (from s in DB.GetTable<SmsReceivedLog>()
                          where (s.DateCreated.Date >= startDate.Date && s.DateCreated.Date <= endDate.Date) &&
                          (s.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          s.MessageId.ToLower().Contains(searchFilterLower) ||
                          s.MessageContents.ToLower().Contains(searchFilterLower)
                              //s.Nonce.ToLower().Contains(searchFilterLower) ||
                              //s.Checksum.ToLower().Contains(searchFilterLower)
                          )
                          orderby s.DateCreated descending
                          select s).ToList();
            }
            return result;
        }

        public void DeleteSmsReceivedLogByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> organizationId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<SmsReceivedLog> q = GetSmsReceivedLogByFilter(searchFilter, startDate, endDate, organizationId);
                DB.GetTable<SmsReceivedLog>().DeleteAllOnSubmit(q);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        public List<SmsSentLog> GetSmsSentList(DateTime startDate, DateTime endDate)
        {
            List<SmsSentLog> result = (from s in DB.GetTable<SmsSentLog>()
                                       where s.DateCreated >= startDate && s.DateCreated <= endDate
                                       select s).ToList();
            return result;
        }

        public List<SmsReceivedLog> GetSmsReceivedList(DateTime startDate, DateTime endDate)
        {
            List<SmsReceivedLog> result = (from s in DB.GetTable<SmsReceivedLog>()
                                           where s.DateCreated >= startDate && s.DateCreated <= endDate
                                           select s).ToList();
            return result;
        }

        public SmsSentLog GetSmsSentLog(Guid smsSentLogId, bool throwExceptionOnNotFound)
        {
            List<SmsSentLog> q = (from s in DB.GetTable<SmsSentLog>()
                                  where s.SmsSentLogId == smsSentLogId
                                  select s).ToList();
            SmsSentLog result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(SmsSentLog).Name,
                    EntityReader<SmsSentLog>.GetPropertyName(p => p.SmsSentLogId, false),
                    smsSentLogId.ToString()));
            }
            return result;
        }

        public List<SmsSentLog> GetSmsSentLog(string messageId, bool throwExceptionOnNotFound)
        {
            List<SmsSentLog> q = (from s in DB.GetTable<SmsSentLog>()
                                  where s.MessageId == messageId
                                  select s).ToList();
            if (q.Count < 1 && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(SmsSentLog).Name,
                    EntityReader<SmsSentLog>.GetPropertyName(p => p.SmsSentLogId, false),
                    messageId));
            }
            return q;
        }

        public SmsReceivedLog GetSmsReceivedLog(Guid smsReceivedLogId, bool throwExceptionOnNotFound)
        {
            List<SmsReceivedLog> q = (from s in DB.GetTable<SmsReceivedLog>()
                                      where s.SmsReceivedLogId == smsReceivedLogId
                                      select s).ToList();
            SmsReceivedLog result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(SmsReceivedLog).Name,
                    EntityReader<SmsReceivedLog>.GetPropertyName(p => p.SmsReceivedLogId, false),
                    smsReceivedLogId.ToString()));
            }
            return result;
        }

        #endregion //Sms Received
    }
}
