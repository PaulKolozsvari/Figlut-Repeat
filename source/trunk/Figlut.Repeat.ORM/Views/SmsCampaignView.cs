namespace Figlut.Repeat.ORM.Views
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SmsCampaignView
    {
        #region Properties

        #region Sms Campaign Properties

        public Guid SmsCampaignId { get; set; }

        public string Name { get; set; }

        public string MessageContents { get; set; }

        public string OrganizationSelectedCode { get; set; }

        public Guid OrganizationId { get; set; }

        public DateTime DateCreated { get; set; }

        public long SmsSentQueueItemCount { get; set; }

        public long SmsSentCount { get; set; }

        #endregion //Sms Campaign Properties

        #region Organization Properties

        public string OrganizationName { get; set; }

        #endregion //Organization Properties

        #endregion //Properties
    }
}