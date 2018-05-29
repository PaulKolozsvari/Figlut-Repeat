namespace Figlut.Spread.Web.Service
{
    #region Using Directives

    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Spread.Web.Service.Configuration;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public partial class SpreadService : ServiceBase
    {
        #region Constructors

        public SpreadService()
        {
            InitializeComponent();
        }

        #endregion //Constructors

        #region Methods

        protected override void OnStart(string[] args)
        {
            try
            {
                Start(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                throw ex;
            }
        }

        protected override void OnStop()
        {
            try
            {
                Stop();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                throw ex;
            }
        }

        internal static void Start(bool initializeServiceHost)
        {
            SpreadApp.Instance.Initialize(initializeServiceHost);
        }

        internal static new void Stop()
        {
            if (GOC.Instance.GetByTypeName<ServiceHost>() != null)
            {
                GOC.Instance.GetByTypeName<ServiceHost>().Close();
                GOC.Instance.Logger.LogMessage(new LogMessage(
                    string.Format("{0} stopped.", SpreadApp.Instance.Settings.ApplicationName),
                    LogMessageType.Information,
                    LoggingLevel.Normal));
            }
        }

        #endregion //Methods
    }
}
