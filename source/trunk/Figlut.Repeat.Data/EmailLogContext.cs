namespace Figlut.Repeat.Data
{
    #region Using Directives

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
        #region Email Log

        public List<EmailLog> GetAllEmailLogs()
        {
            return (from e in DB.GetTable<EmailLog>()
                    select e).ToList();
        }

        public long GetAllEmailLogCount()
        {
            return DB.GetTable<EmailLog>().LongCount();
        }

        public List<EmailLog> GetEmailLogByFilter(string searchFilter, DateTime startDate, DateTime endDate)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<EmailLog> result = (from e in DB.GetTable<EmailLog>()
                                     where (e.DateCreated.Date >= startDate.Date && e.DateCreated.Date <= endDate.Date) &&
                                     (e.EmailAddress.ToLower().Contains(searchFilterLower) ||
                                      e.MessageContents.ToLower().Contains(searchFilterLower) ||
                                      e.TableReference.ToString().ToLower().Contains(searchFilterLower) ||
                                      e.RecordReference.ToString().ToLower().Contains(searchFilterLower) ||
                                      e.ErrorCode.ToString().ToLower().Contains(searchFilterLower) ||
                                      e.ErrorMessage.ToLower().Contains(searchFilterLower))
                                     select e).ToList();
            return result;
        }

        public void DeleteEmailLogByFilter(string searchFilter, DateTime startDate, DateTime endDate)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<EmailLog> q = GetEmailLogByFilter(searchFilter, startDate, endDate);
                DB.GetTable<EmailLog>().DeleteAllOnSubmit(q);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        public EmailLog LogEmailNotification(
            int errorCode,
            string errorMessage,
            string emailAddress,
            string messageContents,
            string tableReference,
            Nullable<Guid> recordReference,
            string tag,
            int emailProviderCode)

        {
            EmailLog result = null;
            using (TransactionScope t = new TransactionScope())
            {
                result = new EmailLog()
                {
                    EmailLogId = Guid.NewGuid(),
                    ErrorCode = errorCode,
                    ErrorMessage = errorMessage,
                    EmailAddress = emailAddress,
                    MessageContents = messageContents,
                    TableReference = tableReference,
                    RecordReference = recordReference,
                    Tag = tag,
                    EmailProviderCode = emailProviderCode,
                    DateCreated = DateTime.Now
                };
                DB.GetTable<EmailLog>().InsertOnSubmit(result);
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        #endregion //Email Log
    }
}