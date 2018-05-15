namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class UserModel
    {
        #region Properties

        #region User

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmailAddress { get; set; }

        public string UserPassword { get; set; }

        public Nullable<Guid> OrganizationId { get; set; }

        public UserRole Role { get; set; }

        public Nullable<DateTime> LastLoginDate { get; set; }

        public Nullable<DateTime> LastActivityDate { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //User

        #region Organization

        public string OrganizationName { get; set; }

        public string OrganizationIdentifier { get; set; }

        public string OrganizationEmailAddress { get; set; }

        public string OrganizationAddress { get; set; }

        public DateTime OrganizationDateCreated { get; set; }

        #endregion //Organization

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.UserName))
            {
                errorMessage = string.Format("{0} not entered", EntityReader<UserModel>.GetPropertyName(p => p.UserName, true));
            }
            else if (string.IsNullOrEmpty(this.UserEmailAddress))
            {
                errorMessage = string.Format("{0} not entered", EntityReader<UserModel>.GetPropertyName(p => p.UserEmailAddress, true));
            }
            else if (!DataShaper.IsValidEmail(this.UserEmailAddress))
            {
                errorMessage = string.Format("{0} is not a  valid email address.", EntityReader<UserModel>.GetPropertyName(p => p.UserEmailAddress, true));
            }
            else if (string.IsNullOrEmpty(this.UserPassword))
            {
                errorMessage = string.Format("{0} not entered", EntityReader<UserModel>.GetPropertyName(p => p.UserPassword, true));
            }
            if (this.Role == UserRole.None)
            {
                errorMessage = string.Format("{0} not entered", EntityReader<UserModel>.GetPropertyName(p => p.Role, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromUser(User user, Organization organization)
        {
            this.UserId = user.UserId;
            this.UserName = user.UserName;
            this.UserEmailAddress = user.EmailAddress;
            this.UserPassword = user.Password;
            this.OrganizationId = user.OrganizationId;
            this.Role = ((UserRole)user.RoleId);
            this.LastLoginDate = user.LastLoginDate;
            this.LastActivityDate = user.LastActivityDate;
            this.DateCreated = user.DateCreated;
            if (organization != null)
            {
                CopyPropertiesFromOrganization(organization);
            }
        }

        public void CopyPropertiesFromOrganization(Organization organization)
        {
            this.OrganizationId = organization.OrganizationId;
            this.OrganizationName = organization.Name;
            this.OrganizationIdentifier = organization.Identifier;
            this.OrganizationEmailAddress = organization.EmailAddress;
            this.OrganizationAddress = organization.Address;
            this.OrganizationDateCreated = organization.DateCreated;
        }

        public void CopyPropertiesToUser(User user)
        {
            user.UserId = this.UserId;
            user.UserName = this.UserName;
            user.EmailAddress = this.UserEmailAddress;
            user.Password = this.UserPassword;
            user.OrganizationId = this.OrganizationId;
            //UserRole userRole;
            //if (!Enum.TryParse<UserRole>(this.Role, out userRole))
            //{
            //    throw new ArgumentException(string.Format("Could not convert {0} of '{1}' to a {2}.",
            //        EntityReader<UserModel>.GetPropertyName(p => p.Role, false),
            //        this.Role,
            //        typeof(UserRole).Name));
            //}
            user.RoleId = (int)this.Role;
            user.LastLoginDate = this.LastLoginDate;
            user.LastActivityDate = this.LastActivityDate;
            user.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}