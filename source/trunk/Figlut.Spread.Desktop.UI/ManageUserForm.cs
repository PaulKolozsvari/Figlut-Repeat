namespace Figlut.Spread.Desktop.UI
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
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

    public partial class ManageUserForm : FiglutBaseForm
    {
        #region Constructors

        public ManageUserForm(User entity)
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

        private void RefreshUI()
        {
            if (_entity != null)
            {
                this.FormTitle = string.Format("Update {0}", typeof(User).Name);
                this.Status = string.Format("Change {0} details and save.", typeof(User).Name);

                txtUserName.Text = _entity.UserName;
                txtPassword.Text = _entity.Password;
                txtEmailAddress.Text = _entity.EmailAddress;

                txtUserName.Enabled = false;
                txtPassword.Enabled = false;
            }
            else
            {
                FormTitle = string.Format("Add {0}", typeof(User).Name);
                this.Status = string.Format("Enter new {0} details and save.", typeof(User).Name);
            }
        }

        private void VerifyInputControls()
        {
            if (string.IsNullOrEmpty(txtUserName.Text))
            {
                txtUserName.Focus();
                throw new UserThrownException(string.Format("{0} not entered.", EntityReader<User>.GetPropertyName(p => p.UserName, true)), LoggingLevel.None);
            }   
            if (string.IsNullOrEmpty(txtEmailAddress.Text))
            {
                txtEmailAddress.Focus();
                throw new UserThrownException(string.Format("{0} not entered.", EntityReader<User>.GetPropertyName(p => p.EmailAddress, true)), LoggingLevel.None);
            }
        }

        private void ValidateUserName(WaitProcess w)
        {
            w.ChangeStatus(string.Format("Checking for existing {0} with the same {1} ...", 
                typeof(User).Name, 
                EntityReader<User>.GetPropertyName(p => p.UserName, true)));

            List<User> userNameQuery = FiglutDesktopManagerApp.Instance.RestClient.GetEntitiesByField<User>(
                EntityReader<User>.GetPropertyName(p => p.UserName, false),
                txtUserName.Text);

            if (userNameQuery.Count > 0)
            {
                txtUserName.Focus();
                throw new UserThrownException(string.Format("There is an existing {0} with the {1} of '{2}'.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.UserName, true),
                    txtUserName.Text),
                    LoggingLevel.None);
            }
        }

        private void ValidateEmailAddress(WaitProcess w)
        {
            w.ChangeStatus(string.Format("Checking for existing {0} with the same {1} ...", 
                typeof(User).Name, 
                EntityReader<User>.GetPropertyName(p => p.EmailAddress, true)));

            List<User> querResult = FiglutDesktopManagerApp.Instance.RestClient.GetEntitiesByField<User>(
                EntityReader<User>.GetPropertyName(p => p.EmailAddress, false),
                txtEmailAddress.Text);

            if (querResult.Count > 0)
            {
                txtEmailAddress.Focus();
                throw new UserThrownException(string.Format("There is an existing {0} with the {1} of '{2}'.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.EmailAddress, true),
                    txtEmailAddress.Text),
                    LoggingLevel.None);
            }
        }

        public User GetExisting(Guid id, WaitProcess w)
        {
            w.ChangeStatus(string.Format("Getting existing {0} ...", typeof(User).Name));
            User result = FiglutDesktopManagerApp.Instance.RestClient.GetEntityById<User>(id);
            if (result == null)
            {
                throw new NullReferenceException(string.Format("Could not find existing {0} with {1} of '{2}'. Refresh your local data.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.UserId, false),
                    id));
            }
            return result;
        }

        private void UpdateEntity()
        {
            using (WaitProcess w = new WaitProcess(mnuMain, this))
            {
                if (txtEmailAddress.Text != _entity.EmailAddress) //Has been changed.
                {
                    ValidateEmailAddress(w);
                }
                User e = GetExisting(_entity.UserId, w);
                e.EmailAddress = txtEmailAddress.Text;
                w.ChangeStatus(string.Format("Saving {0} ...", typeof(User).Name));
                FiglutDesktopManagerApp.Instance.RestClient.PutEntity<User>(e);
                _entity = e;
            }
        }

        #endregion //Methods

        #region Event Handlers

        private void ManageUserForm_MouseDown(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseDown(sender, e);
        }

        private void ManageUserForm_MouseMove(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseMove(sender, e);
        }

        private void ManageUserForm_MouseUp(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseUp(sender, e);
        }

        private void mnuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void ManageUserForm_Load(object sender, EventArgs e)
        {
            RefreshUI();
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            VerifyInputControls();
            UpdateEntity();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void ManageUserForm_KeyUp(object sender, KeyEventArgs e)
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
