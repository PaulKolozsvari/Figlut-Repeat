namespace Figlut.Spread.Receiver.Web.Service
{
    #region Using Directives

    using Figlut.Spread.Data;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Selectors;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class UserValidator : UserNamePasswordValidator
    {
        #region Methods

        public override void Validate(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("UserName not entered.");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Password not entered.");
            }
            SpreadEntityContext context = SpreadEntityContext.Create();
            if (!context.IsUserAuthenticated(userName, password))
            {
                throw new FaultException("Invalid UserName/Password.");
            }
        }

        #endregion //MethodsB
    }
}
