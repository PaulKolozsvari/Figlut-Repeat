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
        #region Invoice Item

        public InvoiceItem GetInvoiceItem(Guid invoiceItemId, bool throwExceptionOnNotFound)
        {
            InvoiceItem result = (from oi in DB.GetTable<InvoiceItem>()
                                where oi.InvoiceItemId == invoiceItemId
                                select oi).FirstOrDefault();
            if(result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(InvoiceItem).Name,
                    EntityReader<InvoiceItem>.GetPropertyName(p => p.InvoiceItemId, false),
                    invoiceItemId.ToString()));
            }
            return result;
        }

        public long GetAllInvoiceItemCount()
        {
            return DB.GetTable<InvoiceItem>().LongCount();
        }

        public List<InvoiceItem> GetInvoiceItemsByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> InvoiceId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<InvoiceItem> result = null;
            if (InvoiceId.HasValue)
            {
                result = (from oi in DB.GetTable<InvoiceItem>()
                          where oi.InvoiceId == InvoiceId.Value &&
                          (oi.DateCreated.Date >= startDate.Date && oi.DateCreated.Date <= endDate.Date) &&
                          oi.ProductName.ToLower().Contains(searchFilterLower)
                          select oi).ToList();
            }
            else
            {
                result = (from oi in DB.GetTable<InvoiceItem>()
                          where (oi.DateCreated.Date >= startDate.Date && oi.DateCreated.Date <= endDate.Date) &&
                          oi.ProductName.ToLower().Contains(searchFilterLower)
                          select oi).ToList();
            }
            return result;
        }

        public void DeleteInvoicesItemsByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> InvoiceId)
        {
            using(TransactionScope t = new TransactionScope())
            {
                List<InvoiceItem> q = GetInvoiceItemsByFilter(searchFilter, startDate, endDate, InvoiceId);
                DB.GetTable<InvoiceItem>().DeleteAllOnSubmit(q);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Invoice Item
    }
}