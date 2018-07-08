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
        #region Organization

        public Organization GetOrganization(Guid organizationId, bool throwExceptionOnNotFound)
        {
            List<Organization> q = (from o in DB.GetTable<Organization>()
                                    where o.OrganizationId == organizationId
                                    select o).ToList();
            Organization result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Organization).Name,
                    EntityReader<Organization>.GetPropertyName(p => p.OrganizationId, false),
                    organizationId.ToString()));
            }
            return result;
        }

        public Organization GetOrganizationByIdentifier(string organizationIdentifier, bool throwExceptionOnNotFound)
        {
            string organizationIdentiferLower = organizationIdentifier.Trim().ToLower();
            List<Organization> q = (from o in DB.GetTable<Organization>()
                                    where o.Identifier.ToLower() == organizationIdentiferLower
                                    select o).ToList();
            Organization result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(Organization).Name,
                    EntityReader<Organization>.GetPropertyName(p => p.Identifier, false),
                    organizationIdentiferLower));
            }
            return result;
        }

        public bool OrganizationExistsByIdentifier(string organizationIdentifier)
        {
            Organization organization = GetOrganizationByIdentifier(organizationIdentifier, false);
            return organization != null;
        }

        public bool Register(Organization organization, User user, out string errorMessage)
        {
            errorMessage = null;
            using (TransactionScope t = new TransactionScope())
            {
                Organization originalOrganization = GetOrganizationByIdentifier(organization.Identifier, false);
                if (OrganizationExistsByIdentifier(organization.Identifier))
                {
                    errorMessage = string.Format("An {0} with the {1} of '{2}' already exists.",
                        typeof(Organization).Name,
                        EntityReader<Organization>.GetPropertyName(p => p.Identifier, false),
                        organization.Identifier);
                    return false;
                }
                if (UserExistsByUserName(user.UserName))
                {
                    errorMessage = string.Format("A {0} with the {1} of '{2}' already exists.",
                        typeof(User).Name,
                        EntityReader<User>.GetPropertyName(p => p.UserName, false),
                        user.UserName);
                    return false;
                }
                if (UserExistsByEmailAddress(user.EmailAddress))
                {
                    errorMessage = string.Format("A {0} with the {1} of '{2}' already exists.",
                        typeof(User).Name,
                        EntityReader<User>.GetPropertyName(p => p.EmailAddress, false),
                        user.EmailAddress);
                    return false;
                }
                DB.GetTable<Organization>().InsertOnSubmit(organization);
                DB.GetTable<User>().InsertOnSubmit(user);
                DB.SubmitChanges();
                t.Complete();
            }
            return true;
        }

        public long GetAllOrganizationCount()
        {
            return DB.GetTable<Organization>().LongCount();
        }

        public List<Organization> GetOrganizationsByFilter(string searchFilter)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<Organization> result = (from o in DB.GetTable<Organization>()
                                         where o.Identifier.ToLower().Contains(searchFilterLower) ||
                                         o.Name.ToLower().Contains(searchFilterLower) ||
                                         o.EmailAddress.ToLower().Contains(searchFilterLower)
                                         orderby o.Name, o.Identifier, o.EmailAddress, o.DateCreated
                                         select o).ToList();
            return result;
        }

        public void DeleteOrganizationsByFilter(string searchFilter)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<Organization> organizations = GetOrganizationsByFilter(searchFilter);
                DB.GetTable<Organization>().DeleteAllOnSubmit(organizations);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        public Organization DecrementSmsCreditFromOrganization(Guid organizationId)
        {
            Organization result = null;
            using (TransactionScope t = new TransactionScope())
            {
                result = GetOrganization(organizationId, true);
                result.SmsCreditsBalance--;
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        public Organization DeductSmsCreditsFromOrganization(Guid organizationId, long smsCreditsToDeduct)
        {
            if (smsCreditsToDeduct < 1)
            {
                throw new ArgumentOutOfRangeException(string.Format("Cannot deduct a negative number of SMS credits from organization with {0} of '{1}'.",
                    EntityReader<Organization>.GetPropertyName(p => p.OrganizationId, false),
                    organizationId));
            }
            Organization result = null;
            using (TransactionScope t = new TransactionScope())
            {
                result = GetOrganization(organizationId, true);
                result.SmsCreditsBalance -= smsCreditsToDeduct;
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        public Organization AddSmsCreditsToOrganization(Guid organizationId, long smsCreditsToAdd)
        {
            if (smsCreditsToAdd < 1)
            {
                throw new ArgumentOutOfRangeException(string.Format("Cannot add a negative number of SMS credits to organization with {0} of '{1}'.",
                    EntityReader<Organization>.GetPropertyName(p => p.OrganizationId, false),
                    organizationId));
            }
            Organization result = null;
            using (TransactionScope t = new TransactionScope())
            {
                result = GetOrganization(organizationId, true);
                result.SmsCreditsBalance += smsCreditsToAdd;
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        #endregion //Organization
    }
}