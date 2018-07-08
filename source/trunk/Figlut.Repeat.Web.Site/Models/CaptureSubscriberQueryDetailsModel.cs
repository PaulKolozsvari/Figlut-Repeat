namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class CaptureSubscriberQueryDetailsModel
    {
        #region Properties

        public string SubscriberCellPhoneNumber { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.SubscriberCellPhoneNumber))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<CaptureSubscriberQueryDetailsModel>.GetPropertyName(p => p.SubscriberCellPhoneNumber, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        #endregion //Methods
    }
}