namespace Figlut.Repeat.ORM.Csv
{
    #region Using Directives

    using Figlut.Repeat.ORM.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SmsCampaignCsv
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

        #region Methods

        public void CopyPropertiesFromSmsCampaignView(SmsCampaignView view)
        {
            this.SmsCampaignId = view.SmsCampaignId;
            this.Name = view.Name;
            this.MessageContents = view.MessageContents;
            this.OrganizationSelectedCode = view.OrganizationSelectedCode;
            this.OrganizationId = view.OrganizationId;
            this.DateCreated = view.DateCreated;
            this.SmsSentQueueItemCount = view.SmsSentQueueItemCount;
            this.SmsSentCount = view.SmsSentCount;
            this.OrganizationName = view.OrganizationName;
        }

        public void CopyPropertiesToSmsCampaignView(SmsCampaignView view)
        {
            view.SmsCampaignId = this.SmsCampaignId;
            view.Name = this.Name;
            view.MessageContents = this.MessageContents;
            view.OrganizationSelectedCode = this.OrganizationSelectedCode;
            view.OrganizationId = this.OrganizationId;
            view.DateCreated = this.DateCreated;
            view.SmsSentQueueItemCount = this.SmsSentQueueItemCount;
            view.SmsSentCount = this.SmsSentCount;
            view.OrganizationName = this.OrganizationName;
        }

        #endregion //Methods
    }
}
