namespace Figlut.Spread.ORM.Csv
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class SmsReceivedCsv
    {
        #region Properties

        public Guid SmsReceivedLogId { get; set; }

        public string CellPhoneNumber { get; set; }

        public string MessageId { get; set; }

        public string MessageContents { get; set; }

        public string DateReceivedOriginalFormat { get; set; }

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
            this.DateCreated = smsReceivedLog.DateCreated;
        }

        public void CopyPropertiesToSmsReceivedLog(SmsReceivedLog smsReceivedLog)
        {
            smsReceivedLog.SmsReceivedLogId = this.SmsReceivedLogId;
            smsReceivedLog.CellPhoneNumber = this.CellPhoneNumber;
            smsReceivedLog.MessageId = this.MessageId;
            smsReceivedLog.MessageContents = this.MessageContents;
            smsReceivedLog.DateReceivedOriginalFormat = this.DateReceivedOriginalFormat;
            smsReceivedLog.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}
