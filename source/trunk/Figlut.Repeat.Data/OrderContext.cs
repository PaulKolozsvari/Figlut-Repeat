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
        #region Order

        public Order GetOrder(Guid orderId, bool throwExceptionOnNotFound)
        {
            Order result = (from o in DB.GetTable<Order>()
                            where o.OrderId == orderId
                            select o).FirstOrDefault();
            if(result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Order).Name,
                    EntityReader<Order>.GetPropertyName(p => p.OrderId, false),
                    orderId.ToString()));
            }
            return result;
        }

        public Order GetOrderByOrderNumber(long orderNumber, bool throwExceptionOnNotFound)
        {
            Order result = (from o in DB.GetTable<Order>()
                            where o.OrderNumber == orderNumber
                            select o).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Order).Name,
                    EntityReader<Order>.GetPropertyName(p => p.OrderNumber, false),
                    orderNumber.ToString()));
            }
            return result;
        }

        public long GetAllOrderCount()
        {
            return DB.GetTable<Order>().LongCount();
        }

        public List<Order> GetOrdersByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<Order> result = null;
            if(organizationId.HasValue)
            {
                result = (from o in DB.GetTable<Order>()
                          where o.OrganizationId == organizationId &&
                          (o.DateCreated.Date >= startDate.Date && o.DateCreated.Date <= endDate.Date) &&
                          o.OrderNumber.ToString().ToLower().Contains(searchFilterLower) &&
                          o.CreatedByUserName.ToLower().Contains(searchFilterLower)
                          select o).ToList();
            }
            else
            {
                result = (from o in DB.GetTable<Order>()
                          where (o.DateCreated.Date >= startDate.Date && o.DateCreated.Date <= endDate.Date) &&
                          o.OrderNumber.ToString().ToLower().Contains(searchFilterLower) &&
                          o.CreatedByUserName.ToLower().Contains(searchFilterLower)
                          select o).ToList();
            }
            return result;
        }

        public void DeleteOrdersByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> organizationId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<Order> q = GetOrdersByFilter(searchFilter, startDate, endDate, organizationId);
                DB.GetTable<Order>().DeleteAllOnSubmit(q);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Order
    }
}
