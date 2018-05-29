namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class ComposeSmsStandaloneModel
    {
        #region Properties

        [DataType(DataType.PhoneNumber)]
        public string CellPhoneNumber { get; set; }

        //[DataType(DataType.MultilineText)]
        public string MessageContents { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.CellPhoneNumber))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<ComposeSmsStandaloneModel>.GetPropertyName(p => p.CellPhoneNumber, true));
            }
            else if (this.CellPhoneNumber.Length < 10)
            {
                errorMessage = string.Format("{0} must be at least 10 characters long.", EntityReader<ComposeSmsStandaloneModel>.GetPropertyName(p => p.CellPhoneNumber, true));
            }
            else if (string.IsNullOrEmpty(this.MessageContents))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<ComposeSmsStandaloneModel>.GetPropertyName(p => p.MessageContents, true));
            }
            else if (this.MessageContents.Length > 140)
            {
                errorMessage = string.Format("{0} may not be greater than 140 characters.", EntityReader<ComposeSmsStandaloneModel>.GetPropertyName(p => p.MessageContents, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        #endregion //Methods
    }
}