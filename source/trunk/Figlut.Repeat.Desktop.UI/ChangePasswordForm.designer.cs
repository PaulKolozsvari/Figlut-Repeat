namespace Figlut.Repeat.Desktop.UI
{
    partial class ChangePasswordForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePasswordForm));
            this.pnlPassword = new System.Windows.Forms.Panel();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlRenterPassword = new System.Windows.Forms.Panel();
            this.txtReenterPassword = new System.Windows.Forms.TextBox();
            this.lblReenterPassword = new System.Windows.Forms.Label();
            this.pnlBackground.SuspendLayout();
            this.pnlFormContent.SuspendLayout();
            this.pnlPassword.SuspendLayout();
            this.mnuMain.SuspendLayout();
            this.pnlRenterPassword.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBackground
            // 
            this.pnlBackground.Controls.Add(this.pnlRenterPassword);
            this.pnlBackground.Controls.Add(this.pnlPassword);
            this.pnlBackground.Controls.Add(this.mnuMain);
            this.pnlBackground.Margin = new System.Windows.Forms.Padding(4);
            this.pnlBackground.Size = new System.Drawing.Size(790, 143);
            // 
            // pnlFormContent
            // 
            this.pnlFormContent.Margin = new System.Windows.Forms.Padding(2);
            this.pnlFormContent.Size = new System.Drawing.Size(790, 143);
            // 
            // pnlFormRight
            // 
            this.pnlFormRight.Location = new System.Drawing.Point(820, 40);
            this.pnlFormRight.Margin = new System.Windows.Forms.Padding(2);
            this.pnlFormRight.Size = new System.Drawing.Size(30, 143);
            // 
            // pnlFormLeft
            // 
            this.pnlFormLeft.Margin = new System.Windows.Forms.Padding(2);
            this.pnlFormLeft.Size = new System.Drawing.Size(30, 143);
            // 
            // lblFormTitle
            // 
            this.lblFormTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFormTitle.Size = new System.Drawing.Size(850, 40);
            this.lblFormTitle.Text = "Change Password ";
            this.lblFormTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChangePasswordForm_MouseDown);
            this.lblFormTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChangePasswordForm_MouseMove);
            this.lblFormTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChangePasswordForm_MouseUp);
            // 
            // statusMain
            // 
            this.statusMain.Location = new System.Drawing.Point(0, 183);
            this.statusMain.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.statusMain.Size = new System.Drawing.Size(850, 40);
            this.statusMain.Text = "Change and save new password.";
            // 
            // pnlPassword
            // 
            this.pnlPassword.BackColor = System.Drawing.Color.Transparent;
            this.pnlPassword.Controls.Add(this.txtPassword);
            this.pnlPassword.Controls.Add(this.lblPassword);
            this.pnlPassword.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPassword.Location = new System.Drawing.Point(0, 40);
            this.pnlPassword.Margin = new System.Windows.Forms.Padding(4);
            this.pnlPassword.Name = "pnlPassword";
            this.pnlPassword.Size = new System.Drawing.Size(790, 42);
            this.pnlPassword.TabIndex = 5;
            // 
            // txtPassword
            // 
            this.txtPassword.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtPassword.Location = new System.Drawing.Point(250, 0);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(540, 31);
            this.txtPassword.TabIndex = 1;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPassword.Location = new System.Drawing.Point(0, 0);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(112, 25);
            this.lblPassword.TabIndex = 0;
            this.lblPassword.Text = "Password:";
            // 
            // mnuMain
            // 
            this.mnuMain.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSave,
            this.mnuCancel});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(790, 40);
            this.mnuMain.TabIndex = 3;
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
            // pnlRenterPassword
            // 
            this.pnlRenterPassword.BackColor = System.Drawing.Color.Transparent;
            this.pnlRenterPassword.Controls.Add(this.txtReenterPassword);
            this.pnlRenterPassword.Controls.Add(this.lblReenterPassword);
            this.pnlRenterPassword.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRenterPassword.Location = new System.Drawing.Point(0, 82);
            this.pnlRenterPassword.Margin = new System.Windows.Forms.Padding(4);
            this.pnlRenterPassword.Name = "pnlRenterPassword";
            this.pnlRenterPassword.Size = new System.Drawing.Size(790, 42);
            this.pnlRenterPassword.TabIndex = 6;
            // 
            // txtReenterPassword
            // 
            this.txtReenterPassword.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtReenterPassword.Location = new System.Drawing.Point(250, 0);
            this.txtReenterPassword.Margin = new System.Windows.Forms.Padding(4);
            this.txtReenterPassword.Name = "txtReenterPassword";
            this.txtReenterPassword.PasswordChar = '*';
            this.txtReenterPassword.Size = new System.Drawing.Size(540, 31);
            this.txtReenterPassword.TabIndex = 1;
            // 
            // lblReenterPassword
            // 
            this.lblReenterPassword.AutoSize = true;
            this.lblReenterPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReenterPassword.Location = new System.Drawing.Point(0, 0);
            this.lblReenterPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblReenterPassword.Name = "lblReenterPassword";
            this.lblReenterPassword.Size = new System.Drawing.Size(195, 25);
            this.lblReenterPassword.TabIndex = 0;
            this.lblReenterPassword.Text = "Re-enter Password";
            // 
            // ChangePasswordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 223);
            this.FormTitle = "Change Password";
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ChangePasswordForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Status = "Change and save new password.";
            this.Text = "Change Password";
            this.Load += new System.EventHandler(this.ChangePasswordForm_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ChangePasswordForm_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChangePasswordForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChangePasswordForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChangePasswordForm_MouseUp);
            this.pnlBackground.ResumeLayout(false);
            this.pnlBackground.PerformLayout();
            this.pnlFormContent.ResumeLayout(false);
            this.pnlPassword.ResumeLayout(false);
            this.pnlPassword.PerformLayout();
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.pnlRenterPassword.ResumeLayout(false);
            this.pnlRenterPassword.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.ToolStripMenuItem mnuCancel;
        private System.Windows.Forms.Panel pnlRenterPassword;
        private System.Windows.Forms.TextBox txtReenterPassword;
        private System.Windows.Forms.Label lblReenterPassword;
    }
}