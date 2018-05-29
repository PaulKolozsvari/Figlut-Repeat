namespace Figlut.Spread.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.ORM.Views;
    using Figlut.Spread.SMS;
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class SpreadEntityContext : EntityContext
    {
        #region Constructors

        public SpreadEntityContext(
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

        public static SpreadEntityContext Create()
        {
            return new SpreadEntityContext(
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