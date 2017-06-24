namespace Figlut.Spread.ORM.Csv
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class GlobalSettingCsv
    {
        #region Properties

        public Guid GlobalSettingId { get; set; }

        public string Category { get; set; }

        public string SettingName { get; set; }

        public string SettingValue { get; set; }

        public string Description { get; set; }

        public Nullable<DateTime> LastDateUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        #endregion //Properties

        #region Methods

        public void CopyPropertiesFromGlobalSetting(GlobalSetting globalSetting)
        {
            this.GlobalSettingId = globalSetting.GlobalSettingId;
            this.Category = globalSetting.Category;
            this.SettingName = globalSetting.SettingName;
            this.SettingValue = globalSetting.SettingValue;
            this.Description = globalSetting.Description;
            this.LastDateUpdated = globalSetting.LastDateUpdated;
            this.DateCreated = globalSetting.DateCreated;
        }

        public void CopyPropertiesTo(GlobalSetting globalSetting)
        {
            globalSetting.GlobalSettingId = this.GlobalSettingId;
            globalSetting.Category = this.Category;
            globalSetting.SettingName = this.SettingName;
            globalSetting.SettingValue = this.SettingValue;
            globalSetting.Description = this.Description;
            globalSetting.LastDateUpdated = this.LastDateUpdated;
            globalSetting.DateCreated = this.DateCreated;
        }

        #endregion //Methods
    }
}
