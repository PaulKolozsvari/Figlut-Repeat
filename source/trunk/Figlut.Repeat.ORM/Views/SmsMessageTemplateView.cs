namespace Figlut.Repeat.ORM.Views
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SmsMessageTemplateView
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
    }
}
