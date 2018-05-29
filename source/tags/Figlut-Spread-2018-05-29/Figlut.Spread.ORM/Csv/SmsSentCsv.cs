namespace Figlut.Spread.ORM.Csv
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SmsSentCsv
    {
        #region Properties

        public Guid SmsSentLogId { get; set; }

        public string SenderUserName { get; set; }

        public bool Success { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string CellPhoneNumber { get; set; }

        public string MessageId { get; set; }

        public string MessageContents { get; set; }

        //public string TableReference { get; set; }

        //public Nullable<Guid> RecordReference { get; set; }

        //public string Tag { get; set; }

        //public int SmsProviderCode { get; set; }

        //public Nullable<Guid> SenderUserId { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromSmsSentLog(SmsSentLog smsSentLog, string senderUserName)
        {
            this.SmsSentLogId = smsSentLog.SmsSentLogId;
            this.SenderUserName = senderUserName;
            this.Success = smsSentLog.Success;
            this.ErrorCode = smsSentLog.ErrorCode;
            this.ErrorMessage = smsSentLog.ErrorMessage;
            this.CellPhoneNumber = smsSentLog.CellPhoneNumber;
            this.MessageId = smsSentLog.MessageId;
            this.MessageContents = smsSentLog.MessageContents;
            //this.TableReference = smsSentLog.TableReference;
            //this.RecordReference = smsSentLog.RecordReference;
            //this.Tag = smsSentLog.Tag;
            //this.SmsProviderCode = smsSentLog.SmsProviderCode;
            //this.SenderUserId = smsSentLog.SenderUserId;
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
            //smsSentLog.MessageContents = this.MessageContents;
            //smsSentLog.TableReference = this.TableReference;
            //smsSentLog.RecordReference = this.RecordReference;
            //smsSentLog.Tag = this.Tag;
            //smsSentLog.SmsProviderCode = this.SmsProviderCode;
            //smsSentLog.SenderUserId = this.SenderUserId;
            smsSentLog.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}