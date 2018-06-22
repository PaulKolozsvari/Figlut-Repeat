namespace Figlut.Spread.Desktop.Manager
{
    using Figlut.Server.Toolkit.Utilities;
    #region Using Directives

    using Figlut.Spread.Desktop.UI;
    using Figlut.Spread.Desktop.Utilities.Configuration;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    #endregion //Using Directives

    static class Program
    {
        #region Fields

        private static MainForm _mainForm;

        #endregion //Fields

        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.ThreadException += Application_ThreadException;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                GOC.Instance.ShowMessageBoxOnException = true;
                if (!File.Exists(new FiglutDesktopManagerSettings().FilePath))
                {
                    FiglutDesktopManagerSettings.CreateDefaultSettings().SaveToFile();
                }
                _mainForm = new MainForm();
                Application.Run(_mainForm);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                if (_mainForm != null && !_mainForm.IsDisposed)
                {
                    _mainForm.ForceClose = true;
                }
                Application.Exit();
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (ExceptionHandler.HandleException(e.Exception))
            {
                _mainForm.ForceClose = true;
                Application.Exit();
            }
        }

        #endregion //Methods
    }
}
