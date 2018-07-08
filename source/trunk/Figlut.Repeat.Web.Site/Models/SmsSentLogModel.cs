namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using Figlut.Repeat.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class SmsSentLogModel
    {
        #region Properties

        public Guid SmsSentLogId { get; set; }

        public bool Success { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorMessageTrimmed { get; set; }

        public string CellPhoneNumber { get; set; }

        public string MessageId { get; set; }

        public string MessageContents { get; set; }

        public string MessageContentsTrimmed { get; set; }

        public string TableReference { get; set; }

        public Nullable<Guid> RecordReference { get; set; }

        public string Tag { get; set; }

        public int SmsProviderCode { get; set; }

        public Nullable<Guid> SenderUserId { get; set; }

        public string SenderUserName { get; set; }

        public bool Delivered { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromSmsSentLog(SmsSentLog smsSentLog)
        {
            this.SmsSentLogId = smsSentLog.SmsSentLogId;
            this.Success = smsSentLog.Success;
            this.ErrorCode = smsSentLog.ErrorCode;
            this.ErrorMessage = smsSentLog.ErrorMessage;
            this.ErrorMessageTrimmed = smsSentLog.ErrorMessage;
            this.CellPhoneNumber = smsSentLog.CellPhoneNumber;
            this.MessageId = smsSentLog.MessageId;
            this.MessageContents = smsSentLog.MessageContents;
            this.MessageContentsTrimmed = smsSentLog.MessageContents;
            this.TableReference = smsSentLog.TableReference;
            this.RecordReference = smsSentLog.RecordReference;
            this.Tag = smsSentLog.Tag;
            this.SmsProviderCode = smsSentLog.SmsProviderCode;
            this.SenderUserId = smsSentLog.SenderUserId;
            this.Delivered = smsSentLog.Delivered;
            this.DateCreated = smsSentLog.DateCreated;
        }

        public void CopyPropertiesToSmsSentLog(SmsSentLog smsSentLog)
        {
            smsSentLog.SmsSentLogId = this.SmsSentLogId;
            smsSentLog.Success = this.Success;
            smsSentLog.ErrorCode = this.ErrorCode;
            smsSentLog.ErrorMessage = this.ErrorMessage;
            smsSentLog.CellPhoneNumber = this.CellPhoneNumber;
            smsSentLog.MessageId = this.MessageId;
            smsSentLog.MessageContents = this.MessageContents;
            smsSentLog.TableReference = this.TableReference;
            smsSentLog.RecordReference = this.RecordReference;
            smsSentLog.Tag = this.Tag;
            smsSentLog.SmsProviderCode = this.SmsProviderCode;
            smsSentLog.SenderUserId = this.SenderUserId;
            smsSentLog.Delivered = this.Delivered;
            smsSentLog.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}