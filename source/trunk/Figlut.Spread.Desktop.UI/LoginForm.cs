namespace Figlut.Spread.Desktop.UI
{
    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    #region Using Directives

    using Figlut.Spread.Desktop.UI.Base;
    using Figlut.Spread.Desktop.Utilities.Configuration;
    using Figlut.Spread.ORM;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    #endregion //Using Directives

    public partial class LoginForm : FiglutBaseForm
    {
        #region Constructors

        public LoginForm()
        {
            InitializeComponent();
        }

        #endregion //Constructors

        #region Event Handlers

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseDown(sender, e);
        }

        private void LoginForm_MouseMove(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseMove(sender, e);
        }

        private void LoginForm_MouseUp(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseUp(sender, e);
        }

        private void LoginForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                mnuCancel.PerformClick();
            }
            else if ((e.KeyCode == Keys.Enter) & e.Control & e.Shift)
            {
                mnuLogin.PerformClick();
            }
        }

        private void mnuLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserName.Text))
            {
                txtUserName.Focus();
                throw new UserThrownException(
                    string.Format("{0} not entered.", EntityReader<User>.GetPropertyName(p => p.UserName, true)),
                    LoggingLevel.None);
            }
            using (WaitProcess w = new WaitProcess(mnuMain, this))
            {
                w.ChangeStatus("Logging in ...");
                User user = FiglutDesktopManagerApp.Instance.Login(w, txtUserName.Text, txtPassword.Text);
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void mnuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        #endregion //Event Handlers
    }
}
