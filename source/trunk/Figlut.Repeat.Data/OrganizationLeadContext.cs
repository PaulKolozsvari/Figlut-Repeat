namespace Figlut.Repeat.Data
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Figlut.Repeat.ORM;
    using Figlut.Server.Toolkit.Data;
    using System.Transactions;
    using Figlut.Server.Toolkit.Data.DB.LINQ;

    #endregion //Using Directives

    public partial class RepeatEntityContext : EntityContext
    {
        #region OrganizationLead

        public OrganizationLead GetOrganizationLead(Guid organizationLeadId, bool throwExceptionOnNotFound)
        {
            List<OrganizationLead> q = (from s in DB.GetTable<OrganizationLead>()
                                        where s.OrganizationLeadId == organizationLeadId
                                        select s).ToList();
            OrganizationLead result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(OrganizationLead).Name,
                    EntityReader<OrganizationLead>.GetPropertyName(p => p.OrganizationLeadId, false),
                    organizationLeadId.ToString()));
            }
            return result;
        }

        public long GetAllOrganizationLeadsCount()
        {
            return DB.GetTable<OrganizationLead>().LongCount();
        }

        public long GetAllOrganizationLeadsCount(Nullable<Guid> organizationId)
        {
            if (!organizationId.HasValue)
            {
                return 0;
            }
            return DB.GetTable<OrganizationLead>().LongCount(p => p.OrganizationId == organizationId.Value);
        }

        public List<OrganizationLead> GetOrganizationLeadsByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<OrganizationLead> result = null;
            if (organizationId.HasValue)
            {
                result = (from lead in DB.GetTable<OrganizationLead>()
                          where lead.OrganizationId == organizationId.Value &&
                          (lead.Name.ToLower().Contains(searchFilterLower) ||
                          lead.PhoneNumber.ToLower().Contains(searchFilterLower) ||
                          lead.InternationPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          lead.WebsiteUrl.ToLower().Contains(searchFilterLower))
                          orderby lead.SearchLocationCentreName descending
                          select lead).ToList();
            }
            else
            {
                result = (from lead in DB.GetTable<OrganizationLead>()
                          where
                          (lead.Name.ToLower().Contains(searchFilterLower) ||
                          lead.PhoneNumber.ToLower().Contains(searchFilterLower) ||
                          lead.InternationPhoneNumber.ToLower().Contains(searchFilterLower) ||
                          lead.WebsiteUrl.ToLower().Contains(searchFilterLower))
                          orderby lead.SearchLocationCentreName descending
                          select lead).ToList();
            }
            return result;
        }

        public void DeleteOrganizationLeadsByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<OrganizationLead> subscriptions = GetOrganizationLeadsByFilter(searchFilter, organizationId);
                DB.GetTable<OrganizationLead>().DeleteAllOnSubmit(subscriptions);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //OrganizationLead
    }
}