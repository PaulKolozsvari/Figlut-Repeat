namespace Figlut.Repeat.Desktop.UI
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Repeat.Desktop.UI.Base;
    using Figlut.Repeat.Desktop.Utilities.Configuration;
    using Figlut.Repeat.ORM;
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

    public partial class ChangePasswordForm : FiglutBaseForm
    {
        #region Constructors

        public ChangePasswordForm(User entity)
        {
            InitializeComponent();
            base.SetBarHeights(FiglutDesktopManagerApp.Instance.Settings.FormBarHeight);
            _entity = entity;
        }

        #endregion //Constructors

        #region Fields

        private User _entity;

        #endregion //Fields

        #region Methods

        private void ValidateInputFields()
        {
            if (txtPassword.Text != txtReenterPassword.Text)
            {
                txtPassword.Focus();
                throw new UserThrownException("Passwords do not match.", LoggingLevel.None);
            }
        }

        private void SaveEntity()
        {
            using (WaitProcess w = new WaitProcess(mnuMain, this))
            {
                w.ChangeStatus(string.Format("Checking for existing {0} ...", typeof(User).Name));
                User user = FiglutDesktopManagerApp.Instance.RestClient.GetEntityById<User>(_entity.UserId);
                if (user == null)
                {
                    throw new Exception(string.Format("Could not find {0} with {1} of '{2}'.",
                        typeof(User).Name,
                        EntityReader<User>.GetPropertyName(p => p.UserId, false),
                        _entity.UserId));
                }
                user.Password = txtPassword.Text;
                w.ChangeStatus(string.Format("Saving {0} ...", typeof(User).Name));
                FiglutDesktopManagerApp.Instance.RestClient.PutEntity<User>(user);

                if (FiglutDesktopManagerApp.Instance.CurrentUser.UserId == user.UserId)
                {
                    FiglutDesktopManagerApp.Instance.Login(w, user.UserName, user.Password);
                }
            }
        }

        #endregion //Methods

        #region Event Handlers

        private void ChangePasswordForm_Load(object sender, EventArgs e)
        {

        }

        private void ChangePasswordForm_MouseDown(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseDown(sender, e);
        }

        private void ChangePasswordForm_MouseMove(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseMove(sender, e);
        }

        private void ChangePasswordForm_MouseUp(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseUp(sender, e);
        }

        private void mnuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            ValidateInputFields();
            SaveEntity();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void ChangePasswordForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                mnuCancel.PerformClick();
            }
            else if ((e.KeyCode == Keys.Enter) & e.Control & e.Shift)
            {
                mnuSave.PerformClick();
            }
        }

        #endregion //Event Handlers
    }
}