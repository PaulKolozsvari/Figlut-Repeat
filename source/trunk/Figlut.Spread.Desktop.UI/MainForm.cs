namespace Figlut.Spread.Desktop.UI
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Figlut.Spread.Desktop.UI.Base;
    using Figlut.Spread.Desktop.Utilities.Configuration;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;

    #endregion //Using Directives

    public partial class MainForm : FiglutBaseForm
    {
        #region Constructors

        public MainForm()
        {
            InitializeComponent();
            base.SetBarHeights(FiglutDesktopManagerApp.Instance.Settings.FormBarHeight);
        }

        #endregion //Constructors

        #region Fields

        private bool _forceClose;
        private EntityCache<Guid, User> _entityCache;
        private Dictionary<string, object> _filterProperties;
        private List<string> _hiddenProperties;

        #endregion //Fields

        #region Properties

        public bool ForceClose
        {
            get { return _forceClose; }
            set { _forceClose = value; }
        }

        #endregion //Properties

        #region Methods

        private List<string> GetHiddenProperties()
        {
            if (_hiddenProperties == null)
            {
                _hiddenProperties = new List<string>();
            }
            _hiddenProperties.Add(EntityReader<User>.GetPropertyName(p => p.UserId, true));
            _hiddenProperties.Add(EntityReader<User>.GetPropertyName(p => p.OrganizationId, true));
            _hiddenProperties.Add(EntityReader<User>.GetPropertyName(p => p.RoleId, true));
            return _hiddenProperties;
        }

        private Dictionary<string, object> GetFilterProperties(TextBox filterTextBox)
        {
            if (_filterProperties == null)
            {
                _filterProperties = new Dictionary<string, object>();
                _filterProperties.Add(EntityReader<User>.GetPropertyName(p => p.UserName, false), null);
            }
            _filterProperties[EntityReader<User>.GetPropertyName(p => p.UserName, false)] = txtSearch.Text;
            return _filterProperties;
        }

        private void RefreshData(bool fromServer)
        {
            using (WaitProcess w = new WaitProcess(mnuMain, this))
            {
                int selectedRowIndex = -1;
                if (grdData.SelectedRows.Count > 0)
                {
                    selectedRowIndex = grdData.SelectedRows[0].Index;
                }
                if (_entityCache == null)
                {
                    _entityCache = new EntityCache<Guid, User>();
                }
                if (fromServer)
                {
                    w.ChangeStatus("Downloading users ...");
                    _entityCache.RefreshFromServer(FiglutDesktopManagerApp.Instance.RestClient);
                    txtSearch.Focus();
                }
                DataTable table = _entityCache.GetDataTable(GetFilterProperties(txtSearch), false, true);
                grdData.DataSource = table;
                grdData.Refresh();
                grdData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                GetHiddenProperties().ForEach(p => grdData.Columns[p].Visible = false);
                if (selectedRowIndex < grdData.Rows.Count && selectedRowIndex > -1)
                {
                    grdData.Rows[selectedRowIndex].Selected = true;
                }
            }
        }

        private Guid GetSelectedEntityId()
        {
            Guid result = UIHelper.GetSelectedDataGridViewRowCellValue<Guid>(grdData, EntityReader<User>.GetPropertyName(p => p.UserId, true));
            if (result == Guid.Empty)
            {
                throw new UserThrownException("No item selected.", LoggingLevel.None);
            }
            return result;
        }

        private User GetSelectedEntity()
        {
            Guid id = GetSelectedEntityId();
            User result = _entityCache[id];
            if (result == null)
            {
                throw new Exception(string.Format("Could not find {0} with {1} of '{2}' in cache.",
                    typeof(User).Name,
                    EntityReader<User>.GetPropertyName(p => p.UserId, false),
                    id));
            }
            return result;
        }

        private void UpdateEntity()
        {
            User e = GetSelectedEntity();
            using (ManageUserForm f = new ManageUserForm(e))
            {
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.BeginInvoke(new Action<bool>(RefreshData), true);
                }
            }
        }

        private void ChangePassword()
        {
            User e = GetSelectedEntity();
            using (ChangePasswordForm f = new ChangePasswordForm(e))
            {
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.BeginInvoke(new Action<bool>(RefreshData), true);
                }
            }
        }

        private void DeleteEntity()
        {
            User e = GetSelectedEntity();
            if (UIHelper.AskQuestion(string.Format("Are you sure you want to delete {0} '{1}'?", DataShaper.ShapeCamelCaseString(typeof(User).Name), e.UserName)) !=
                System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }
            using (WaitProcess w = new WaitProcess(mnuMain, this))
            {
                w.ChangeStatus("Deleting ...");
                FiglutDesktopManagerApp.Instance.RestClient.DeleteById<User>(e.UserId);
            }
            _entityCache.Delete(e.UserId);
            this.BeginInvoke(new Action<bool>(RefreshData), true);
        }

        #endregion //Methods

        #region Event Handlers

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseDown(sender, e);
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseMove(sender, e);
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            base.BorderlessForm_MouseUp(sender, e);
        }

        private void picMaximize_Click(object sender, EventArgs e)
        {
            base.BorderlessForm_Maximize(sender, e);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            using (WaitProcess w = new WaitProcess(mnuMain, this))
            {
                FiglutDesktopManagerApp.Instance.Initialize(false, w);
                if (FiglutDesktopManagerApp.Instance.Settings.StartInFullScreen)
                {
                    base.BorderlessForm_Maximize(sender, e);
                }
            }
            using (LoginForm f = new LoginForm())
            {
                if (f.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.ForceClose = true;
                    this.Close();
                    return;
                }
            }
            this.BeginInvoke(new Action<bool>(RefreshData), true);
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            if (UIHelper.AskQuestion("Are you sure you want exit?") == System.Windows.Forms.DialogResult.Yes)
            {
                Close();
            }
        }

        private void tsUpdate_Click(object sender, EventArgs e)
        {
            UpdateEntity();
        }

        private void tsDelete_Click(object sender, EventArgs e)
        {
            DeleteEntity();
        }

        private void tsRefresh_Click(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action<bool>(RefreshData), true);
        }

        #endregion //Event Handlers
    }
}