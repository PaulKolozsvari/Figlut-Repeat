namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class EditUserPasswordModel
    {
        #region Properties

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (this.UserId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<EditUserPasswordModel>.GetPropertyName(p => p.UserId, true));
            }
            if (string.IsNullOrEmpty(this.NewPassword))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<EditUserPasswordModel>.GetPropertyName(p => p.NewPassword, true));
            }
            else if (string.IsNullOrEmpty(this.ConfirmNewPassword))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<EditUserPasswordModel>.GetPropertyName(p => p.ConfirmNewPassword, true));
            }
            else if (this.NewPassword != this.ConfirmNewPassword)
            {
                errorMessage = "The new passwords do not match.";
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        #endregion //Methods
    }
}