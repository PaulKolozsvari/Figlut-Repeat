namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM;

    #endregion //Using Directives

    public class SmsSentQueueItemModel
    {
        #region Properties

        public Guid SmsSentQueueItemId { get; set; }

        public string CellPhoneNumber { get; set; }

        public string MessageContents { get; set; }

        public string TableReference { get; set; }

        public Nullable<Guid> RecordReference { get; set; }

        public string Tag { get; set; }

        public int SmsProviderCode { get; set; }

        public Nullable<Guid> SenderUserId { get; set; }

        public Nullable<Guid> OrganizationId { get; set; }

        public Nullable<Guid> SubscriberId { get; set; }

        public string SubscriberName { get; set; }

        public string Campaign { get; set; }

        public Nullable<Guid> SmsCampaignId { get; set; }

        public bool FailedToSend { get; set; }

        public string FailedToSendErrorMessage { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            string formattedPhoneNumber = null;
            if (this.SmsSentQueueItemId == Guid.Empty)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SmsSentQueueItemModel>.GetPropertyName(p => p.SmsSentQueueItemId, true));
            }
            else if (string.IsNullOrEmpty(this.CellPhoneNumber))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SmsSentQueueItemModel>.GetPropertyName(p => p.CellPhoneNumber, true));
            }
            if (!DataShaper.IsValidPhoneNumber(this.CellPhoneNumber, out formattedPhoneNumber))
            {
                errorMessage = string.Format("{0} is not a valid cell phone number.", this.CellPhoneNumber);
            }
            else if (string.IsNullOrEmpty(this.MessageContents))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<SmsSentQueueItemModel>.GetPropertyName(p => p.MessageContents, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public void CopyPropertiesFromSmsSentQueueItem(SmsSentQueueItem smsSentQueueItem)
        {
            this.SmsSentQueueItemId = smsSentQueueItem.SmsSentQueueItemId;
            this.CellPhoneNumber = smsSentQueueItem.CellPhoneNumber;
            this.MessageContents = smsSentQueueItem.MessageContents;
            this.TableReference = smsSentQueueItem.TableReference;
            this.RecordReference = smsSentQueueItem.RecordReference;
            this.Tag = smsSentQueueItem.Tag;
            this.SmsProviderCode = smsSentQueueItem.SmsProviderCode;
            this.SenderUserId = smsSentQueueItem.SenderUserId;
            this.OrganizationId = smsSentQueueItem.OrganizationId;
            this.SubscriberId = smsSentQueueItem.SubscriberId;
            this.SubscriberName = smsSentQueueItem.SubscriberName;
            this.Campaign = smsSentQueueItem.Campaign;
            this.SmsCampaignId = smsSentQueueItem.SmsCampaignId;
            this.FailedToSend = smsSentQueueItem.FailedToSend;
            this.FailedToSendErrorMessage = smsSentQueueItem.FailedToSendErrorMessage;
            this.DateCreated = smsSentQueueItem.DateCreated;
        }

        public void CopyPropertiesToSmsSentQueueItem(SmsSentQueueItem smsSentQueueItem)
        {
            smsSentQueueItem.SmsSentQueueItemId = this.SmsSentQueueItemId;
            smsSentQueueItem.CellPhoneNumber = this.CellPhoneNumber;
            smsSentQueueItem.MessageContents = this.MessageContents;
            smsSentQueueItem.TableReference = this.TableReference;
            smsSentQueueItem.RecordReference = this.RecordReference;
            smsSentQueueItem.Tag = this.Tag;
            smsSentQueueItem.SmsProviderCode = this.SmsProviderCode;
            smsSentQueueItem.SenderUserId = this.SenderUserId;
            smsSentQueueItem.OrganizationId = this.OrganizationId;
            smsSentQueueItem.SubscriberId = this.SubscriberId;
            smsSentQueueItem.SubscriberName = this.SubscriberName;
            smsSentQueueItem.Campaign = this.Campaign;
            smsSentQueueItem.SmsCampaignId = this.SmsCampaignId;
            smsSentQueueItem.FailedToSend = this.FailedToSend;
            smsSentQueueItem.FailedToSendErrorMessage = this.FailedToSendErrorMessage;
            smsSentQueueItem.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}