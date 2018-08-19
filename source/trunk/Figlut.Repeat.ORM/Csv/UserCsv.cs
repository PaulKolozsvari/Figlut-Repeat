namespace Figlut.Repeat.ORM.Csv
{
    #region Using Directives

    using Figlut.Repeat.ORM.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class UserCsv
    {
        #region Properties

        #region User

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmailAddress { get; set; }

        public bool UserEnableEmailNotifications { get; set; }

        public string UserPassword { get; set; }

        public string Role { get; set; }

        public Nullable<DateTime> LastLoginDate { get; set; }

        public DateTime DateCreated { get; set; }

        public Nullable<Guid> OrganizationId { get; set; }

        #endregion //User

        #region Organization

        public string OrganizationName { get; set; }

        public string OrganizationIdentifier { get; set; }

        public string OrganizationPrimaryContactEmailAddress { get; set; }

        public string OrganizationAddress { get; set; }

        public DateTime OrganizationDateCreated { get; set; }

        #endregion //Organization

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromUser(User user, Organization organization)
        {
            this.UserId = user.UserId;
            this.UserName = user.UserName;
            this.UserEmailAddress = user.EmailAddress;
            this.UserEnableEmailNotifications = user.EnableEmailNotifications;
            this.UserPassword = user.Password;
            this.OrganizationId = user.OrganizationId;
            this.Role = ((UserRole)user.RoleId).ToString();
            this.LastLoginDate = user.LastLoginDate;
            this.DateCreated = user.DateCreated;
            if (organization != null)
            {
                CopyPropertiesFromOrganization(organization);
            }
        }

        public void CopyPropertiesFromOrganization(Organization organization)
        {
            this.OrganizationName = organization.Name;
            this.OrganizationIdentifier = organization.Identifier;
            this.OrganizationPrimaryContactEmailAddress = organization.PrimaryContactEmailAddress;
            this.OrganizationAddress = organization.Address;
            this.OrganizationDateCreated = organization.DateCreated;
        }

        public void CopyPropertiesToUser(User user)
        {
            user.UserId = this.UserId;
            user.UserName = this.UserName;
            user.EmailAddress = this.UserEmailAddress;
            user.EnableEmailNotifications = this.UserEnableEmailNotifications;
            user.Password = this.UserPassword;
            user.OrganizationId = this.OrganizationId;
            UserRole userRole;
            if (Enum.TryParse<UserRole>(this.Role, out userRole))
            {
                user.RoleId = (int)userRole;
            }
            user.LastLoginDate = this.LastLoginDate;
            user.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}
