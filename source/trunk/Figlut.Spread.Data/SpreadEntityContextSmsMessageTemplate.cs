namespace Figlut.Spread.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class SpreadEntityContext : EntityContext
    {
        #region Sms Message Template

        public SmsMessageTemplate GetSmsMessageTemplate(Guid smsMessageTemplateId, bool throwExceptionOnNotFound)
        {
            SmsMessageTemplate result = (from s in DB.GetTable<SmsMessageTemplate>()
                                         where s.SmsMessageTemplateId == smsMessageTemplateId
                                         select s).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(SmsMessageTemplate).Name,
                    EntityReader<SmsMessageTemplate>.GetPropertyName(p => p.SmsMessageTemplateId, false),
                    smsMessageTemplateId.ToString()));
            }
            return result;
        }

        public long GetAllSmsMessageTemplateCount()
        {
            return DB.GetTable<SmsMessageTemplate>().LongCount();
        }

        public SmsMessageTemplateView GetSmsMessageTemplateView(Guid smsMessageTemplateId, bool throwExceptionOnNotFound)
        {
            SmsMessageTemplateView result = (from s in DB.GetTable<SmsMessageTemplate>()
                                             join o in DB.GetTable<Organization>() on s.OrganizationId equals o.OrganizationId into set
                                             from sub in set.DefaultIfEmpty()
                                             where s.SmsMessageTemplateId == smsMessageTemplateId
                                             orderby s.Message
                                             select new SmsMessageTemplateView()
                                             {
                                                 SmsMessageTemplateId = s.SmsMessageTemplateId,
                                                 OrganizationId = s.OrganizationId,
                                                 Message = s.Message,
                                                 DateCreated = s.DateCreated,
                                                 OrganizationName = sub.Name
                                             }).FirstOrDefault();
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(SmsMessageTemplateView).Name,
                    EntityReader<SmsMessageTemplateView>.GetPropertyName(p => p.SmsMessageTemplateId, false),
                    smsMessageTemplateId.ToString()));
            }
            return result;
        }

        public List<SmsMessageTemplateView> GetSmsMessageTemplateViewsByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<SmsMessageTemplateView> result = null;
            if (organizationId.HasValue)
            {
                result = (from s in DB.GetTable<SmsMessageTemplate>()
                          join o in DB.GetTable<Organization>() on s.OrganizationId equals o.OrganizationId into set
                          from sub in set.DefaultIfEmpty()
                          where s.OrganizationId == organizationId.Value &&
                          (s.Message.ToLower().Contains(searchFilterLower))
                          orderby s.Message
                          select new SmsMessageTemplateView()
                          {
                              SmsMessageTemplateId = s.SmsMessageTemplateId,
                              OrganizationId = s.OrganizationId,
                              Message = s.Message,
                              DateCreated = s.DateCreated,
                              OrganizationName = sub.Name
                          }).ToList();
            }
            else
            {
                result = (from s in DB.GetTable<SmsMessageTemplate>()
                          join o in DB.GetTable<Organization>() on s.OrganizationId equals o.OrganizationId into set
                          from sub in set.DefaultIfEmpty()
                          where (s.Message.ToLower().Contains(searchFilterLower))
                          orderby s.Message
                          select new SmsMessageTemplateView()
                          {
                              SmsMessageTemplateId = s.SmsMessageTemplateId,
                              OrganizationId = s.OrganizationId,
                              Message = s.Message,
                              DateCreated = s.DateCreated,
                              OrganizationName = sub.Name
                          }).ToList();
            }
            return result;
        }

        public List<SmsMessageTemplate> GetSmsMessageTemplatesByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<SmsMessageTemplate> result = null;
            if (organizationId.HasValue)
            {
                result = (from s in DB.GetTable<SmsMessageTemplate>()
                          where s.OrganizationId == organizationId.Value &&
                          s.Message.ToLower().Contains(searchFilterLower)
                          orderby s.Message
                          select s).ToList();
            }
            else
            {
                result = (from s in DB.GetTable<SmsMessageTemplate>()
                          where s.Message.ToLower().Contains(searchFilterLower)
                          orderby s.Message
                          select s).ToList();
            }
            return result;
        }

        public void DeleteSmsMessageTemplatesByFilter(string searchFilter, Nullable<Guid> organizationId)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<SmsMessageTemplate> smsMessageTemplates = GetSmsMessageTemplatesByFilter(searchFilter, organizationId);
                DB.GetTable<SmsMessageTemplate>().DeleteAllOnSubmit(smsMessageTemplates);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        #endregion //Sms Message Template
    }
}