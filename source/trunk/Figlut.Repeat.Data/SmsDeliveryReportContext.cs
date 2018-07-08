namespace Figlut.Repeat.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Repeat.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class RepeatEntityContext : EntityContext
    {
        #region SMS Delivery Report Log

        public SmsDeliveryReportLog LogSmsDeliveryReport(
            string cellPhoneNumber,
            string messageId,
            string messageContents,
            string dateReceivedOriginalFormat,
            string campaign,
            string dataField,
            string nonce,
            string nonceDateOriginalFormat,
            string checksum,
            int smsProviderCode)
        {
            SmsDeliveryReportLog result = null;
            using (TransactionScope t = new TransactionScope())
            {
                result = new SmsDeliveryReportLog()
                {
                    SmsDeliveryReportLogId = Guid.NewGuid(),
                    CellPhoneNumber = cellPhoneNumber,
                    MessageId = messageId,
                    MessageContents = messageContents,
                    DateReceivedOriginalFormat = dateReceivedOriginalFormat,
                    Campaign = campaign,
                    DataField = dataField,
                    Nonce = nonce,
                    NonceDateOriginalFormat = nonceDateOriginalFormat,
                    Checksum = checksum,
                    SmsProviderCode = smsProviderCode,
                    DateCreated = DateTime.Now
                };
                DB.GetTable<SmsDeliveryReportLog>().InsertOnSubmit(result);
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        #endregion //SMS Delivery Report Log
    }
}