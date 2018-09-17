namespace Figlut.Repeat.Data
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Repeat.ORM;

    #endregion //Using Directives

    public partial class RepeatEntityContext : EntityContext
    {
        #region Invoice

        public Invoice GetInvoice(Guid invoiceId, bool throwExceptionOnNotFound)
        {
            Invoice result = (from o in DB.GetTable<Invoice>()
                            where o.InvoiceId == invoiceId
                            select o).FirstOrDefault();
            if(result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Invoice).Name,
                    EntityReader<Invoice>.GetPropertyName(p => p.InvoiceId, false),
                    invoiceId.ToString()));
            }
            return result;
        }

        public Invoice GetInvoiceByInvoiceNumber(long invoiceNumber, bool throwExceptionOnNotFound)
        {
            Invoice result = (from o in DB.GetTable<Invoice>()
                            where o.InvoiceNumber == invoiceNumber
                            select o).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Invoice).Name,
                    EntityReader<Invoice>.GetPropertyName(p => p.InvoiceNumber, false),
                    invoiceNumber.ToString()));
            }
            return result;
        }

        public long GetAllInvoiceCount()
        {
            return DB.GetTable<Invoice>().LongCount();
        }

        public List<Invoice> GetInvoicesByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<Invoice> result = null;
            if(organizationId.HasValue)
            {
                result = (from o in DB.GetTable<Invoice>()
                          where o.OrganizationId == organizationId &&
                          (o.DateCreated.Date >= startDate.Date && o.DateCreated.Date <= endDate.Date) &&
                          o.InvoiceNumber.ToString().ToLower().Contains(searchFilterLower) &&
                          o.CreatedByUserName.ToLower().Contains(searchFilterLower)
                          select o).ToList();
            }
            else
            {
                result = (from o in DB.GetTable<Invoice>()
                          where (o.DateCreated.Date >= startDate.Date && o.DateCreated.Date <= endDate.Date) &&
                          o.InvoiceNumber.ToString().ToLower().Contains(searchFilterLower) &&
                          o.CreatedByUserName.ToLower().Contains(searchFilterLower)
                          select o).ToList();
            }
            return result;
        }

        public void DeleteInvoicesByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> organizationId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<Invoice> q = GetInvoicesByFilter(searchFilter, startDate, endDate, organizationId);
                DB.GetTable<Invoice>().DeleteAllOnSubmit(q);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Invoice
    }
}
