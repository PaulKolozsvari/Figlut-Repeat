namespace Figlut.Spread.Receiver.Web.Service
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
            this.FiglutSpreadReceiverServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.FiglutSpreadReceiverServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // FiglutSpreadReceiverServiceProcessInstaller
            // 
            this.FiglutSpreadReceiverServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.FiglutSpreadReceiverServiceProcessInstaller.Password = null;
            this.FiglutSpreadReceiverServiceProcessInstaller.Username = null;
            // 
            // FiglutSpreadReceiverServiceInstaller
            // 
            this.FiglutSpreadReceiverServiceInstaller.Description = "Web Service for receiving SMS\' from service providers. Does not require authentic" +
    "ation by clients.";
            this.FiglutSpreadReceiverServiceInstaller.DisplayName = "Figlut Spread Receiver Web Service";
            this.FiglutSpreadReceiverServiceInstaller.ServiceName = "Figlut.SpreadReceiverService";
            this.FiglutSpreadReceiverServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FiglutSpreadReceiverServiceProcessInstaller,
            this.FiglutSpreadReceiverServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller FiglutSpreadReceiverServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller FiglutSpreadReceiverServiceInstaller;
    }
}