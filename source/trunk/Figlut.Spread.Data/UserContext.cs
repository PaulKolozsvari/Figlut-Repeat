namespace Figlut.Spread.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class SpreadEntityContext : EntityContext
    {
        #region User

        public User GetUser(Guid userId, bool throwExceptionOnNotFound)
        {
            List<User> q = (from u in DB.GetTable<User>()
                            where u.UserId == userId
                            select u).ToList();
            User result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.UserId, false),
                    userId));
            }
            return result;
        }

        public bool IsUserAuthenticated(string identifier, string password)
        {
            string identifierLower = identifier.ToLower();
            List<User> q = (from u in DB.GetTable<User>()
                            where (u.UserName.ToLower() == identifierLower || u.EmailAddress.ToLower() == identifierLower) && u.Password == password
                            select u).ToList();
            return q.Count > 0;
        }

        public User GetUserByUserName(string userName, bool throwExceptionOnNotFound)
        {
            string userNameLower = userName.ToLower();
            User result = null;
            List<User> q = (from u in DB.GetTable<User>()
                            where u.UserName.ToLower() == userNameLower
                            select u).ToList();
            result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.UserName, false),
                    userName));
            }
            return result;
        }

        public User GetUserByEmailAddress(string emailAddress, bool throwExceptionOnNotFound)
        {
            string emailAddressLower = emailAddress.ToLower();
            User result = null;
            List<User> q = (from u in DB.GetTable<User>()
                            where u.EmailAddress.ToLower() == emailAddressLower
                            select u).ToList();
            result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.EmailAddress, false),
                    emailAddress));
            }
            return result;
        }

        public bool AnotherUserWithUserNameOrEmailAddressExists(string userName, string emailAddress, out string errorMessage)
        {
            errorMessage = null;
            User user = GetUserByUserName(userName, false);
            if (user != null)
            {
                errorMessage = string.Format("A {0} with the {1} of '{2}' already exists.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.UserName, true),
                    userName);
            }
            user = GetUserByEmailAddress(emailAddress, false);
            if (user != null)
            {
                errorMessage = string.Format("A {0} with the {1} of '{2}' already exists.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.EmailAddress, true),
                    emailAddress);
            }
            return !string.IsNullOrEmpty(errorMessage);
        }

        /// <summary>
        /// Gets the user by an identifier i.e. either username or email address.
        /// </summary>
        public User GetUserByIdentifier(string identifier, bool throwExceptionOnNotFound)
        {
            string identifierLower = identifier.ToLower();
            User result = null;
            List<User> q = (from u in DB.GetTable<User>()
                            where u.UserName.ToLower() == identifierLower || u.EmailAddress.ToLower() == identifierLower
                            select u).ToList();
            result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} or {2} of '{3}'.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.UserName, false),
                    EntityReader<User>.GetPropertyName(p => p.EmailAddress, false),
                    identifier));
            }
            return result;
        }

        public bool IsUserOfRole(string userIdentifier, UserRole roleToCheck)
        {
            User user = GetUserByIdentifier(userIdentifier, true);
            UserRole userRole = (UserRole)user.RoleId;
            return (userRole & roleToCheck) == roleToCheck;
        }

        public bool UserExistsByUserName(string userName)
        {
            User user = GetUserByUserName(userName, false);
            return user != null;
        }

        public bool UserExistsByEmailAddress(string emailAddress)
        {
            User user = GetUserByEmailAddress(emailAddress, false);
            return user != null;
        }

        public long GetAllUserCount()
        {
            return DB.GetTable<User>().LongCount();
        }

        public List<User> GetUsersOfRole(UserRole role)
        {
            int roleId = (int)role;
            return (from u in DB.GetTable<User>()
                    where (u.RoleId & roleId) == roleId
                    select u).ToList();
        }

        public List<User> GetUsersByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<User> result = null;
            if (organizationId.HasValue)
            {
                result = (from user in DB.GetTable<User>()
                          join organization in DB.GetTable<Organization>() on user.OrganizationId equals organization.OrganizationId into setOrganization
                          from organizationView in setOrganization.DefaultIfEmpty()
                          where organizationView.OrganizationId == organizationId.Value &&
                          (user.UserName.ToLower().Contains(searchFilterLower) ||
                          user.EmailAddress.ToLower().Contains(searchFilterLower) ||
                          organizationView.Name.ToLower().Contains(searchFilterLower))
                          orderby user.UserName, user.EmailAddress, user.DateCreated
                          select user).ToList();
            }
            else
            {
                result = (from user in DB.GetTable<User>()
                          join organization in DB.GetTable<Organization>() on user.OrganizationId equals organization.OrganizationId into setOrganization
                          from organizationView in setOrganization.DefaultIfEmpty()
                          where (user.UserName.ToLower().Contains(searchFilterLower) ||
                          user.EmailAddress.ToLower().Contains(searchFilterLower) ||
                          organizationView.Name.ToLower().Contains(searchFilterLower))
                          orderby user.UserName, user.EmailAddress, user.DateCreated
                          select user).ToList();
            }
            return result;
        }

        public List<User> GetUsersByFilter(string searchFilter, Guid userIdToExclude, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<User> result = null;
            if (organizationId.HasValue)
            {
                result = (from user in DB.GetTable<User>()
                          join organization in DB.GetTable<Organization>() on user.OrganizationId equals organization.OrganizationId into setOrganization
                          from organizationView in setOrganization.DefaultIfEmpty()
                          where organizationView.OrganizationId == organizationId.Value &&
                          user.UserId != userIdToExclude &&
                          (user.UserName.ToLower().Contains(searchFilterLower) ||
                          user.EmailAddress.ToLower().Contains(searchFilterLower) ||
                          organizationView.Name.ToLower().Contains(searchFilterLower))
                          orderby user.UserName, user.EmailAddress, user.DateCreated
                          select user).ToList();
            }
            else
            {
                result = (from user in DB.GetTable<User>()
                          join organization in DB.GetTable<Organization>() on user.OrganizationId equals organization.OrganizationId into setOrganization
                          from organizationView in setOrganization.DefaultIfEmpty()
                          where user.UserId != userIdToExclude &&
                          (user.UserName.ToLower().Contains(searchFilterLower) ||
                          user.EmailAddress.ToLower().Contains(searchFilterLower) ||
                          organizationView.Name.ToLower().Contains(searchFilterLower))
                          orderby user.UserName, user.EmailAddress, user.DateCreated
                          select user).ToList();
            }
            return result;
        }

        public void DeleteUsersByFilter(string searchFilter, User currentUser, Nullable<Guid> organizationId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<User> users = GetUsersByFilter(searchFilter, currentUser.UserId, organizationId);
                DB.GetTable<User>().DeleteAllOnSubmit(users);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //User
    }
}