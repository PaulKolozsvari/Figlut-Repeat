namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Repeat.ORM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class GlobalSettingModel
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

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(this.Category))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<GlobalSettingModel>.GetPropertyName(p => p.Category, true));
                return false;
            }
            if (string.IsNullOrEmpty(this.SettingName))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<GlobalSettingModel>.GetPropertyName(p => p.SettingName, true));
                return false;
            }
            if (string.IsNullOrEmpty(this.SettingValue))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<GlobalSettingModel>.GetPropertyName(p => p.SettingValue, true));
                return false;
            }
            if (!this.LastDateUpdated.HasValue)
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<GlobalSettingModel>.GetPropertyName(p => p.LastDateUpdated, true));
                return false;
            }
            return true;
        }

        #endregion //Methods
    }
}