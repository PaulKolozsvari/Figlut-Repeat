namespace Figlut.Repeat.Web.Service
{
    partial class ProjectInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FiglutRepeatServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.FiglutRepeatServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // FiglutRepeatServiceProcessInstaller
            // 
            this.FiglutRepeatServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.FiglutRepeatServiceProcessInstaller.Password = null;
            this.FiglutRepeatServiceProcessInstaller.Username = null;
            // 
            // FiglutRepeatServiceInstaller
            // 
            this.FiglutRepeatServiceInstaller.Description = "Web Service for authenticated users to interact with the Figlut system.";
            this.FiglutRepeatServiceInstaller.DisplayName = "Figlut Repeat Web Service";
            this.FiglutRepeatServiceInstaller.ServiceName = "Figlut.RepeatService";
            this.FiglutRepeatServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FiglutRepeatServiceProcessInstaller,
            this.FiglutRepeatServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller FiglutRepeatServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller FiglutRepeatServiceInstaller;
    }
}