namespace Figlut.Repeat.Desktop.UI.Base
{
    partial class FiglutBaseForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FiglutBaseForm));
            this.pnlBackground = new Figlut.Server.Toolkit.Winforms.GradientPanel();
            this.pnlFormContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlFormContent
            // 
            this.pnlFormContent.Controls.Add(this.pnlBackground);
            this.pnlFormContent.Size = new System.Drawing.Size(888, 447);
            // 
            // pnlFormRight
            // 
            this.pnlFormRight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlFormRight.BackgroundImage")));
            this.pnlFormRight.GradientEndColor = System.Drawing.Color.AliceBlue;
            this.pnlFormRight.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.pnlFormRight.GradientStartColor = System.Drawing.Color.SteelBlue;
            this.pnlFormRight.Location = new System.Drawing.Point(918, 40);
            this.pnlFormRight.Size = new System.Drawing.Size(30, 447);
            // 
            // pnlFormLeft
            // 
            this.pnlFormLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlFormLeft.BackgroundImage")));
            this.pnlFormLeft.GradientEndColor = System.Drawing.Color.AliceBlue;
            this.pnlFormLeft.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.pnlFormLeft.GradientStartColor = System.Drawing.Color.SteelBlue;
            this.pnlFormLeft.Size = new System.Drawing.Size(30, 447);
            // 
            // lblFormTitle
            // 
            this.lblFormTitle.GradientEndColor = System.Drawing.Color.SteelBlue;
            this.lblFormTitle.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.lblFormTitle.Size = new System.Drawing.Size(948, 40);
            this.lblFormTitle.Text = " ";
            // 
            // statusMain
            // 
            this.statusMain.ForeColor = System.Drawing.Color.Black;
            this.statusMain.GradientEndColor = System.Drawing.Color.AliceBlue;
            this.statusMain.GradientStartColor = System.Drawing.Color.AliceBlue;
            this.statusMain.Location = new System.Drawing.Point(0, 487);
            this.statusMain.Size = new System.Drawing.Size(948, 40);
            // 
            // pnlBackground
            // 
            this.pnlBackground.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlBackground.BackgroundImage")));
            this.pnlBackground.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBackground.GradientEndColor = System.Drawing.Color.WhiteSmoke;
            this.pnlBackground.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.pnlBackground.GradientStartColor = System.Drawing.Color.White;
            this.pnlBackground.Location = new System.Drawing.Point(0, 0);
            this.pnlBackground.Margin = new System.Windows.Forms.Padding(6);
            this.pnlBackground.Name = "pnlBackground";
            this.pnlBackground.Size = new System.Drawing.Size(888, 447);
            this.pnlBackground.TabIndex = 10;
            // 
            // RepeatBaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 527);
            this.Name = "RepeatBaseForm";
            this.Text = "RepeatBaseForm";
            this.pnlFormContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public Figlut.Server.Toolkit.Winforms.GradientPanel pnlBackground;
    }
}