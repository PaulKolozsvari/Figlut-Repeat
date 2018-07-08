namespace Figlut.Repeat.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
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
        #region Sms Sent

        public SmsSentLog LogSmsSent(
            string cellPhoneNumber,
            string message,
            bool success,
            string errorCode,
            string errorMessage,
            string messageId,
            string messageContents,
            int smsProviderCode,
            User senderUser,
            bool beforeCreditsDeduction,
            Nullable<Guid> subscriberId,
            string subscriberName,
            string smsCampaignName,
            Nullable<Guid> smsCampaignId)
        {
            SmsSentLog result = null;
            using (TransactionScope t = new TransactionScope())
            {
                result = new SmsSentLog()
                {
                    SmsSentLogId = Guid.NewGuid(),
                    Success = success,
                    ErrorCode = errorCode,
                    ErrorMessage = errorMessage,
                    CellPhoneNumber = cellPhoneNumber,
                    MessageId = messageId,
                    MessageContents = message,
                    TableReference = null,
                    RecordReference = null,
                    Tag = null,
                    SmsProviderCode = smsProviderCode,
                    Delivered = false,
                    SubscriberId = subscriberId,
                    SubscriberName = subscriberName,
                    Campaign = smsCampaignName,
                    SmsCampaignId = smsCampaignId,
                    DateCreated = DateTime.Now
                };
                if (senderUser != null)
                {
                    result.SenderUserId = senderUser.UserId;
                    if (senderUser.OrganizationId.HasValue)
                    {
                        Organization senderOrganization = GetOrganization(senderUser.OrganizationId.Value, false);
                        if (senderOrganization != null)
                        {
                            result.OrganizationId = senderOrganization.OrganizationId;
                            string beforeCreditsDeductionMessage = beforeCreditsDeduction ? "before credits deduction" : "after credits deduction";
                            result.Tag = string.Format("{0} '{1}' {2} ({3}): {4}",
                                typeof(Organization).Name,
                                senderOrganization.Name,
                                EntityReader<Organization>.GetPropertyName(p => p.SmsCreditsBalance, true),
                                beforeCreditsDeductionMessage,
                                senderOrganization.SmsCreditsBalance);
                        }
                        else
                        {
                            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Could not find {0} with {1} of '{2}', when saving {3} with {4} of '{5}'. Associated {6}: {7}",
                                typeof(Organization).Name, //0
                                EntityReader<Organization>.GetPropertyName(p => p.OrganizationId, false), //1
                                senderUser.OrganizationId.Value, //2
                                typeof(SmsSentLog).Name, //3
                                EntityReader<SmsSentLog>.GetPropertyName(p => p.SmsSentLogId, false), //4
                                result.SmsSentLogId, //5
                                typeof(User).Name, //6
                                senderUser.UserName), //7
                                LogMessageType.Warning,
                                LoggingLevel.Normal));
                        }
                    }
                }
                DB.GetTable<SmsSentLog>().InsertOnSubmit(result);
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        public void UpdateSmsSentLog(Guid smsSentLogId, Nullable<Guid> subscriberId, string subscriberName, string smsCampaignName, Nullable<Guid> smsCamapaignId)
        {
            SmsSentLog smsSentLog = GetSmsSentLog(smsSentLogId, true);
            smsSentLog.SubscriberId = subscriberId;
            smsSentLog.SubscriberName = subscriberName;
            smsSentLog.Campaign = smsCampaignName;
            smsSentLog.SmsCampaignId = smsCamapaignId;
            DB.SubmitChanges();
        }

        public SmsSentLog LogFailedSmsSent(
            string cellPhoneNumber,
            string message,
            int smsProviderCode,
            Exception ex,
            User senderUser,
            out string errorMessage)
        {
            SmsSentLog result = null;
            using (TransactionScope t = new TransactionScope())
            {
                StringBuilder errorMessageBuilder = new StringBuilder();
                errorMessageBuilder.Append(ex.Message);
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    errorMessageBuilder.AppendFormat(" Inner Error: {0}.", ex.InnerException.Message);
                }
                errorMessage = errorMessageBuilder.ToString();
                result = new SmsSentLog()
                {
                    SmsSentLogId = Guid.NewGuid(),
                    Success = false,
                    ErrorCode = "HTTP failure",
                    ErrorMessage = errorMessageBuilder.ToString(),
                    CellPhoneNumber = cellPhoneNumber,
                    MessageId = null,
                    MessageContents = message,
                    TableReference = null,
                    RecordReference = null,
                    Tag = null,
                    SmsProviderCode = smsProviderCode,
                    DateCreated = DateTime.Now
                };
                if (senderUser != null)
                {
                    result.SenderUserId = senderUser.UserId;
                    if (senderUser.OrganizationId.HasValue)
                    {
                        Organization senderOrganization = GetOrganization(senderUser.OrganizationId.Value, false);
                        if (senderOrganization != null)
                        {
                            result.OrganizationId = senderOrganization.OrganizationId;
                        }
                        else
                        {
                            GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Could not find {0} with {1} of '{2}', when saving {3} with {4} of '{5}'. Associated {6}: {7}",
                                typeof(Organization).Name, //0
                                EntityReader<Organization>.GetPropertyName(p => p.OrganizationId, false), //1
                                senderUser.OrganizationId.Value, //2
                                typeof(SmsSentLog).Name, //3
                                EntityReader<SmsSentLog>.GetPropertyName(p => p.SmsSentLogId, false), //4
                                result.SmsSentLogId, //5
                                typeof(User).Name, //6
                                senderUser.UserName), //7
                                LogMessageType.Warning,
                                LoggingLevel.Normal));
                        }
                    }
                }
                DB.GetTable<SmsSentLog>().InsertOnSubmit(result);
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        public List<SmsSentLog> FlagSmsSentAsDelivered(string messageId, bool throwExceptionOnNotFound)
        {
            List<SmsSentLog> result = null;
            using (TransactionScope t = new TransactionScope())
            {
                result = GetSmsSentLog(messageId, throwExceptionOnNotFound);
                foreach (SmsSentLog s in result)
                {
                    s.Delivered = true;
                }
                DB.SubmitChanges();
                t.Complete();
            }
            return result;
        }

        public long GetAllSmsSentLogCount()
        {
            return DB.GetTable<SmsSentLog>().LongCount();
        }

        public List<SmsSentLog> GetSmsSentLogByFilter(
            string searchFilter, 
            DateTime startDate, 
            DateTime endDate, 
            Nullable<Guid> organizationId,
            Nullable<Guid> smsCampaignId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<SmsSentLog> result = null;
            if (organizationId.HasValue) //For specified organization only.
            {
                if (smsCampaignId.HasValue)
                {
                    result = (from s in DB.GetTable<SmsSentLog>()
                              where (s.OrganizationId.HasValue && s.OrganizationId.Value == organizationId.Value) &&
                              (s.SmsCampaignId.HasValue && s.SmsCampaignId.Value == smsCampaignId.Value) &&
                              (s.DateCreated.Date >= startDate.Date && s.DateCreated.Date <= endDate.Date) &&
                              (s.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                              s.MessageId.ToLower().Contains(searchFilterLower) ||
                              s.MessageContents.ToLower().Contains(searchFilterLower) ||
                              s.Success.ToString().ToLower().Contains(searchFilterLower) ||
                              s.ErrorCode.ToLower().Contains(searchFilterLower) ||
                              s.ErrorMessage.ToLower().Contains(searchFilterLower))
                              //s.TableReference.ToLower().Contains(searchFilterLower) ||
                              //s.RecordReference.ToString().ToLower().Contains(searchFilterLower) ||
                              //s.Tag.ToLower().Contains(searchFilterLower))
                              orderby s.DateCreated descending
                              select s).ToList();
                }
                else
                {
                    result = (from s in DB.GetTable<SmsSentLog>()
                              where (s.OrganizationId.HasValue && s.OrganizationId.Value == organizationId.Value) &&
                              (s.DateCreated.Date >= startDate.Date && s.DateCreated.Date <= endDate.Date) &&
                              (s.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                              s.MessageId.ToLower().Contains(searchFilterLower) ||
                              s.MessageContents.ToLower().Contains(searchFilterLower) ||
                              s.Success.ToString().ToLower().Contains(searchFilterLower) ||
                              s.ErrorCode.ToLower().Contains(searchFilterLower) ||
                              s.ErrorMessage.ToLower().Contains(searchFilterLower))
                              //s.TableReference.ToLower().Contains(searchFilterLower) ||
                              //s.RecordReference.ToString().ToLower().Contains(searchFilterLower) ||
                              //s.Tag.ToLower().Contains(searchFilterLower))
                              orderby s.DateCreated descending
                              select s).ToList();
                }
            }
            else //For all organizations.
            {
                if (smsCampaignId.HasValue)
                {
                    result = (from s in DB.GetTable<SmsSentLog>()
                              where (s.SmsCampaignId.HasValue && s.SmsCampaignId.Value == smsCampaignId.Value) &&
                              (s.DateCreated.Date >= startDate.Date && s.DateCreated.Date <= endDate.Date) &&
                              (s.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                              s.MessageId.ToLower().Contains(searchFilterLower) ||
                              s.MessageContents.ToLower().Contains(searchFilterLower) ||
                              s.Success.ToString().ToLower().Contains(searchFilterLower) ||
                              s.ErrorCode.ToLower().Contains(searchFilterLower) ||
                              s.ErrorMessage.ToLower().Contains(searchFilterLower))
                              //s.TableReference.ToLower().Contains(searchFilterLower) ||
                              //s.RecordReference.ToString().ToLower().Contains(searchFilterLower) ||
                              //s.Tag.ToLower().Contains(searchFilterLower))
                              orderby s.DateCreated descending
                              select s).ToList();
                }
                else
                {
                    result = (from s in DB.GetTable<SmsSentLog>()
                              where (s.DateCreated.Date >= startDate.Date && s.DateCreated.Date <= endDate.Date) &&
                              (s.CellPhoneNumber.ToLower().Contains(searchFilterLower) ||
                              s.MessageId.ToLower().Contains(searchFilterLower) ||
                              s.MessageContents.ToLower().Contains(searchFilterLower) ||
                              s.Success.ToString().ToLower().Contains(searchFilterLower) ||
                              s.ErrorCode.ToLower().Contains(searchFilterLower) ||
                              s.ErrorMessage.ToLower().Contains(searchFilterLower))
                              //s.TableReference.ToLower().Contains(searchFilterLower) ||
                              //s.RecordReference.ToString().ToLower().Contains(searchFilterLower) ||
                              //s.Tag.ToLower().Contains(searchFilterLower))
                              orderby s.DateCreated descending
                              select s).ToList();
                }
            }
            return result;
        }

        public void DeleteSmsSentLogByFilter(
            string searchFilter, 
            DateTime startDate, 
            DateTime endDate, 
            Nullable<Guid> organizationId,
            Nullable<Guid> smsCampaignId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<SmsSentLog> q = GetSmsSentLogByFilter(searchFilter, startDate, endDate, organizationId, smsCampaignId);
                DB.GetTable<SmsSentLog>().DeleteAllOnSubmit(q);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Sms Sent
    }
}