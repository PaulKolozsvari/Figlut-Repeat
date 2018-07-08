namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Repeat.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class ChangeUserPasswordModel
    {
        #region Properties

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(User user, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.CurrentPassword))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<ChangeUserPasswordModel>.GetPropertyName(p => p.CurrentPassword, true));
            }
            else if (string.IsNullOrEmpty(this.NewPassword))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<ChangeUserPasswordModel>.GetPropertyName(p => p.NewPassword, true));
            }
            else if (string.IsNullOrEmpty(this.ConfirmNewPassword))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<ChangeUserPasswordModel>.GetPropertyName(p => p.ConfirmNewPassword, true));
            }
            else if (this.NewPassword != this.ConfirmNewPassword)
            {
                errorMessage = "The new passwords do not match.";
            }
            if (user.Password != this.CurrentPassword)
            {
                errorMessage = string.Format("{0} is invalid.", EntityReader<ChangeUserPasswordModel>.GetPropertyName(p => p.CurrentPassword, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        #endregion //Methods
    }
}