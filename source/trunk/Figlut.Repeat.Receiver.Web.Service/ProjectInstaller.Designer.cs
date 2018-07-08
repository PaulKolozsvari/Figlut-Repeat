namespace Figlut.Repeat.Receiver.Web.Service
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
            this.FiglutRepeatReceiverServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.FiglutRepeatReceiverServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // FiglutRepeatReceiverServiceProcessInstaller
            // 
            this.FiglutRepeatReceiverServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.FiglutRepeatReceiverServiceProcessInstaller.Password = null;
            this.FiglutRepeatReceiverServiceProcessInstaller.Username = null;
            // 
            // FiglutRepeatReceiverServiceInstaller
            // 
            this.FiglutRepeatReceiverServiceInstaller.Description = "Web Service for receiving SMS\' from service providers. Does not require authentic" +
    "ation by clients.";
            this.FiglutRepeatReceiverServiceInstaller.DisplayName = "Figlut Repeat Receiver Web Service";
            this.FiglutRepeatReceiverServiceInstaller.ServiceName = "Figlut.RepeatReceiverService";
            this.FiglutRepeatReceiverServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FiglutRepeatReceiverServiceProcessInstaller,
            this.FiglutRepeatReceiverServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller FiglutRepeatReceiverServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller FiglutRepeatReceiverServiceInstaller;
    }
}