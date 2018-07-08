namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class SmsCampaignModel
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

        public bool IsValid(out string errorMessage, int maxSmsSendMessageLength)
        {
            errorMessage = null;
            if (this.SmsCampaignId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SmsCampaignModel>.GetPropertyName(p => p.SmsCampaignId, true));
            }
            else if (string.IsNullOrEmpty(this.Name))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SmsCampaignModel>.GetPropertyName(p => p.Name, true));
            }
            else if (string.IsNullOrEmpty(this.MessageContents))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SmsCampaignModel>.GetPropertyName(p => p.MessageContents, true));
            }
            else if (this.MessageContents.Length > maxSmsSendMessageLength)
            {
                errorMessage = string.Format("{0} may not be greater than {1} characters.", EntityReader<ComposeSmsStandaloneModel>.GetPropertyName(p => p.MessageContents, true), maxSmsSendMessageLength);
            }
            else if (this.DateCreated == new DateTime())
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SmsCampaignModel>.GetPropertyName(p => p.DateCreated, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

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

        public void CopyPropertiesToSmsCampaign(SmsCampaign smsCampaign)
        {
            smsCampaign.SmsCampaignId = this.SmsCampaignId;
            smsCampaign.Name = this.Name;
            smsCampaign.OrganizationSelectedCode = this.OrganizationSelectedCode;
            smsCampaign.OrganizationId = this.OrganizationId;
            smsCampaign.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}