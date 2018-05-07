namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Server.Toolkit.Web;
    using Figlut.Server.Toolkit.Web.Client;
    using Figlut.Server.Toolkit.Web.Client.IP_API;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class SpreadController : Controller
    {
        #region Constants

        protected const string CONFIRMATION_DIALOG_PARTIAL_VIEW_NAME = "_ConfirmationDialog";
        protected const string CONFIRMATION_DIALOG_DIV_ID = "dlgConfirmation"; //Used to manage div making up the dialog.

        protected const string WAIT_DIALOG_PARTIAL_VIEW_NAME = "_WaitDialog";
        protected const string WAIT_DIALOG_DIV_ID = "dlgWait";

        protected const string INFORMATION_DIALOG_PARTIAL_VIEW_NAME = "_InformationDialog";
        protected const string INFORMATION_DIALOG_DIV_ID = "dlgInformation";

        protected const string WEB_REQUEST_ACTIVITY_SOURCE_APPLICATION = "WebSite";

        #endregion //Constants

        #region Actions

        public virtual ActionResult WaitDialog(string message)
        {
            try
            {
                WaitModel model = new WaitModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = WAIT_DIALOG_DIV_ID;
                model.WaitMessage = message == null ? string.Empty : message;
                PartialViewResult result = PartialView(WAIT_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public virtual ActionResult InformationDialog(string message)
        {
            try
            {
                InformationModel model = new InformationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = INFORMATION_DIALOG_DIV_ID;
                model.InformationMessage = message == null ? string.Empty : message;
                PartialViewResult result = PartialView(INFORMATION_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        #endregion //Actions

        #region Methods

        protected string GetCurrentActionName()
        {
            string result = this.ControllerContext.RouteData.Values["action"].ToString();
            return result;
        }

        protected string GetCurrentControllerName()
        {
            string result = this.ControllerContext.RouteData.Values["controller"].ToString();
            return result;
        }

        protected string GetWebRequestVerb()
        {
            return Request.RequestType;
        }

        protected string GetFullRequestUrl()
        {
            return Request.Url != null ? Request.Url.AbsoluteUri : null;
        }

        protected string GetFullRequestReferrerUrl()
        {
            return Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri : null;
        }

        protected string GetUserAgent()
        {
            return Request.UserAgent;
        }

        protected string GetUserHostAddress()
        {
            return Request.UserHostAddress;
        }

        protected string GetUserHostName()
        {
            return Request.UserHostName;
        }

        protected bool IsCrawler()
        {
            return Request.Browser == null ? false : Request.Browser.Crawler;
        }

        protected bool IsMobileDevice()
        {
            return Request.Browser == null ? false : Request.Browser.IsMobileDevice;
        }

        public string GetMobileDeviceManufacturer()
        {
            return IsMobileDevice() ? Request.Browser.MobileDeviceManufacturer : null;
        }

        public string GetMobileDeviceModel()
        {
            return IsMobileDevice() ? Request.Browser.MobileDeviceModel : null;
        }

        public string GetPlatform()
        {
            return Request.Browser == null ? null : Request.Browser.Platform;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionDescriptor.ActionName;
            if (actionName.Contains("Dialog") || actionName.ToLower().Contains("dialog") || controllerName == "WebRequestActivity")
            {
                return;
            }
            LogWebRequestActivity(controllerName, actionName, true);
            if (Convert.ToBoolean(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.LogAllHttpHeaders].SettingValue))
            {
                LogHeaders();
            }
            base.OnActionExecuting(filterContext);
        }

        private bool ExcludeWebRequestActivityByUserAgent(string currentUserAgent)
        {
            string currentUserAgentLower = currentUserAgent.Trim().ToLower();
            string userAgentsToExcludeCsv = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.WebRequestActivityUserAgentsToExclude].SettingValue;
            foreach(string userAgentText in userAgentsToExcludeCsv.Split(','))
            {
                string s = userAgentText.Trim().ToLower();
                if (currentUserAgentLower.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ExcludeWebRequestActivityByRequestVerb(string currentRequestVerb)
        {
            bool logGet = Convert.ToBoolean(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.LogGetWebRequestActivity].SettingValue);
            bool logPost = Convert.ToBoolean(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.LogPostWebRequestActivity].SettingValue);
            bool logPut = Convert.ToBoolean(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.LogPutWebRequestActivity].SettingValue);
            bool logDelete = Convert.ToBoolean(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.LogDeleteWebRequestActivity].SettingValue);
            string currentRequestVerbLower = currentRequestVerb.ToLower();
            if ((currentRequestVerbLower.Contains(HttpVerbs.Post.ToString().ToLower()) && !logPost) ||
                (currentRequestVerbLower.Contains(HttpVerbs.Delete.ToString().ToLower()) && !logDelete) ||
                (currentRequestVerbLower.Contains(HttpVerbs.Put.ToString().ToLower()) && !logPut) ||
                (currentRequestVerbLower.Contains(HttpVerbs.Get.ToString().ToLower()) && !logGet))
            {
                return true;
            }
            return false;
        }

        private query GetWhoIsQuery(string ipAddress, bool handleExceptions)
        {
            query result = null;
            if (Convert.ToBoolean(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.EnableWhoIsWebServiceQuery].SettingValue))
            {
                int timeout = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.WhoIsWebServiceRequestTimeout].SettingValue);
                result = SpreadWebApp.Instance.WhoIsWebClient.GetWhoIsInfo(ipAddress, timeout, handleExceptions);
            }
            return result;
        }

        private string GetClientType(bool isCrawler, bool isMobileDevice)
        {
            string result = null;
            if (isMobileDevice)
            {
                result = "Mobile";
            }
            else if (isCrawler)
            {
                result = "Crawler";
            }
            else
            {
                result = "PC";
            }
            return result;
        }

        private void LogWebRequestActivity(string controllerName, string actionName, bool handleExceptions)
        {
            try
            {
                DateTime requestDate = DateTime.Now;
                string requestVerb = GetWebRequestVerb();
                string requestUrl = GetFullRequestUrl();
                string requestReferrerUrl = GetFullRequestReferrerUrl();
                string userAgent = GetUserAgent();
                string userHostAddress = GetUserHostAddress();
                string userHostName = GetUserHostName();

                bool isCrawler = IsCrawler();
                bool isMobileDevice = IsMobileDevice();
                string mobileDeviceManufacturer = GetMobileDeviceManufacturer();
                string mobileDeviceModel = GetMobileDeviceModel();
                string platform = GetPlatform();
                string clientType = GetClientType(isCrawler, isMobileDevice);
                bool logWebRequestActivity = !(ExcludeWebRequestActivityByUserAgent(userAgent) || ExcludeWebRequestActivityByRequestVerb(requestVerb));
                query whoIsQuery = null;
                if (logWebRequestActivity)
                {
                    whoIsQuery = GetWhoIsQuery(userHostAddress, true);
                }
                string currentUserName = this.Request.IsAuthenticated ? this.User.Identity.Name : null;
                SpreadEntityContext context = SpreadEntityContext.Create();
                WebRequestActivity webRequestActivity = context.LogWebRequestActivity(
                    logWebRequestActivity,
                    requestVerb,
                    requestUrl,
                    requestReferrerUrl,
                    userAgent,
                    userHostAddress,
                    userHostName,
                    actionName,
                    controllerName,
                    clientType,
                    isCrawler,
                    isMobileDevice,
                    mobileDeviceManufacturer,
                    mobileDeviceModel,
                    platform,
                    currentUserName,
                    WEB_REQUEST_ACTIVITY_SOURCE_APPLICATION,
                    null,
                    whoIsQuery,
                    requestDate);
            }
            catch (Exception ex)
            {
                if (!handleExceptions)
                {
                    throw ex;
                }
                ExceptionHandler.HandleException(ex);
            }
        }

        protected void LogHeaders()
        {
            string allHeadersFullString = GetAllHeadersFullString();
            string allHeadersFormatted = GetAllHeadersFormatted();
            GOC.Instance.Logger.LogMessage(new LogMessage(allHeadersFullString, LogMessageType.Information, LoggingLevel.Maximum));
        }

        public void SetViewBagErrorMessage(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
        }

        public JsonResult GetJsonResult(bool success)
        {
            return Json(new { Success = success, ErrorMsg = string.Empty });
        }

        public JsonResult GetJsonResult(bool success, string errorMessage)
        {
            return Json(new { Success = success, ErrorMsg = errorMessage });
        }

        public JsonResult GetJsonFileResult(bool success, string fileName)
        {
            return Json(new { Success = success, FileName = fileName });
        }

        public RedirectToRouteResult RedirectToHome()
        {
            return RedirectToAction("Index", "Home");
        }

        public RedirectToRouteResult RedirectToError(string message)
        {
            return RedirectToAction("Error", "Information", new { errorMessage = message });
        }

        public RedirectToRouteResult RedirectToInformation(string message)
        {
            return RedirectToAction("Information", "Information", new { informationMessage = message });
        }

        public RedirectToRouteResult RedirectToIndex()
        {
            return RedirectToAction("Index");
        }

        public User GetUser(string identifier, SpreadEntityContext context, bool throwExceptionOnNotFound)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            return context.GetUserByIdentifier(identifier, true);
        }

        public User GetCurrentUser(SpreadEntityContext context)
        {
            return GetUser(this.User.Identity.Name, context, true);
        }

        public User GetCurrentUser(SpreadEntityContext context, bool throwExceptionOnNotFound)
        {
            return GetUser(this.User.Identity.Name, context, throwExceptionOnNotFound);
        }

        public Organization GetOrganizationFromUser(SpreadEntityContext context, User user, bool throwExceptionOnNotFound)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            Organization result = null;
            if (!user.OrganizationId.HasValue && throwExceptionOnNotFound)
            {
                throw new ArgumentNullException(string.Format("Current {0} '{1}' is not associated to an {2}.",
                    typeof(User).Name,
                    user.UserName,
                    typeof(Organization).Name));
            }
            result = context.GetOrganization(user.OrganizationId.Value, throwExceptionOnNotFound);
            return result;
        }

        public Organization GetCurrentOrganization(SpreadEntityContext context, bool throwExceptionOnNotFound)
        {
            if(context == null)
            {
                context = SpreadEntityContext.Create();
            }
            User currentUser = GetCurrentUser(context);
            return GetOrganizationFromUser(context, currentUser, throwExceptionOnNotFound);
        }

        public bool IsCurrentUserOfRole(UserRole roleToCheck, SpreadEntityContext context)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            return context.IsUserOfRole(this.User.Identity.Name, roleToCheck);
        }

        public bool IsCurrentUserAdministrator(SpreadEntityContext context)
        {
            if (!this.Request.IsAuthenticated)
            {
                return false;
            }
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            return context.IsUserOfRole(this.User.Identity.Name, UserRole.Administrator);
        }

        public bool CurrentUserHasAccessToOrganization(Guid organizationId, SpreadEntityContext context)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            if (IsCurrentUserAdministrator(context) ||
                GetCurrentOrganization(context, true).OrganizationId == organizationId)
            {
                return true;
            }
            return false;
        }

        #region Header Methods

        public string GetAllHeadersFullString()
        {
            return Request.Headers.ToString();
        }

        public string GetAllHeadersFormatted()
        {
            StringBuilder result = new StringBuilder();
            foreach (var key in Request.Headers.AllKeys)
            {
                result.AppendLine(string.Format("{0}={1}", key, Request.Headers[key]));
            }
            return result.ToString();
        }

        public string GetHeader(string key, bool throwExceptionOnNotFound)
        {
            string result = string.Empty;
            if (Request != null && Request.Headers.HasKeys())
            {
                result = Request.Headers.Get(key);
            }
            if (result == null && throwExceptionOnNotFound)
            {
                throw new NullReferenceException(string.Format("Could not find HTTP Header with key {0}.", key));
            }
            return result;
        }

        public FileContentResult GetCsvFileResult<E>(EntityCache<Guid, E> cache) where E : class
        {
            string filePath = Path.GetTempFileName();
            cache.ExportToCsv(filePath, null, false, false);
            string downloadFileName = string.Format("{0}-{1}.csv", typeof(E).Name, DateTime.Now);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileBytes, "text/plain", downloadFileName);
        }

        protected void GetConfirmationModelFromSearchParametersString(
            string searchParametersString,
            out string[] searchParameters,
            out string searchText)
        {
            searchText = string.Empty;
            searchParameters = searchParametersString.Split('|');
            if (!string.IsNullOrEmpty(searchParametersString) && searchParameters.Length > 0)
            {
                searchText = searchParameters[0];
            }
        }

        protected void GetConfirmationModelFromSearchParametersString(
            string searchParametersString,
            out string[] searchParameters,
            out string searchText,
            out Nullable<DateTime> startDate,
            out Nullable<DateTime> endDate)
        {
            searchText = string.Empty;
            startDate = null;
            endDate = null;
            searchParameters = searchParametersString.Split('|');
            if (!string.IsNullOrEmpty(searchParametersString) && searchParameters.Length >= 3)
            {
                searchText = searchParameters[0];
                DateTime startDateParsed;
                DateTime endDateParsed;
                if (DateTime.TryParse(searchParameters[1], out startDateParsed))
                {
                    startDate = startDateParsed;
                }
                if (DateTime.TryParse(searchParameters[2], out endDateParsed))
                {
                    endDate = endDateParsed;
                }
            }
        }

        #endregion //Header Methods

        #endregion //Methods
    }
}
