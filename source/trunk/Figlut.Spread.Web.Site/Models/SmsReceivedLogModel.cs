namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using Figlut.Spread.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class SmsReceivedLogModel
    {
        #region Properties

        public Guid SmsReceivedLogId { get; set; }

        public string CellPhoneNumber { get; set; }

        public string MessageId { get; set; }

        public string MessageContentsTrimmed { get; set; }

        public string MessageContents { get; set; }

        public string DateReceivedOriginalFormat { get; set; }

        public string Campaign { get; set; }

        public string DataField { get; set; }

        public string Nonce { get; set; }

        public string NonceDateOriginalFormat { get; set; }

        public string Checksum { get; set; }

        public int SmsProviderCode { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromSmsReceivedLog(SmsReceivedLog smsReceivedLog)
        {
            this.SmsReceivedLogId = smsReceivedLog.SmsReceivedLogId;
            this.CellPhoneNumber = smsReceivedLog.CellPhoneNumber;
            this.MessageId = smsReceivedLog.MessageId;
            this.MessageContents = smsReceivedLog.MessageContents;
            this.DateReceivedOriginalFormat = smsReceivedLog.DateReceivedOriginalFormat;
            this.Campaign = smsReceivedLog.Campaign;
            this.DataField = smsReceivedLog.DataField;
            this.Nonce = smsReceivedLog.Nonce;
            this.NonceDateOriginalFormat = smsReceivedLog.NonceDateOriginalFormat;
            this.Checksum = smsReceivedLog.Checksum;
            this.SmsProviderCode = smsReceivedLog.SmsProviderCode;
            this.DateCreated = smsReceivedLog.DateCreated;
        }

        public void CopyPropertiesToSmsReceivedLog(SmsReceivedLog smsReceivedLog)
        {
            smsReceivedLog.SmsReceivedLogId = this.SmsReceivedLogId;
            smsReceivedLog.CellPhoneNumber = this.CellPhoneNumber;
            smsReceivedLog.MessageId = this.MessageId;
            smsReceivedLog.MessageContents = this.MessageContents;
            smsReceivedLog.DateReceivedOriginalFormat = this.DateReceivedOriginalFormat;
            smsReceivedLog.Campaign = this.Campaign;
            smsReceivedLog.DataField = this.DataField;
            smsReceivedLog.Nonce = this.Nonce;
            smsReceivedLog.NonceDateOriginalFormat = this.NonceDateOriginalFormat;
            smsReceivedLog.Checksum = this.Checksum;
            smsReceivedLog.SmsProviderCode = this.SmsProviderCode;
            smsReceivedLog.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}