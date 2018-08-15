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

        public List<EmailLog> GetEmailLogByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<EmailLog> result = null;
            if (organizationId.HasValue)
            {
                result = (from e in DB.GetTable<EmailLog>()
                          where (e.OrganizationId.HasValue && e.OrganizationId.Value == organizationId.Value) &&
                          (e.DateCreated.Date >= startDate.Date && e.DateCreated.Date <= endDate.Date) &&
                          (e.Category.ToString().ToLower().Contains(searchFilterLower) ||
                           e.EmailAddress.ToLower().Contains(searchFilterLower) ||
                           e.Subject.ToLower().Contains(searchFilterLower) ||
                           e.MessageContents.ToLower().Contains(searchFilterLower) ||
                           e.TableReference.ToString().ToLower().Contains(searchFilterLower) ||
                           e.RecordReference.ToString().ToLower().Contains(searchFilterLower) ||
                           e.ErrorCode.ToString().ToLower().Contains(searchFilterLower) ||
                           e.ErrorMessage.ToLower().Contains(searchFilterLower))
                           orderby e.DateCreated descending
                          select e).ToList();
            }
            else
            {
                result = (from e in DB.GetTable<EmailLog>()
                          where (e.DateCreated.Date >= startDate.Date && e.DateCreated.Date <= endDate.Date) &&
                          (e.Category.ToString().ToLower().Contains(searchFilterLower) ||
                           e.EmailAddress.ToLower().Contains(searchFilterLower) ||
                           e.Subject.ToLower().Contains(searchFilterLower) ||
                           e.MessageContents.ToLower().Contains(searchFilterLower) ||
                           e.TableReference.ToString().ToLower().Contains(searchFilterLower) ||
                           e.RecordReference.ToString().ToLower().Contains(searchFilterLower) ||
                           e.ErrorCode.ToString().ToLower().Contains(searchFilterLower) ||
                           e.ErrorMessage.ToLower().Contains(searchFilterLower))
                          orderby e.DateCreated descending
                          select e).ToList();
            }
            return result;
        }

        public List<EmailLog> GetEmailLogForOrganizationForDay(Guid organizationId, DateTime day, string category)
        {
            string categoryLower = category == null ? string.Empty : category.ToLower();
            List<EmailLog> result = (from e in DB.GetTable<EmailLog>()
                                     where (e.OrganizationId.HasValue && e.OrganizationId.Value == organizationId) &&
                                     e.Category.ToLower().Equals(categoryLower) &&
                                     e.DateCreated.Date == day.Date
                                     orderby e.DateCreated descending
                                     select e).ToList();
            return result;
        }

        public void DeleteEmailLogByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> organizationId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<EmailLog> q = GetEmailLogByFilter(searchFilter, startDate, endDate, organizationId);
                DB.GetTable<EmailLog>().DeleteAllOnSubmit(q);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        public EmailLog LogEmailNotification(
            int errorCode,
            string errorMessage,
            string category,
            string emailAddress,
            string subject,
            string messageContents,
            string tableReference,
            Nullable<Guid> recordReference,
            string tag,
            int emailProviderCode,
            Nullable<Guid> organizationId)

        {
            EmailLog result = null;
            using (TransactionScope t = new TransactionScope())
            {
                result = new EmailLog()
                {
                    EmailLogId = Guid.NewGuid(),
                    ErrorCode = errorCode,
                    ErrorMessage = errorMessage,
                    Category = category,
                    EmailAddress = emailAddress,
                    Subject = subject,
                    MessageContents = messageContents,
                    TableReference = tableReference,
                    RecordReference = recordReference,
                    Tag = tag,
                    EmailProviderCode = emailProviderCode,
                    OrganizationId = organizationId,
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