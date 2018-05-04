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
        #region Order Item

        public OrderItem GetOrderItem(Guid orderItemId, bool throwExceptionOnNotFound)
        {
            OrderItem result = (from oi in DB.GetTable<OrderItem>()
                                where oi.OrderItemId == orderItemId
                                select oi).FirstOrDefault();
            if(result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(OrderItem).Name,
                    EntityReader<OrderItem>.GetPropertyName(p => p.OrderItemId, false),
                    orderItemId.ToString()));
            }
            return result;
        }

        public long GetAllOrderItemCount()
        {
            return DB.GetTable<OrderItem>().LongCount();
        }

        public List<OrderItem> GetOrderItemsByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> orderId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<OrderItem> result = null;
            if (orderId.HasValue)
            {
                result = (from oi in DB.GetTable<OrderItem>()
                          where oi.OrderId == orderId.Value &&
                          (oi.DateCreated.Date >= startDate.Date && oi.DateCreated.Date <= endDate.Date) &&
                          oi.ProductName.ToLower().Contains(searchFilterLower)
                          select oi).ToList();
            }
            else
            {
                result = (from oi in DB.GetTable<OrderItem>()
                          where (oi.DateCreated.Date >= startDate.Date && oi.DateCreated.Date <= endDate.Date) &&
                          oi.ProductName.ToLower().Contains(searchFilterLower)
                          select oi).ToList();
            }
            return result;
        }

        public void DeleteOrdersItemsByFilter(string searchFilter, DateTime startDate, DateTime endDate, Nullable<Guid> orderId)
        {
            using(TransactionScope t = new TransactionScope())
            {
                List<OrderItem> q = GetOrderItemsByFilter(searchFilter, startDate, endDate, orderId);
                DB.GetTable<OrderItem>().DeleteAllOnSubmit(q);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Order Item
    }
}