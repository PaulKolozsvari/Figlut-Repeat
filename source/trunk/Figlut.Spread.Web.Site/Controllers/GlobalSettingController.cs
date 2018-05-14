namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Csv;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class GlobalSettingController : SpreadController
    {
        #region Constants

        private const string GLOBAL_SETTING_GRID_PARTIAL_VIEW_NAME = "_GlobalSettingGrid";
        private const string EDIT_GLOBAL_SETTING_DIALOG_PARTIAL_VIEW_NAME = "_EditGlobalSettingDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<GlobalSettingModel> GetGlobalSettingFilterModel(SpreadEntityContext context, FilterModel<GlobalSettingModel> model)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<GlobalSetting> globalSettingList = context.GetGlobalSettingsByFilter(model.SearchText);
            List<GlobalSettingModel> modelList = new List<GlobalSettingModel>();
            foreach (GlobalSetting g in globalSettingList)
            {
                GlobalSettingModel m = new GlobalSettingModel();
                m.CopyPropertiesFromGlobalSetting(g);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllGlobalSettingCount();
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index()
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                FilterModel<GlobalSettingModel> model = GetGlobalSettingFilterModel(context, new FilterModel<GlobalSettingModel>());
                if (model == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return View(new FilterModel<GlobalSettingModel>());
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                return View(model);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Index(FilterModel<GlobalSettingModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                FilterModel<GlobalSettingModel> resultModel = GetGlobalSettingFilterModel(context, model);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(GLOBAL_SETTING_GRID_PARTIAL_VIEW_NAME, new FilterModel<GlobalSettingModel>());
                }
                return PartialView(GLOBAL_SETTING_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> globalSettingId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                if(!globalSettingId.HasValue)
                {
                    return PartialView(EDIT_GLOBAL_SETTING_DIALOG_PARTIAL_VIEW_NAME, new GlobalSettingModel());
                }
                GlobalSetting globalSetting = context.GetGlobalSetting(globalSettingId.Value, true);
                GlobalSettingModel model = new GlobalSettingModel();
                model.CopyPropertiesFromGlobalSetting(globalSetting);
                PartialViewResult result = PartialView(EDIT_GLOBAL_SETTING_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult EditDialog(GlobalSettingModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                model.LastDateUpdated = DateTime.Now;
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                GlobalSetting originalGlobalSetting = context.GetGlobalSetting(model.GlobalSettingId, true);
                model.CopyPropertiesTo(originalGlobalSetting);
                context.DB.SubmitChanges();
                SpreadWebApp.Instance.RefreshGlobalSettingsFromDatabase();
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid globalSettingId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                GlobalSetting globalSetting = context.GetGlobalSetting(globalSettingId, true);
                context.Delete<GlobalSetting>(globalSetting);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult ConfirmDeleteDialog(Guid identifier)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                GlobalSetting globalSetting = context.GetGlobalSetting(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (globalSetting != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete Global Setting '{0}'?", globalSetting.SettingName);
                }
                PartialViewResult result = PartialView(CONFIRMATION_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ConfirmDeleteDialog(ConfirmationModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                GlobalSetting globalSetting = context.GetGlobalSetting(model.Identifier, true);
                context.Delete<GlobalSetting>(globalSetting);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult ConfirmDeleteAllDialog(string searchParametersString)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;

                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                model.SearchText = searchText;
                model.ConfirmationMessage = "Delete all Global Settings currently loaded?";
                PartialViewResult result = PartialView(CONFIRMATION_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ConfirmDeleteAllDialog(ConfirmationModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                context.DeleteGlobalSettingByFilter(model.SearchText);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult DownloadCsvFile(string searchParametersString)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<GlobalSetting> globalSettingList = context.GetGlobalSettingsByFilter(searchText);
                EntityCache<Guid, GlobalSettingCsv> cache = new EntityCache<Guid, GlobalSettingCsv>();
                foreach (GlobalSetting g in globalSettingList)
                {
                    GlobalSettingCsv csv = new GlobalSettingCsv();
                    csv.CopyPropertiesFromGlobalSetting(g);
                    cache.Add(csv.GlobalSettingId, csv);
                }
                return GetCsvFileResult<GlobalSettingCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult Edit()
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                GlobalSettingsModel model = new GlobalSettingsModel();
                model.DisableScreenScalingForMobileDevices = Convert.ToBoolean(context.GetGlobalSettingBySettingName(GlobalSettingName.DisableScreenScalingForMobileDevices, true).SettingValue);
                model.CreatePersistentAuthenticationCookie = Convert.ToBoolean(context.GetGlobalSettingBySettingName(GlobalSettingName.CreatePersistentAuthenticationCookie, true).SettingValue);
                model.LogAllHttpHeaders = Convert.ToBoolean(context.GetGlobalSettingBySettingName(GlobalSettingName.LogAllHttpHeaders, true).SettingValue);
                model.DefaultCurrencySymbol = context.GetGlobalSettingBySettingName(GlobalSettingName.DefaultCurrencySymbol, true).SettingValue;
                model.LogGetWebRequestActivity = Convert.ToBoolean(context.GetGlobalSettingBySettingName(GlobalSettingName.LogGetWebRequestActivity, true).SettingValue);
                model.LogPostWebRequestActivity = Convert.ToBoolean(context.GetGlobalSettingBySettingName(GlobalSettingName.LogPostWebRequestActivity, true).SettingValue);
                model.LogPutWebRequestActivity = Convert.ToBoolean(context.GetGlobalSettingBySettingName(GlobalSettingName.LogPutWebRequestActivity, true).SettingValue);
                model.LogDeleteWebRequestActivity = Convert.ToBoolean(context.GetGlobalSettingBySettingName(GlobalSettingName.LogDeleteWebRequestActivity, true).SettingValue);
                model.WebRequestActivityUserAgentsToExclude = context.GetGlobalSettingBySettingName(GlobalSettingName.WebRequestActivityUserAgentsToExclude, true).SettingValue;

                model.EnableWhoIsWebServiceQuery = Convert.ToBoolean(context.GetGlobalSettingBySettingName(GlobalSettingName.EnableWhoIsWebServiceQuery, true).SettingValue);
                model.WhoIsWebServiceUrl = context.GetGlobalSettingBySettingName(GlobalSettingName.WhoIsWebServiceUrl, true).SettingValue;
                model.WhoIsWebServiceRequestTimeout = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.WhoIsWebServiceRequestTimeout, true).SettingValue);

                model.OrganizationIdentifierIndicator = context.GetGlobalSettingBySettingName(GlobalSettingName.OrganizationIdentifierIndicator, true).SettingValue;
                model.SubscriberNameIndicator = context.GetGlobalSettingBySettingName(GlobalSettingName.SubscriberNameIndicator, true).SettingValue;
                model.MaxSmsSendMessageLength = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.MaxSmsSendMessageLength, true).SettingValue);
                model.SmsSendMessageSuffix = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsSendMessageSuffix, true).SettingValue;
                model.OrganizationIdentifierMaxLength = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.OrganizationIdentifierMaxLength, true).SettingValue);

                model.SmsPerPagePageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.SmsPerPagePageToDisplay, true).SettingValue);
                model.OrganizationsPerPageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.OrganizationsPerPageToDisplay, true).SettingValue);
                model.SubscribersPerPageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.SubscribersPerPageToDisplay, true).SettingValue);
                model.UsersPerPageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.UsersPerPageToDisplay, true).SettingValue);
                model.SubscriptionsPerPageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.SubscriptionsPerPageToDisplay, true).SettingValue);
                model.SmsProcessorsPerPageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.SmsProcessorsPerPageToDisplay, true).SettingValue);
                model.SmsProcessorLogsPerPageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.SmsProcessorLogsPerPageToDisplay, true).SettingValue);
                model.WebRequestActivityPerPageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.WebRequestActivityPerPageToDisplay, true).SettingValue);
                model.SmsCampaignsPerPageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.SmsCampaignsPerPageToDisplay, true).SettingValue);
                model.CountriesPerPagePageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.CountriesPerPagePageToDisplay, true).SettingValue);
                model.PublicHolidaysPerPagePageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.PublicHolidaysPerPagePageToDisplay, true).SettingValue);
                model.SmsMessageTemplatesPerPagePageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.SmsMessageTemplatesPerPagePageToDisplay, true).SettingValue);
                model.RepeatSchedulesPerPageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.RepeatSchedulesPerPageToDisplay, true).SettingValue);
                model.RepeatScheduleEntriesPerPageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.RepeatScheduleEntriesPerPageToDisplay, true).SettingValue);
                model.GlobalSettingsPerPageToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.GlobalSettingsPerPageToDisplay, true).SettingValue);

                model.SmsContentsTrimOnGrid = Convert.ToBoolean(context.GetGlobalSettingBySettingName(GlobalSettingName.SmsContentsTrimOnGrid, true).SettingValue);
                model.SmsContentsTrimLengthOnGrid = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.SmsContentsTrimLengthOnGrid, true).SettingValue);
                model.SmsErrorTrimLengthOnGrid = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.SmsErrorTrimLengthOnGrid, true).SettingValue);
                model.CellPhoneNumberTrimOnGrid = Convert.ToBoolean(context.GetGlobalSettingBySettingName(GlobalSettingName.CellPhoneNumberTrimOnGrid, true).SettingValue);
                model.CellPhoneNumberTrimLengthOnGrid = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.CellPhoneNumberTrimLengthOnGrid, true).SettingValue);
                model.SmsDaysToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.SmsDaysToDisplay, true).SettingValue);
                model.SmsProcessorLogDaysToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.SmsProcessorLogDaysToDisplay, true).SettingValue);
                model.WebRequestActivityDaysToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.WebRequestActivityDaysToDisplay, true).SettingValue);
                model.MaximumSmsDateRangeDaysToDisplay = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.MaximumSmsDateRangeDaysToDisplay, true).SettingValue);
                model.SmsProcessorMessageTrimOnGrid = Convert.ToBoolean(context.GetGlobalSettingBySettingName(GlobalSettingName.SmsProcessorMessageTrimOnGrid, true).SettingValue);
                model.SmsProcessorMessageTrimLengthOnGrid = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.SmsProcessorMessageTrimLengthOnGrid, true).SettingValue);

                model.DefaultRepeatDaysInterval = Convert.ToInt32(context.GetGlobalSettingBySettingName(GlobalSettingName.DefaultRepeatDaysInterval, true).SettingValue);

                model.FiglutPhoneNumber = context.GetGlobalSettingBySettingName(GlobalSettingName.FiglutPhoneNumber, true).SettingValue;
                model.FiglutSupportEmailAddress = context.GetGlobalSettingBySettingName(GlobalSettingName.FiglutSupportEmailAddress, true).SettingValue;
                model.FiglutMarketingEmailAddress = context.GetGlobalSettingBySettingName(GlobalSettingName.FiglutMarketingEmailAddress, true).SettingValue;
                model.FiglutGeneralEmailAddress = context.GetGlobalSettingBySettingName(GlobalSettingName.FiglutGeneralEmailAddress, true).SettingValue;
                model.FiglutAddress = context.GetGlobalSettingBySettingName(GlobalSettingName.FiglutAddress, true).SettingValue;

                return View(model);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Edit(GlobalSettingsModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();

                GlobalSetting disableScreenScalingForMobileDevices = context.GetGlobalSettingBySettingName(GlobalSettingName.DisableScreenScalingForMobileDevices, true);
                disableScreenScalingForMobileDevices.SettingValue = model.DisableScreenScalingForMobileDevices.ToString();

                GlobalSetting createPersistentAuthenticationCookie = context.GetGlobalSettingBySettingName(GlobalSettingName.CreatePersistentAuthenticationCookie, true);
                createPersistentAuthenticationCookie.SettingValue = model.CreatePersistentAuthenticationCookie.ToString();

                GlobalSetting logAllHttpHeaders = context.GetGlobalSettingBySettingName(GlobalSettingName.LogAllHttpHeaders, true);
                logAllHttpHeaders.SettingValue = model.LogAllHttpHeaders.ToString();

                GlobalSetting defaultCurrencySymbol = context.GetGlobalSettingBySettingName(GlobalSettingName.DefaultCurrencySymbol, true);
                defaultCurrencySymbol.SettingValue = model.DefaultCurrencySymbol.ToString();

                GlobalSetting logGetWebRequestActivity = context.GetGlobalSettingBySettingName(GlobalSettingName.LogGetWebRequestActivity, true);
                logGetWebRequestActivity.SettingValue = model.LogGetWebRequestActivity.ToString();

                GlobalSetting logPostWebRequestActivity = context.GetGlobalSettingBySettingName(GlobalSettingName.LogPostWebRequestActivity, true);
                logPostWebRequestActivity.SettingValue = model.LogPostWebRequestActivity.ToString();

                GlobalSetting logPutWebRequestActivity = context.GetGlobalSettingBySettingName(GlobalSettingName.LogPutWebRequestActivity, true);
                logPutWebRequestActivity.SettingValue = model.LogPutWebRequestActivity.ToString();

                GlobalSetting logDeleteWebRequestActivity = context.GetGlobalSettingBySettingName(GlobalSettingName.LogDeleteWebRequestActivity, true);
                logDeleteWebRequestActivity.SettingValue = model.LogDeleteWebRequestActivity.ToString();

                GlobalSetting webRequestActivityUserAgentsToExclude = context.GetGlobalSettingBySettingName(GlobalSettingName.WebRequestActivityUserAgentsToExclude, true);
                webRequestActivityUserAgentsToExclude.SettingValue = model.WebRequestActivityUserAgentsToExclude.ToString();

                GlobalSetting enableWhoIsWebServiceQuery = context.GetGlobalSettingBySettingName(GlobalSettingName.EnableWhoIsWebServiceQuery, true);
                enableWhoIsWebServiceQuery.SettingValue = model.EnableWhoIsWebServiceQuery.ToString();

                GlobalSetting whoIsWebServiceUrl = context.GetGlobalSettingBySettingName(GlobalSettingName.WhoIsWebServiceUrl, true);
                whoIsWebServiceUrl.SettingValue = model.WhoIsWebServiceUrl.ToString();

                GlobalSetting whoIsWebServiceRequestTimeout = context.GetGlobalSettingBySettingName(GlobalSettingName.WhoIsWebServiceRequestTimeout, true);
                whoIsWebServiceRequestTimeout.SettingValue = model.WhoIsWebServiceRequestTimeout.ToString();

                GlobalSetting organizationIdentifierIndicator = context.GetGlobalSettingBySettingName(GlobalSettingName.OrganizationIdentifierIndicator, true);
                organizationIdentifierIndicator.SettingValue = model.OrganizationIdentifierIndicator;

                GlobalSetting subscriberNameIndicator = context.GetGlobalSettingBySettingName(GlobalSettingName.SubscriberNameIndicator, true);
                subscriberNameIndicator.SettingValue = model.SubscriberNameIndicator;

                GlobalSetting maxSmsSendMessageLength = context.GetGlobalSettingBySettingName(GlobalSettingName.MaxSmsSendMessageLength, true);
                maxSmsSendMessageLength.SettingValue = model.MaxSmsSendMessageLength.ToString();

                GlobalSetting smsSendMessageSuffix = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsSendMessageSuffix, true);
                smsSendMessageSuffix.SettingValue = model.SmsSendMessageSuffix.ToString();

                GlobalSetting organizationIdentifierMaxLength = context.GetGlobalSettingBySettingName(GlobalSettingName.OrganizationIdentifierMaxLength, true);
                organizationIdentifierMaxLength.SettingValue = model.OrganizationIdentifierMaxLength.ToString();

                GlobalSetting smsPerPagePageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsPerPagePageToDisplay, true);
                smsPerPagePageToDisplay.SettingValue = model.SmsPerPagePageToDisplay.ToString();

                GlobalSetting organizationsPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.OrganizationsPerPageToDisplay, true);
                organizationsPerPageToDisplay.SettingValue = model.OrganizationsPerPageToDisplay.ToString();

                GlobalSetting subscribersPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.SubscribersPerPageToDisplay, true);
                subscribersPerPageToDisplay.SettingValue = model.SubscribersPerPageToDisplay.ToString();

                GlobalSetting usersPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.UsersPerPageToDisplay, true);
                usersPerPageToDisplay.SettingValue = model.UsersPerPageToDisplay.ToString();

                GlobalSetting subscriptionsPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.SubscriptionsPerPageToDisplay, true);
                subscriptionsPerPageToDisplay.SettingValue = model.SubscriptionsPerPageToDisplay.ToString();

                GlobalSetting smsProcessorsPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsProcessorsPerPageToDisplay, true);
                smsProcessorsPerPageToDisplay.SettingValue = model.SmsProcessorsPerPageToDisplay.ToString();

                GlobalSetting smsProcessorLogsPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsProcessorLogsPerPageToDisplay, true);
                smsProcessorLogsPerPageToDisplay.SettingValue = model.SmsProcessorLogsPerPageToDisplay.ToString();

                GlobalSetting webRequestActivityPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.WebRequestActivityPerPageToDisplay, true);
                webRequestActivityPerPageToDisplay.SettingValue = model.SmsProcessorLogsPerPageToDisplay.ToString();

                GlobalSetting smsCampaignsPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsCampaignsPerPageToDisplay, true);
                smsCampaignsPerPageToDisplay.SettingValue = model.SmsCampaignsPerPageToDisplay.ToString();

                GlobalSetting countriesPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.CountriesPerPagePageToDisplay, true);
                countriesPerPageToDisplay.SettingValue = model.CountriesPerPagePageToDisplay.ToString();

                GlobalSetting publicHolidaysPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.PublicHolidaysPerPagePageToDisplay, true);
                publicHolidaysPerPageToDisplay.SettingValue = model.PublicHolidaysPerPagePageToDisplay.ToString();

                GlobalSetting smsMessageTemplatesPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsMessageTemplatesPerPagePageToDisplay, true);
                smsMessageTemplatesPerPageToDisplay.SettingValue = model.SmsMessageTemplatesPerPagePageToDisplay.ToString();

                GlobalSetting repeatSchedulesPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.RepeatSchedulesPerPageToDisplay, true);
                repeatSchedulesPerPageToDisplay.SettingValue = model.RepeatSchedulesPerPageToDisplay.ToString();

                GlobalSetting repeatScheduleEntriesPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.RepeatScheduleEntriesPerPageToDisplay, true);
                repeatScheduleEntriesPerPageToDisplay.SettingValue = model.RepeatScheduleEntriesPerPageToDisplay.ToString();

                GlobalSetting globalSettingsPerPageToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.GlobalSettingsPerPageToDisplay, true);
                globalSettingsPerPageToDisplay.SettingValue = model.GlobalSettingsPerPageToDisplay.ToString();

                GlobalSetting smsContentsTrimOnGrid = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsContentsTrimOnGrid, true);
                smsContentsTrimOnGrid.SettingValue = model.SmsContentsTrimOnGrid.ToString();

                GlobalSetting smsContentsTrimLengthOnGrid = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsContentsTrimLengthOnGrid, true);
                smsContentsTrimLengthOnGrid.SettingValue = model.SmsContentsTrimLengthOnGrid.ToString();

                GlobalSetting smsErrorTrimLengthOnGrid = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsErrorTrimLengthOnGrid, true);
                smsErrorTrimLengthOnGrid.SettingValue = model.SmsErrorTrimLengthOnGrid.ToString();

                GlobalSetting cellPhoneNumberTrimOnGrid = context.GetGlobalSettingBySettingName(GlobalSettingName.CellPhoneNumberTrimOnGrid, true);
                cellPhoneNumberTrimOnGrid.SettingValue = model.CellPhoneNumberTrimOnGrid.ToString();

                GlobalSetting cellPhoneNumberTrimLengthOnGrid = context.GetGlobalSettingBySettingName(GlobalSettingName.CellPhoneNumberTrimLengthOnGrid, true);
                cellPhoneNumberTrimLengthOnGrid.SettingValue = model.CellPhoneNumberTrimLengthOnGrid.ToString();

                GlobalSetting maximumSmsDateRangeDaysToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.MaximumSmsDateRangeDaysToDisplay, true);
                maximumSmsDateRangeDaysToDisplay.SettingValue = model.MaximumSmsDateRangeDaysToDisplay.ToString();

                GlobalSetting smsProcessorMessageTrimOnGrid = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsProcessorMessageTrimOnGrid, true);
                smsProcessorMessageTrimOnGrid.SettingValue = model.SmsProcessorMessageTrimOnGrid.ToString();

                GlobalSetting smsProcessorMessageTrimLengthOnGrid = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsProcessorMessageTrimLengthOnGrid, true);
                smsProcessorMessageTrimLengthOnGrid.SettingValue = model.SmsProcessorMessageTrimLengthOnGrid.ToString();

                GlobalSetting smsDaysToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsDaysToDisplay, true);
                smsDaysToDisplay.SettingValue = model.SmsDaysToDisplay.ToString();

                GlobalSetting smsProcessorLogDaysToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.SmsProcessorLogDaysToDisplay, true);
                smsProcessorLogDaysToDisplay.SettingValue = model.SmsProcessorLogDaysToDisplay.ToString();

                GlobalSetting webRequestActivityDaysToDisplay = context.GetGlobalSettingBySettingName(GlobalSettingName.WebRequestActivityDaysToDisplay, true);
                webRequestActivityDaysToDisplay.SettingValue = model.WebRequestActivityDaysToDisplay.ToString();

                GlobalSetting defaultRepeatDaysInterval = context.GetGlobalSettingBySettingName(GlobalSettingName.DefaultRepeatDaysInterval, true);
                defaultRepeatDaysInterval.SettingValue = model.DefaultRepeatDaysInterval.ToString();

                GlobalSetting figlutPhoneNumber = context.GetGlobalSettingBySettingName(GlobalSettingName.FiglutPhoneNumber, true);
                figlutPhoneNumber.SettingValue = model.FiglutPhoneNumber;

                GlobalSetting figlutSupportEmailAddress = context.GetGlobalSettingBySettingName(GlobalSettingName.FiglutSupportEmailAddress, true);
                figlutSupportEmailAddress.SettingValue = model.FiglutSupportEmailAddress;

                GlobalSetting figlutMarketingEmailAddress = context.GetGlobalSettingBySettingName(GlobalSettingName.FiglutMarketingEmailAddress, true);
                figlutMarketingEmailAddress.SettingValue = model.FiglutMarketingEmailAddress;

                GlobalSetting figlutGeneralEmailAddress = context.GetGlobalSettingBySettingName(GlobalSettingName.FiglutGeneralEmailAddress, true);
                figlutGeneralEmailAddress.SettingValue = model.FiglutGeneralEmailAddress;

                GlobalSetting figlutAddress = context.GetGlobalSettingBySettingName(GlobalSettingName.FiglutAddress, true);
                figlutAddress.SettingValue = model.FiglutAddress;

                List<GlobalSetting> globalSettings = new List<GlobalSetting>()
                {
                    disableScreenScalingForMobileDevices,
                    createPersistentAuthenticationCookie,
                    logAllHttpHeaders,
                    defaultCurrencySymbol,
                    logGetWebRequestActivity,
                    logPostWebRequestActivity,
                    logPutWebRequestActivity,
                    logDeleteWebRequestActivity,
                    webRequestActivityUserAgentsToExclude,
                    enableWhoIsWebServiceQuery,
                    whoIsWebServiceUrl,
                    whoIsWebServiceRequestTimeout,
                    organizationIdentifierIndicator,
                    subscriberNameIndicator,
                    maxSmsSendMessageLength,
                    smsSendMessageSuffix,
                    organizationIdentifierMaxLength,
                    smsPerPagePageToDisplay,
                    organizationsPerPageToDisplay,
                    subscribersPerPageToDisplay,
                    usersPerPageToDisplay,
                    subscriptionsPerPageToDisplay,
                    smsProcessorsPerPageToDisplay,
                    smsProcessorLogsPerPageToDisplay,
                    webRequestActivityPerPageToDisplay,
                    smsCampaignsPerPageToDisplay,
                    countriesPerPageToDisplay,
                    publicHolidaysPerPageToDisplay,
                    smsMessageTemplatesPerPageToDisplay,
                    repeatSchedulesPerPageToDisplay,
                    repeatScheduleEntriesPerPageToDisplay,
                    globalSettingsPerPageToDisplay,
                    smsContentsTrimOnGrid,
                    smsContentsTrimLengthOnGrid,
                    smsErrorTrimLengthOnGrid,
                    cellPhoneNumberTrimOnGrid,
                    cellPhoneNumberTrimLengthOnGrid,
                    smsDaysToDisplay,
                    smsProcessorLogDaysToDisplay,
                    webRequestActivityDaysToDisplay,
                    maximumSmsDateRangeDaysToDisplay,
                    smsProcessorMessageTrimOnGrid,
                    smsProcessorMessageTrimLengthOnGrid,
                    defaultRepeatDaysInterval,
                    figlutPhoneNumber,
                    figlutSupportEmailAddress,
                    figlutMarketingEmailAddress,
                    figlutGeneralEmailAddress,
                    figlutAddress
                };
                context.SaveGlobalSettings(globalSettings);
                SpreadWebApp.Instance.RefreshGlobalSettingsFromDatabase();
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        #endregion //Actions
    }
}
