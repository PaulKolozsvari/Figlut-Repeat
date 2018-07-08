namespace Figlut.Repeat.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Helpers;
    using Figlut.Repeat.ORM.Views;
    using Figlut.Repeat.SMS;
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class RepeatEntityContext : EntityContext
    {
        #region Constructors

        public RepeatEntityContext(
            DataContext db,
            LinqFunnelSettings settings,
            bool handleExceptions,
            Type userLinqToSqlType,
            Type serverActionLinqToSqlType,
            Type serverErrorLinqToSqlType)
            : base(db, settings, handleExceptions, userLinqToSqlType, serverActionLinqToSqlType, serverErrorLinqToSqlType)
        {
        }

        #endregion //Constructors

        #region Methods

        #region Factory Methods

        public static RepeatEntityContext Create()
        {
            return new RepeatEntityContext(
                GOC.Instance.GetNewLinqToSqlDataContext(),
                GOC.Instance.GetByTypeName<LinqFunnelSettings>(),
                false,
                GOC.Instance.UserLinqToSqlType,
                GOC.Instance.ServerActionLinqToSqlType,
                GOC.Instance.ServerErrorLinqToSqlType);
        }

        #endregion //Factory Methods

        #endregion //Methods
    }
}