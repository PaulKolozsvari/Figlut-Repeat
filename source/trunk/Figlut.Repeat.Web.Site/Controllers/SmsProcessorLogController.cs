namespace Figlut.Repeat.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Csv;
    using Figlut.Repeat.ORM.Helpers;
    using Figlut.Repeat.ORM.Views;
    using Figlut.Repeat.Web.Site.Configuration;
    using Figlut.Repeat.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class SmsProcessorLogController : RepeatController
    {
        #region Constants

        private const string SMS_PROCESSOR_LOG_GRID_PARTIAL_VIEW_NAME = "_SmsProcessorLogGrid";

        #endregion //Constants

        #region Methods

        private FilterModel<SmsProcessorLogModel> GetSmsProcessorLogFilterModel(
            RepeatEntityContext context, 
            FilterModel<SmsProcessorLogModel> model, 
            Nullable<Guid> smsProcessorId)
        {
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            DateTime startDate;
            DateTime endDate;
            if (!model.StartDate.HasValue || !model.EndDate.HasValue) //Has not been specified by the user on the page yet i.e. this is the first time the page is loading.
            {
                context.GetGlobalSettingSmsProcessorLogDaysDateRange(out startDate, out endDate);
                model.StartDate = startDate;
                model.EndDate = endDate;
            }
            else
            {
                if (model.StartDate > model.EndDate)
                {
                    SetViewBagErrorMessage(string.Format("{0} may not be later than {1}.",
                        EntityReader<FilterModel<SmsProcessorLogModel>>.GetPropertyName(p => p.StartDate, true),
                        EntityReader<FilterModel<SmsProcessorLogModel>>.GetPropertyName(p => p.EndDate, true)));
                    return null;
                }
                startDate = model.StartDate.Value;
                endDate = model.EndDate.Value;
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            if (!model.IsAdministrator)
            {
                User currentUser = GetCurrentUser(context);
                SetViewBagErrorMessage(string.Format("User {0} does nto have administrator rights to view SMS Processor Logs. ", currentUser.UserName));
                return null;
            }
            bool smsProcessorMessageTrimOnGrid = Convert.ToBoolean(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.SmsProcessorMessageTrimOnGrid].SettingValue);
            int smsProcessorMessageTrimLengthOnGrid = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.SmsProcessorMessageTrimLengthOnGrid].SettingValue);
            List<SmsProcessorLogView> smsProcessorLogViewList = null;
            smsProcessorLogViewList = context.GetSmsProcessorLogViewsByFilter(model.SearchText, startDate, endDate, null);

            List<SmsProcessorLogModel> modelList = new List<SmsProcessorLogModel>();
            foreach (SmsProcessorLogView v in smsProcessorLogViewList)
            {
                SmsProcessorLogModel m = new SmsProcessorLogModel();
                m.CopyPropertiesFromSmsProcessorLogView(v);
                if (smsProcessorMessageTrimOnGrid)
                {
                    m.MessageTrimmed = DataEditor.GetTrimmedMessageContents(v.Message, smsProcessorMessageTrimLengthOnGrid);
                }
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllSmsProcessorLogsCount();
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
                FilterModel<SmsProcessorLogModel> model = GetSmsProcessorLogFilterModel(context, new FilterModel<SmsProcessorLogModel>(), null);
                if (model == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return View(new FilterModel<SmsProcessorLogModel>());
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
        public ActionResult Index(FilterModel<SmsProcessorLogModel> model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                FilterModel<SmsProcessorLogModel> resultModel = GetSmsProcessorLogFilterModel(context, model, null);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(SMS_PROCESSOR_LOG_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsProcessorLogModel>());
                }
                return PartialView(SMS_PROCESSOR_LOG_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid smsProcessorLogId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                SmsProcessorLog smsProcessorLog = context.GetSmsProcessorLog(smsProcessorLogId, true);
                context.Delete<SmsProcessorLog>(smsProcessorLog);
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
                SmsProcessorLog smsProcessorLog = context.GetSmsProcessorLog(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (smsProcessorLog != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete SMS Processor Log message at '{0}' ?", smsProcessorLog.DateCreated);
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
                SmsProcessorLog smsProcessorLog = context.GetSmsProcessorLog(model.Identifier, true);
                context.Delete<SmsProcessorLog>(smsProcessorLog);
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
                        "Delete all SMS Processor Log messages currently loaded between {0}-{1}-{2} and {3}-{4}-{5} ?",
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
                context.DeleteSmsProcessorLogByFilter(model.SearchText, model.StartDate.Value, model.EndDate.Value, null);
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
                List<SmsProcessorLogView> smsProcessorLogViewList = context.GetSmsProcessorLogViewsByFilter(searchText, startDate.Value, endDate.Value, null);
                EntityCache<Guid, SmsProcessorLogCsv> cache = new EntityCache<Guid, SmsProcessorLogCsv>();
                foreach (SmsProcessorLogView v in smsProcessorLogViewList)
                {
                    SmsProcessorLogCsv csv = new SmsProcessorLogCsv();
                    csv.CopyPropertiesFromSmsProcessorLogView(v);
                    cache.Add(csv.SmsProcessorLogId, csv);
                }
                return GetCsvFileResult<SmsProcessorLogCsv>(cache);
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