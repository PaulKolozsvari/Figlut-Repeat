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

        public List<User> GetUsersByFilter(string searchFilter)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<User> result = (from u in DB.GetTable<User>()
                                 join o in DB.GetTable<Organization>() on u.OrganizationId equals o.OrganizationId into set
                                 from uo in set.DefaultIfEmpty()
                                 where u.UserName.ToLower().Contains(searchFilterLower) ||
                                 u.EmailAddress.ToLower().Contains(searchFilterLower) ||
                                 uo.Name.ToLower().Contains(searchFilterLower)
                                 orderby u.UserName, u.EmailAddress, u.DateCreated
                                 select u).ToList();
            return result;
        }

        public List<User> GetUsersByFilter(string searchFilter, Guid userIdToExclude)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<User> result = (from u in DB.GetTable<User>()
                                 join o in DB.GetTable<Organization>() on u.OrganizationId equals o.OrganizationId into set
                                 from uo in set.DefaultIfEmpty()
                                 where u.UserId != userIdToExclude &&
                                 (u.UserName.ToLower().Contains(searchFilterLower) ||
                                 u.EmailAddress.ToLower().Contains(searchFilterLower) ||
                                 uo.Name.ToLower().Contains(searchFilterLower))
                                 orderby u.UserName, u.EmailAddress, u.DateCreated
                                 select u).ToList();
            return result;
        }

        public void DeleteUsersByFilter(string searchFilter, User currentUser)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<User> users = GetUsersByFilter(searchFilter, currentUser.UserId);
                DB.GetTable<User>().DeleteAllOnSubmit(users);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //User
    }
}