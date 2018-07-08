namespace Figlut.Repeat.Data
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Data.DB.LINQ;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;

    #endregion //Using Directives

    public partial class RepeatEntityContext : EntityContext
    {
        #region GlobalSetting

        public List<GlobalSetting> GetAllGlobalSettings()
        {
            return (from g in DB.GetTable<GlobalSetting>()
                    orderby g.Category, g.SettingName
                    select g).ToList();
        }

        public List<GlobalSetting> GetGlobalSettingsByFilter(string searchFilter)
        {
            string searchFilterLower = searchFilter == null ? string.Empty : searchFilter.ToLower();
            List<GlobalSetting> result = (from g in DB.GetTable<GlobalSetting>()
                                          where g.SettingName.ToLower().Contains(searchFilterLower) ||
                                          g.Category.ToLower().Contains(searchFilterLower) ||
                                          g.SettingValue.ToLower().Contains(searchFilterLower) ||
                                          (g.Description != null && g.Description.ToLower().Contains(searchFilterLower))
                                          orderby g.Category, g.SettingName
                                          select g).ToList();
            return result;
        }

        public void DeleteGlobalSettingByFilter(string searchFilter)
        {
            using (TransactionScope t = new TransactionScope())
            {
                List<GlobalSetting> globalSettings = GetGlobalSettingsByFilter(searchFilter);
                DB.GetTable<GlobalSetting>().DeleteAllOnSubmit(globalSettings);
                DB.SubmitChanges();
                t.Complete();
            }
        }

        public long GetAllGlobalSettingCount()
        {
            return DB.GetTable<GlobalSetting>().LongCount();
        }

        public GlobalSetting GetGlobalSetting(Guid globalSettingId, bool throwExceptionOnNotFound)
        {
            List<GlobalSetting> q = (from g in DB.GetTable<GlobalSetting>()
                                     where g.GlobalSettingId == globalSettingId
                                     select g).ToList();
            GlobalSetting result = q.Count < 1 ? null : q[0];
            if (throwExceptionOnNotFound && result == null)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(GlobalSetting).Name,
                    EntityReader<GlobalSetting>.GetPropertyName(p => p.GlobalSettingId, false),
                    globalSettingId.ToString()));
            }
            return result;
        }

        public GlobalSetting GetGlobalSettingBySettingName(GlobalSettingName settingName, bool throwExceptionOnNotFound)
        {
            return GetGlobalSettingBySettingName(settingName.ToString(), throwExceptionOnNotFound);
        }

        public GlobalSetting GetGlobalSettingBySettingName(string settingName, bool throwExceptionOnNotFound)
        {
            List<GlobalSetting> q = (from s in DB.GetTable<GlobalSetting>()
                                     where s.SettingName == settingName
                                     select s).ToList();
            GlobalSetting result = q.Count < 1 ? null : q[0];
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find {0} with {1} of '{2}'.",
                    typeof(GlobalSetting).Name,
                    EntityReader<GlobalSetting>.GetPropertyName(p => p.SettingName, false),
                    settingName.ToString()));
            }
            return result;
        }

        public void GetGlobalSettingSmsDaysDateRange(out DateTime startDate, out DateTime endDate)
        {
            GlobalSetting setting = GetGlobalSettingBySettingName(GlobalSettingName.SmsDaysToDisplay, true);
            int smsDaysToDisplay = Convert.ToInt32(setting.SettingValue);
            endDate = DateTime.Now;
            startDate = endDate.Subtract(new TimeSpan(smsDaysToDisplay - 1, 0, 0, 0)); //e.g. if the smsDaysToDisplay is 1, then subtract 0, which will include only today's SMS'.
        }

        public void GetGlobalSettingSmsProcessorLogDaysDateRange(out DateTime startDate, out DateTime endDate)
        {
            GlobalSetting setting = GetGlobalSettingBySettingName(GlobalSettingName.SmsProcessorLogDaysToDisplay, true);
            int daysToDisplay = Convert.ToInt32(setting.SettingValue);
            endDate = DateTime.Now;
            startDate = endDate.Subtract(new TimeSpan(daysToDisplay - 1, 0, 0, 0)); //e.g. if the daysToDisplay is 1, then subtract 0, which will include only today's logs'.
        }

        public void GetGlobalSettingWebRequestActivityDaysDataRange(out DateTime startDate, out DateTime endDate)
        {
            GlobalSetting setting = GetGlobalSettingBySettingName(GlobalSettingName.WebRequestActivityDaysToDisplay, true);
            int daysToDisplay = Convert.ToInt32(setting.SettingValue);
            endDate = DateTime.Now;
            startDate = endDate.Subtract(new TimeSpan(daysToDisplay - 1, 0, 0, 0)); //e.g. if the daysToDisplay is 1, then subtract 0, which will include only today's date.
        }

        public void SaveGlobalSettings(List<GlobalSetting> globalSettings)
        {
            using (TransactionScope t = new TransactionScope())
            {
                foreach (GlobalSetting g in globalSettings)
                {
                    GlobalSetting original = GetGlobalSettingBySettingName(g.SettingName, false);
                    if (original == null)
                    {
                        DB.GetTable<GlobalSetting>().InsertOnSubmit(g);
                    }
                    else
                    {
                        EntityReader<GlobalSetting>.CopyProperties(g, original, true);
                    }
                    DB.SubmitChanges();
                }
                t.Complete();
            }
        }

        #endregion //GlobalSetting
    }
}
