namespace Figlut.Repeat.Desktop.UI.Base
{
    #region Using Directives

    using Figlut.Server.Toolkit.Winforms;
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

    public partial class FiglutBaseForm : BorderlessForm
    {
        #region Constructors

        public FiglutBaseForm()
        {
            InitializeComponent();
        }

        #endregion //Constructors

        #region Methods

        protected void SetBarHeights(int height)
        {
            lblFormTitle.Height = statusMain.Height = height;
        }

        #endregion //Methods
    }
}
