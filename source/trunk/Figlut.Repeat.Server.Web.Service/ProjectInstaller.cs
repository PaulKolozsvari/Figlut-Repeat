namespace Figlut.Repeat.Web.Service
{
    #region Using Directives

    using Figlut.Server.Toolkit.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    #endregion //Using Directives

    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        #region Constructors

        public ProjectInstaller()
        {
            InitializeComponent();
        }

        #endregion //Constructors

        #region Constants

        private const string DEFAULT_EVENT_SOURCE_NAME = "FRepeat.Source";
        private const string DEFAULT_EVENT_LOG_NAME = "FRepeat.Log";

        #endregion //Constants

        #region Methods

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            try
            {
                base.OnBeforeInstall(savedState);
                if (!EventLog.SourceExists(DEFAULT_EVENT_SOURCE_NAME))
                {
                    EventLog.CreateEventSource(DEFAULT_EVENT_SOURCE_NAME, DEFAULT_EVENT_LOG_NAME);
                }
            }
            catch (Exception ex)
            {
                UIHelper.DisplayException(ex);
            }
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            try
            {
                if (EventLog.SourceExists(DEFAULT_EVENT_SOURCE_NAME))
                {
                    EventLog.DeleteEventSource(DEFAULT_EVENT_SOURCE_NAME);
                }
                if (EventLog.Exists(DEFAULT_EVENT_LOG_NAME))
                {
                    EventLog.Delete(DEFAULT_EVENT_LOG_NAME);
                }
                base.OnAfterUninstall(savedState);
            }
            catch (Exception ex)
            {
                UIHelper.DisplayException(ex);
            }
        }

        #endregion //Methods
    }
}
