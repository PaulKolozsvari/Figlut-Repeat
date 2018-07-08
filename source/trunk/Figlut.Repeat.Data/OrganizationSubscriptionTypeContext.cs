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
        #region OrganizationSubscriptionType

        public OrganizationSubscriptionType GetOrganizationSubscriptionType(Guid organizationSubscriptionTypeId, bool throwExceptionOnNotFound)
        {
            OrganizationSubscriptionType result = (from p in DB.GetTable<OrganizationSubscriptionType>()
                                                   where p.OrganizationSubscriptionTypeId == organizationSubscriptionTypeId
                                                   select p).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(OrganizationSubscriptionType).Name,
                    EntityReader<OrganizationSubscriptionType>.GetPropertyName(p => p.OrganizationSubscriptionTypeId, false),
                    organizationSubscriptionTypeId.ToString()));
            }
            return result;
        }

        public OrganizationSubscriptionType GetOrganizationSubscriptionTypeByOrganizationSubscriptionTypeCode(string name, bool throwExceptionOnNotFound)
        {
            OrganizationSubscriptionType result = (from p in DB.GetTable<OrganizationSubscriptionType>()
                                                   where p.Name == name
                                                   select p).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(OrganizationSubscriptionType).Name,
                    EntityReader<OrganizationSubscriptionType>.GetPropertyName(p => p.Name, false),
                    name.ToString()));
            }
            return result;
        }

        public long GetAllOrganizationSubscriptionTypesCount()
        {
            return DB.GetTable<OrganizationSubscriptionType>().LongCount();
        }

        public List<OrganizationSubscriptionType> GetOrganizationSubscriptionTypesByFilter(string searchFilter)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<OrganizationSubscriptionType> result = (from p in DB.GetTable<OrganizationSubscriptionType>()
                                                         where p.Name.ToLower().Contains(searchFilterLower)
                                                         select p).ToList();
            return result;
        }

        public void DeleteOrganizationSubscriptionTypesByFilter(string searchFilter)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<OrganizationSubscriptionType> q = GetOrganizationSubscriptionTypesByFilter(searchFilter);
                DB.GetTable<OrganizationSubscriptionType>().DeleteAllOnSubmit(q);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //OrganizationSubscriptionType
    }
}
