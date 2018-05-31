namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class UserProfileModel
    {
        #region Properties

        #region User

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmailAddress { get; set; }

        public string UserCellPhoneNumber { get; set; }

        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        public Nullable<Guid> OrganizationId { get; set; }

        public int RoleId { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //User

        #region Organization

        public string OrganizationName { get; set; }

        public string OrganizationIdentifier { get; set; }

        [DataType(DataType.EmailAddress)]
        public string OrganizationEmailAddress { get; set; }

        public string OrganizationAddress { get; set; }

        public DateTime OrganizationDateCreated { get; set; }

        #endregion //Organization

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            string formattedPhoneNumber = null;
            if (string.IsNullOrEmpty(this.UserName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<UserProfileModel>.GetPropertyName(p => p.UserName, true));
            }
            else if (string.IsNullOrEmpty(this.UserEmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<UserProfileModel>.GetPropertyName(p => p.UserEmailAddress, true));
            }
            else if (!DataShaper.IsValidEmail(this.UserEmailAddress))
            {
                errorMessage = string.Format("{0} is not a  valid email address.", EntityReader<UserProfileModel>.GetPropertyName(p => p.UserEmailAddress, true));
            }
            else if (string.IsNullOrEmpty(this.UserCellPhoneNumber))
            {
                errorMessage = string.Format("{0} is not a  valid email address.", EntityReader<UserProfileModel>.GetPropertyName(p => p.UserCellPhoneNumber, true));
            }
            else if (!DataShaper.IsValidPhoneNumber(this.UserCellPhoneNumber, out formattedPhoneNumber))
            {
                errorMessage = string.Format("{0} is not a valid cell phone number.", this.UserCellPhoneNumber);
            }
            else if (string.IsNullOrEmpty(this.UserPassword))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<UserProfileModel>.GetPropertyName(p => p.UserPassword, true));
            }
            if (!this.OrganizationId.HasValue || this.OrganizationId == Guid.Empty)
            {
                errorMessage = string.Format("{0} has no value.", EntityReader<UserProfileModel>.GetPropertyName(p => p.OrganizationId, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromUser(User user, Organization organization)
        {
            this.UserId = user.UserId;
            this.UserName = user.UserName;
            this.UserEmailAddress = user.EmailAddress;
            this.UserCellPhoneNumber = user.CellPhoneNumber;
            this.UserPassword = user.Password;
            this.OrganizationId = user.OrganizationId;
            this.RoleId = user.RoleId;
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
            this.DateCreated = organization.DateCreated;
        }

        public void CopyPropertiesToUser(User user)
        {
            user.UserId = this.UserId;
            user.UserName = this.UserName;
            user.EmailAddress = this.UserEmailAddress;
            user.CellPhoneNumber = this.UserCellPhoneNumber;
            user.Password = this.UserPassword;
            user.OrganizationId = this.OrganizationId;
            user.RoleId = this.RoleId;
            user.DateCreated = this.DateCreated;
        }

        public Organization CreateOrganization()
        {
            return new Organization()
            {
                OrganizationId = Guid.NewGuid(),
                Name = this.OrganizationName,
                Identifier = this.OrganizationIdentifier,
                EmailAddress = this.OrganizationEmailAddress,
                Address = this.OrganizationAddress,
                DateCreated = DateTime.Now
            };
        }

        public User CreateUser(UserRole role)
        {
            return new User()
            {
                UserId = Guid.NewGuid(),
                UserName = this.UserName,
                EmailAddress = this.UserEmailAddress,
                CellPhoneNumber = this.UserCellPhoneNumber,
                Password = this.UserPassword,
                OrganizationId = this.OrganizationId,
                RoleId = (int)role,
                DateCreated = DateTime.Now
            };
        }

        #endregion //Methods
    }
}