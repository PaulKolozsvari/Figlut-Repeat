namespace Figlut.Repeat.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Csv;
    using Figlut.Repeat.ORM.Helpers;
    using Figlut.Repeat.Web.Site.Configuration;
    using Figlut.Repeat.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class WebRequestActivityController : RepeatController
    {
        #region Constants

        private const string WEB_REQUEST_ACTIVITY_GRID_PARTIAL_VIEW_NAME = "_WebRequestActivityGrid";
        private const string EDIT_WEB_REQUEST_ACTIVITY_DIALOG_PARTIAL_VIEW_NAME = "_EditWebRequestActivityDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<WebRequestActivityModel> GetWebRequestActivityFilterModel(
            RepeatEntityContext context,
            FilterModel<WebRequestActivityModel> model,
            Nullable<Guid> userId)
        {
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            DateTime startDate;
            DateTime endDate;
            if (!model.StartDate.HasValue || !model.EndDate.HasValue) //Has not been specified by the user on the page yet i.e. this is the first time the page is loading.
            {
                context.GetGlobalSettingWebRequestActivityDaysDataRange(out startDate, out endDate);
                model.StartDate = startDate;
                model.EndDate = endDate;
            }
            else
            {
                if (model.StartDate > model.EndDate)
                {
                    SetViewBagErrorMessage(string.Format("{0} may not be later than {1}.",
                        Figlut.Server.Toolkit.Data.EntityReader<FilterModel<WebRequestActivityModel>>.GetPropertyName(p => p.StartDate, true),
                        EntityReader<FilterModel<WebRequestActivityModel>>.GetPropertyName(p => p.EndDate, true)));
                    return null;
                }
                startDate = model.StartDate.Value;
                endDate = model.EndDate.Value;
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            if (!model.IsAdministrator)
            {
                User currentUser = GetCurrentUser(context);
                SetViewBagErrorMessage(string.Format("User {0} does nto have administrator rights to view Web Request Activities. ", currentUser.UserName));
                return null;
            }
            List<WebRequestActivity> webRequestActivityList = null;
            webRequestActivityList = context.GetWebRequestActivityByFilter(model.SearchText, startDate, endDate, userId);

            List<WebRequestActivityModel> modelList = new List<WebRequestActivityModel>();
            foreach (WebRequestActivity w in webRequestActivityList)
            {
                WebRequestActivityModel m = new WebRequestActivityModel();
                m.CopyPropertiesFromWebRequestActivity(w);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllWebRequestActivitiesCount();
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index()
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                FilterModel<WebRequestActivityModel> model = GetWebRequestActivityFilterModel(context, new FilterModel<WebRequestActivityModel>(), null);
                if (model == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return View(new FilterModel<WebRequestActivityModel>());
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                return View(model);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Index(FilterModel<WebRequestActivityModel> model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                FilterModel<WebRequestActivityModel> resultModel = GetWebRequestActivityFilterModel(context, model, null);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(WEB_REQUEST_ACTIVITY_GRID_PARTIAL_VIEW_NAME, new FilterModel<WebRequestActivityModel>());
                }
                return PartialView(WEB_REQUEST_ACTIVITY_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid webRequestActivityId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                WebRequestActivity webRequestActivity = context.GetWebRequestActivity(webRequestActivityId, true);
                context.Delete<WebRequestActivity>(webRequestActivity);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult ConfirmDeleteDialog(Guid identifier)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                WebRequestActivity webRequestActivity = context.GetWebRequestActivity(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (webRequestActivity != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete Web Request Activity at '{0}' ?", webRequestActivity.DateCreated);
                }
                PartialViewResult result = PartialView(CONFIRMATION_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ConfirmDeleteDialog(ConfirmationModel model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                WebRequestActivity webRequestActivity = context.GetWebRequestActivity(model.Identifier, true);
                context.Delete<WebRequestActivity>(webRequestActivity);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult ConfirmDeleteAllDialog(string searchParametersString)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
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
                Nullable<DateTime> startDate;
                Nullable<DateTime> endDate;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText, out startDate, out endDate);

                model.SearchText = searchText;
                model.StartDate = startDate;
                model.EndDate = endDate;

                if (model.StartDate.HasValue && model.EndDate.HasValue)
                {
                    model.ConfirmationMessage = string.Format(
                        "Delete all Web Request Activities currently loaded between {0}-{1}-{2} and {3}-{4}-{5} ?",
                        model.StartDate.Value.Year.ToString(),
                        model.StartDate.Value.Month.ToString(),
                        model.StartDate.Value.Day.ToString(),
                        model.EndDate.Value.Year.ToString(),
                        model.EndDate.Value.Month.ToString(),
                        model.EndDate.Value.Day.ToString());
                }
                PartialViewResult result = PartialView(CONFIRMATION_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ConfirmDeleteAllDialog(ConfirmationModel model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                context.DeleteWebRequestActivityByFilter(model.SearchText, model.StartDate.Value, model.EndDate.Value, null);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> webRequestActivityId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                if (!webRequestActivityId.HasValue)
                {
                    return PartialView(EDIT_WEB_REQUEST_ACTIVITY_DIALOG_PARTIAL_VIEW_NAME, new WebRequestActivityModel());
                }
                WebRequestActivity webRequestActivity = context.GetWebRequestActivity(webRequestActivityId.Value, true);
                WebRequestActivityModel model = new WebRequestActivityModel();
                model.CopyPropertiesFromWebRequestActivity(webRequestActivity);
                PartialViewResult result = PartialView(EDIT_WEB_REQUEST_ACTIVITY_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult EditDialog(WebRequestActivityModel model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                WebRequestActivity webRequestActivity = context.GetWebRequestActivity(model.WebRequestActivityId, true);
                model.CopyPropertiesToWebRequestActivity(webRequestActivity);
                context.Save<WebRequestActivity>(webRequestActivity, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult DownloadCsvFile(string searchParametersString)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                string[] searchParameters;
                string searchText;
                Nullable<DateTime> startDate;
                Nullable<DateTime> endDate;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText, out startDate, out endDate);
                if (!startDate.HasValue)
                {
                    throw new ArgumentException("Start Date not specified.");
                }
                if (!endDate.HasValue)
                {
                    throw new ArgumentException("End Date not specified.");
                }
                List<WebRequestActivity> webRequestActivityList = context.GetWebRequestActivityByFilter(searchText, startDate.Value, endDate.Value, null);
                EntityCache<Guid, WebRequestActivityCsv> cache = new EntityCache<Guid, WebRequestActivityCsv>();
                foreach (WebRequestActivity w in webRequestActivityList)
                {
                    WebRequestActivityCsv csv = new WebRequestActivityCsv();
                    csv.CopyPropertiesFromWebRequestActivity(w);
                    cache.Add(csv.WebRequestActivityId, csv);
                }
                return GetCsvFileResult<WebRequestActivityCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        #endregion //Actions
    }
}
