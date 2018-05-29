namespace Figlut.Spread.Web.Service
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
            this.FiglutSpreadServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.FiglutSpreadServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // FiglutSpreadServiceProcessInstaller
            // 
            this.FiglutSpreadServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.FiglutSpreadServiceProcessInstaller.Password = null;
            this.FiglutSpreadServiceProcessInstaller.Username = null;
            // 
            // FiglutSpreadServiceInstaller
            // 
            this.FiglutSpreadServiceInstaller.Description = "Web Service for authenticated users to interact with the Figlut system.";
            this.FiglutSpreadServiceInstaller.DisplayName = "Figlut Spread Web Service";
            this.FiglutSpreadServiceInstaller.ServiceName = "Figlut.SpreadService";
            this.FiglutSpreadServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FiglutSpreadServiceProcessInstaller,
            this.FiglutSpreadServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller FiglutSpreadServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller FiglutSpreadServiceInstaller;
    }
}