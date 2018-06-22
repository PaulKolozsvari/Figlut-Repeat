namespace Figlut.Spread.Desktop.UI
{
    partial class ManageUserForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageUserForm));
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlUserName = new System.Windows.Forms.Panel();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.pnlEmailAddress = new System.Windows.Forms.Panel();
            this.txtEmailAddress = new System.Windows.Forms.TextBox();
            this.lblEmailAddress = new System.Windows.Forms.Label();
            this.pnlNotes = new System.Windows.Forms.Panel();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.pnlPassword = new System.Windows.Forms.Panel();
            this.pnlBackground.SuspendLayout();
            this.pnlFormContent.SuspendLayout();
            this.mnuMain.SuspendLayout();
            this.pnlUserName.SuspendLayout();
            this.pnlEmailAddress.SuspendLayout();
            this.pnlNotes.SuspendLayout();
            this.pnlPassword.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBackground
            // 
            this.pnlBackground.Controls.Add(this.pnlNotes);
            this.pnlBackground.Controls.Add(this.pnlEmailAddress);
            this.pnlBackground.Controls.Add(this.pnlPassword);
            this.pnlBackground.Controls.Add(this.pnlUserName);
            this.pnlBackground.Controls.Add(this.mnuMain);
            this.pnlBackground.Margin = new System.Windows.Forms.Padding(4);
            this.pnlBackground.Size = new System.Drawing.Size(888, 228);
            // 
            // pnlFormContent
            // 
            this.pnlFormContent.Margin = new System.Windows.Forms.Padding(2);
            this.pnlFormContent.Size = new System.Drawing.Size(888, 228);
            // 
            // pnlFormRight
            // 
            this.pnlFormRight.Margin = new System.Windows.Forms.Padding(2);
            this.pnlFormRight.Size = new System.Drawing.Size(30, 228);
            // 
            // pnlFormLeft
            // 
            this.pnlFormLeft.Margin = new System.Windows.Forms.Padding(2);
            this.pnlFormLeft.Size = new System.Drawing.Size(30, 228);
            // 
            // lblFormTitle
            // 
            this.lblFormTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ManageUserForm_MouseDown);
            this.lblFormTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ManageUserForm_MouseMove);
            this.lblFormTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ManageUserForm_MouseUp);
            // 
            // statusMain
            // 
            this.statusMain.Location = new System.Drawing.Point(0, 268);
            this.statusMain.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            // 
            // mnuMain
            // 
            this.mnuMain.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSave,
            this.mnuCancel});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(888, 40);
            this.mnuMain.TabIndex = 1;
            this.mnuMain.Text = "menuStrip1";
            // 
            // mnuSave
            // 
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.Size = new System.Drawing.Size(77, 36);
            this.mnuSave.Text = "Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // mnuCancel
            // 
            this.mnuCancel.Name = "mnuCancel";
            this.mnuCancel.Size = new System.Drawing.Size(98, 36);
            this.mnuCancel.Text = "Cancel";
            this.mnuCancel.Click += new System.EventHandler(this.mnuCancel_Click);
            // 
            // pnlUserName
            // 
            this.pnlUserName.BackColor = System.Drawing.Color.Transparent;
            this.pnlUserName.Controls.Add(this.txtUserName);
            this.pnlUserName.Controls.Add(this.lblUserName);
            this.pnlUserName.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlUserName.Location = new System.Drawing.Point(0, 40);
            this.pnlUserName.Margin = new System.Windows.Forms.Padding(4);
            this.pnlUserName.Name = "pnlUserName";
            this.pnlUserName.Size = new System.Drawing.Size(888, 42);
            this.pnlUserName.TabIndex = 2;
            // 
            // txtUserName
            // 
            this.txtUserName.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtUserName.Location = new System.Drawing.Point(264, 0);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(4);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(624, 31);
            this.txtUserName.TabIndex = 1;
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUserName.Location = new System.Drawing.Point(0, 0);
            this.lblUserName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(139, 25);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "User Name: *";
            // 
            // pnlEmailAddress
            // 
            this.pnlEmailAddress.BackColor = System.Drawing.Color.Transparent;
            this.pnlEmailAddress.Controls.Add(this.txtEmailAddress);
            this.pnlEmailAddress.Controls.Add(this.lblEmailAddress);
            this.pnlEmailAddress.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEmailAddress.Location = new System.Drawing.Point(0, 124);
            this.pnlEmailAddress.Margin = new System.Windows.Forms.Padding(4);
            this.pnlEmailAddress.Name = "pnlEmailAddress";
            this.pnlEmailAddress.Size = new System.Drawing.Size(888, 42);
            this.pnlEmailAddress.TabIndex = 4;
            // 
            // txtEmailAddress
            // 
            this.txtEmailAddress.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtEmailAddress.Location = new System.Drawing.Point(264, 0);
            this.txtEmailAddress.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmailAddress.Name = "txtEmailAddress";
            this.txtEmailAddress.Size = new System.Drawing.Size(624, 31);
            this.txtEmailAddress.TabIndex = 1;
            // 
            // lblEmailAddress
            // 
            this.lblEmailAddress.AutoSize = true;
            this.lblEmailAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEmailAddress.Location = new System.Drawing.Point(0, 0);
            this.lblEmailAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmailAddress.Name = "lblEmailAddress";
            this.lblEmailAddress.Size = new System.Drawing.Size(170, 25);
            this.lblEmailAddress.TabIndex = 0;
            this.lblEmailAddress.Text = "Email Address: *";
            // 
            // pnlNotes
            // 
            this.pnlNotes.BackColor = System.Drawing.Color.Transparent;
            this.pnlNotes.Controls.Add(this.txtNotes);
            this.pnlNotes.Controls.Add(this.lblNotes);
            this.pnlNotes.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlNotes.Location = new System.Drawing.Point(0, 166);
            this.pnlNotes.Margin = new System.Windows.Forms.Padding(4);
            this.pnlNotes.Name = "pnlNotes";
            this.pnlNotes.Size = new System.Drawing.Size(888, 42);
            this.pnlNotes.TabIndex = 5;
            // 
            // txtNotes
            // 
            this.txtNotes.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtNotes.Location = new System.Drawing.Point(264, 0);
            this.txtNotes.Margin = new System.Windows.Forms.Padding(4);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(624, 31);
            this.txtNotes.TabIndex = 1;
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNotes.Location = new System.Drawing.Point(0, 0);
            this.lblNotes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(74, 25);
            this.lblNotes.TabIndex = 0;
            this.lblNotes.Text = "Notes:";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPassword.Location = new System.Drawing.Point(0, 0);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(126, 25);
            this.lblPassword.TabIndex = 0;
            this.lblPassword.Text = "Password: *";
            // 
            // txtPassword
            // 
            this.txtPassword.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtPassword.Location = new System.Drawing.Point(264, 0);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(624, 31);
            this.txtPassword.TabIndex = 1;
            // 
            // pnlPassword
            // 
            this.pnlPassword.BackColor = System.Drawing.Color.Transparent;
            this.pnlPassword.Controls.Add(this.txtPassword);
            this.pnlPassword.Controls.Add(this.lblPassword);
            this.pnlPassword.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPassword.Location = new System.Drawing.Point(0, 82);
            this.pnlPassword.Margin = new System.Windows.Forms.Padding(4);
            this.pnlPassword.Name = "pnlPassword";
            this.pnlPassword.Size = new System.Drawing.Size(888, 42);
            this.pnlPassword.TabIndex = 3;
            // 
            // ManageUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 308);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ManageUserForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage User";
            this.Load += new System.EventHandler(this.ManageUserForm_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ManageUserForm_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ManageUserForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ManageUserForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ManageUserForm_MouseUp);
            this.pnlBackground.ResumeLayout(false);
            this.pnlBackground.PerformLayout();
            this.pnlFormContent.ResumeLayout(false);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.pnlUserName.ResumeLayout(false);
            this.pnlUserName.PerformLayout();
            this.pnlEmailAddress.ResumeLayout(false);
            this.pnlEmailAddress.PerformLayout();
            this.pnlNotes.ResumeLayout(false);
            this.pnlNotes.PerformLayout();
            this.pnlPassword.ResumeLayout(false);
            this.pnlPassword.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.ToolStripMenuItem mnuCancel;
        private System.Windows.Forms.Panel pnlUserName;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Panel pnlEmailAddress;
        private System.Windows.Forms.TextBox txtEmailAddress;
        private System.Windows.Forms.Label lblEmailAddress;
        private System.Windows.Forms.Panel pnlNotes;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.Panel pnlPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
    }
}