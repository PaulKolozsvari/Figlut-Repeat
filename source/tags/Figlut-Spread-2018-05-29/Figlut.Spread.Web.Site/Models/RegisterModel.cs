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

    public class RegisterModel
    {
        #region Properties

        #region Organization

        public string OrganizationName { get; set; }

        public string OrganizationIdentifier { get; set; }

        public string OrganizationEmailAddress { get; set; }

        public string OrganizationAddress { get; set; }

        #endregion //Organization

        #region User

        public string UserName { get; set; }

        public string UserEmailAddress { get; set; }

        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        #endregion //User

        #region Other

        public int OrganizationIdentifierMaxLength { get; set; }

        #endregion //Other

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage, int organizationIdentifierMaxLength)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.OrganizationName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RegisterModel>.GetPropertyName(p => p.OrganizationName, true));
            }
            else if (string.IsNullOrEmpty(this.OrganizationIdentifier))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RegisterModel>.GetPropertyName(p => p.OrganizationIdentifier, true));
            }
            else if (this.OrganizationIdentifier.Length > organizationIdentifierMaxLength)
            {
                errorMessage = string.Format("{0} must be shorter than {1} characters.", EntityReader<RegisterModel>.GetPropertyName(p => p.OrganizationIdentifier, true), organizationIdentifierMaxLength);
            }
            else if (this.OrganizationIdentifier.Contains(' '))
            {
                errorMessage = string.Format("{0} must not contain spaces.", EntityReader<RegisterModel>.GetPropertyName(p => p.OrganizationIdentifier, true));
            }
            else if (string.IsNullOrEmpty(this.OrganizationEmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RegisterModel>.GetPropertyName(p => p.OrganizationEmailAddress, true));
            }
            else if (!DataShaper.IsValidEmail(this.OrganizationEmailAddress))
            {
                errorMessage = string.Format("{0} is not a  valid email address.", EntityReader<RegisterModel>.GetPropertyName(p => p.OrganizationEmailAddress, true));
            }
            else if (string.IsNullOrEmpty(this.OrganizationAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RegisterModel>.GetPropertyName(p => p.OrganizationAddress, true));
            }
            else if (string.IsNullOrEmpty(this.UserName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RegisterModel>.GetPropertyName(p => p.UserName, true));
            }
            else if (string.IsNullOrEmpty(this.UserPassword))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RegisterModel>.GetPropertyName(p => p.UserPassword, true));
            }
            else if (string.IsNullOrEmpty(this.UserEmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<RegisterModel>.GetPropertyName(p => p.UserEmailAddress, true));
            }
            else if (!DataShaper.IsValidEmail(this.UserEmailAddress))
            {
                errorMessage = string.Format("{0} is not a  valid email address.", EntityReader<RegisterModel>.GetPropertyName(p => p.UserEmailAddress, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromUser(User user)
        {
            //this.UserId = user.UserId;
            this.UserName = user.UserName;
            this.UserEmailAddress = user.EmailAddress;
            this.UserPassword = user.Password;
            //this.OrganizationId = user.OrganizationId;
            //this.RoleId = user.RoleId;
            //this.DateCreated = user.DateCreated;
        }

        public void CopyPropertiesFromOrganization(Organization organization)
        {
            //this.OrganizationId = organization.OrganizationId;
            this.OrganizationName = organization.Name;
            this.OrganizationIdentifier = organization.Identifier;
            this.OrganizationEmailAddress = organization.EmailAddress;
            this.OrganizationAddress = organization.Address;
            //this.DateCreated = organization.DateCreated;
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

        public User CreateUser(Organization organization, UserRole role)
        {
            return new User()
            {
                UserId = Guid.NewGuid(),
                UserName = this.UserName,
                EmailAddress = this.UserEmailAddress,
                Password = this.UserPassword,
                OrganizationId = organization.OrganizationId,
                RoleId = (int)role,
                DateCreated = DateTime.Now
            };
        }

        #endregion //Methods
    }
}