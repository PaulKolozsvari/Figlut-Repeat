namespace Figlut.Repeat.ORM.Csv
{
    using Figlut.Repeat.ORM.Views;
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SmsMessageTemplateCsv
    {
        #region Sms Message Template Properties

        public Guid SmsMessageTemplateId { get; set; }

        public Guid OrganizationId { get; set; }

        public string Message { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Sms Message Template Properties

        #region Organization Properties

        public string OrganizationName { get; set; }

        #endregion //Organization Properties

        #region Methods

        public void CopyPropertiesFromSmsMessageTemplateView(SmsMessageTemplateView view)
        {
            this.SmsMessageTemplateId = view.SmsMessageTemplateId;
            this.OrganizationId = view.OrganizationId;
            this.Message = view.Message;
            this.DateCreated = view.DateCreated;

            this.OrganizationName = view.OrganizationName;
        }

        public void CopyPropertiesToSmsMessageTemplateView(SmsMessageTemplateView view)
        {
            view.SmsMessageTemplateId = this.SmsMessageTemplateId;
            view.OrganizationId = this.OrganizationId;
            view.Message = this.Message;
            view.DateCreated = this.DateCreated;

            view.OrganizationName = this.OrganizationName;
        }

        #endregion //Methods
    }
}