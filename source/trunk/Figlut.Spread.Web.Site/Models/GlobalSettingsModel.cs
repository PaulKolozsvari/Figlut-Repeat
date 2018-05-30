namespace Figlut.Spread.Web.Site.Models
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class GlobalSettingsModel
    {
        #region Properties

        public bool DisableScreenScalingForMobileDevices { get; set; }

        public bool CreatePersistentAuthenticationCookie { get; set; }

        public bool LogAllHttpHeaders { get; set; }

        public bool LogUserLastActivityDate { get; set; }

        public string DefaultCurrencySymbol { get; set; }

        public bool LogGetWebRequestActivity { get; set; }

        public bool LogPostWebRequestActivity { get; set; }

        public bool LogPutWebRequestActivity { get; set; }

        public bool LogDeleteWebRequestActivity { get; set; }

        public string WebRequestActivityUserAgentsToExclude { get; set; }

        public bool EnableWhoIsWebServiceQuery { get; set; }

        public string WhoIsWebServiceUrl { get; set; }

        public int WhoIsWebServiceRequestTimeout { get; set; }

        public bool EnableGoogleAnalytics { get; set; }

        public string OrganizationIdentifierIndicator { get; set; }

        public string SubscriberNameIndicator { get; set; }

        public int MaxSmsSendMessageLength { get; set; }

        public string SmsSendMessageSuffix { get; set; }

        public int OrganizationIdentifierMaxLength { get; set; }

        public int SmsPerPagePageToDisplay { get; set; }

        public int OrganizationsPerPageToDisplay { get; set; }

        public int SubscribersPerPageToDisplay { get; set; }

        public int UsersPerPageToDisplay { get; set; }

        public int SubscriptionsPerPageToDisplay { get; set; }

        public int SmsProcessorsPerPageToDisplay { get; set; }

        public int SmsProcessorLogsPerPageToDisplay { get; set; }

        public int WebRequestActivityPerPageToDisplay { get; set; }

        public int SmsCampaignsPerPageToDisplay { get; set; }

        public int CountriesPerPagePageToDisplay { get; set; }

        public int PublicHolidaysPerPagePageToDisplay { get; set; }

        public int SmsMessageTemplatesPerPagePageToDisplay { get; set; }

        public int SchedulesPerPageToDisplay { get; set; }

        public int ScheduleEntriesPerPageToDisplay { get; set; }

        public int GlobalSettingsPerPageToDisplay { get; set; }

        public bool SmsContentsTrimOnGrid { get; set; }

        public int SmsContentsTrimLengthOnGrid { get; set; }

        public bool CellPhoneNumberTrimOnGrid { get; set; }

        public int CellPhoneNumberTrimLengthOnGrid { get; set; }

        public int SmsErrorTrimLengthOnGrid { get; set; }

        public int SmsDaysToDisplay { get; set; }

        public int SmsProcessorLogDaysToDisplay { get; set; }

        public int WebRequestActivityDaysToDisplay { get; set; }

        public int MaximumSmsDateRangeDaysToDisplay { get; set; }

        public bool SmsProcessorMessageTrimOnGrid { get; set; }

        public int SmsProcessorMessageTrimLengthOnGrid { get; set; }

        public int DefaultRepeatDaysInterval { get; set; }

        public string FiglutPhoneNumber { get; set; }

        public string FiglutSupportEmailAddress { get; set; }

        public string FiglutMarketingEmailAddress { get; set; }

        public string FiglutGeneralEmailAddress { get; set; }

        public string FiglutAddress { get; set; }

        #endregion //Properties

        #region Methods

        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            if (this.SmsPerPagePageToDisplay <= 0)
            {
                errorMessage = string.Format("{0} may not be less than 0.", EntityReader<GlobalSettingsModel>.GetPropertyName(p => p.SmsPerPagePageToDisplay, true));
            }
            if (this.SmsContentsTrimLengthOnGrid <= 0)
            {
                errorMessage = string.Format("{0} may not be less than 0.", EntityReader<GlobalSettingsModel>.GetPropertyName(p => p.SmsContentsTrimLengthOnGrid, true));
            }
            if (this.CellPhoneNumberTrimLengthOnGrid <= 0)
            {
                errorMessage = string.Format("{0} may not be less than 0.", EntityReader<GlobalSettingsModel>.GetPropertyName(p => p.CellPhoneNumberTrimLengthOnGrid, true));
            }            
            if (this.SmsDaysToDisplay <= 0)
            {
                errorMessage = string.Format("{0} may not be less than 0.", EntityReader<GlobalSettingsModel>.GetPropertyName(p => p.SmsDaysToDisplay, true));
            }
            if (this.MaximumSmsDateRangeDaysToDisplay <= 0)
            {
                errorMessage = string.Format("{0} may not be less than 0.", EntityReader<GlobalSettingsModel>.GetPropertyName(p => p.MaximumSmsDateRangeDaysToDisplay, true));
            }
            if (string.IsNullOrEmpty(this.FiglutPhoneNumber))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<GlobalSettingsModel>.GetPropertyName(p => p.FiglutPhoneNumber, true));
            }
            else if (string.IsNullOrEmpty(this.FiglutSupportEmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<GlobalSettingsModel>.GetPropertyName(p => p.FiglutSupportEmailAddress, true));
            }
            else if (string.IsNullOrEmpty(this.FiglutMarketingEmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<GlobalSettingsModel>.GetPropertyName(p => p.FiglutMarketingEmailAddress, true));
            }
            else if (string.IsNullOrEmpty(this.FiglutGeneralEmailAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<GlobalSettingsModel>.GetPropertyName(p => p.FiglutGeneralEmailAddress, true));
            }
            else if (string.IsNullOrEmpty(this.FiglutAddress))
            {
                errorMessage = string.Format("{0} not entered.", EntityReader<GlobalSettingsModel>.GetPropertyName(p => p.FiglutAddress, true));
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        #endregion //Methods
    }
}