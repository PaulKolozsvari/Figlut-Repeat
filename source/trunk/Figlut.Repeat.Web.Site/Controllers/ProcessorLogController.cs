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

    public class ProcessorLogController : RepeatController
    {
        #region Constants

        private const string PROCESSOR_LOG_GRID_PARTIAL_VIEW_NAME = "_ProcessorLogGrid";

        #endregion //Constants

        #region Methods

        private FilterModel<ProcessorLogModel> GetProcessorLogFilterModel(
            RepeatEntityContext context, 
            FilterModel<ProcessorLogModel> model, 
            Nullable<Guid> processorId)
        {
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            DateTime startDate;
            DateTime endDate;
            if (!model.StartDate.HasValue || !model.EndDate.HasValue) //Has not been specified by the user on the page yet i.e. this is the first time the page is loading.
            {
                context.GetGlobalSettingProcessorLogDaysDateRange(out startDate, out endDate);
                model.StartDate = startDate;
                model.EndDate = endDate;
            }
            else
            {
                if (model.StartDate > model.EndDate)
                {
                    SetViewBagErrorMessage(string.Format("{0} may not be later than {1}.",
                        EntityReader<FilterModel<ProcessorLogModel>>.GetPropertyName(p => p.StartDate, true),
                        EntityReader<FilterModel<ProcessorLogModel>>.GetPropertyName(p => p.EndDate, true)));
                    return null;
                }
                startDate = model.StartDate.Value;
                endDate = model.EndDate.Value;
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            if (!model.IsAdministrator)
            {
                User currentUser = GetCurrentUser(context);
                SetViewBagErrorMessage(string.Format("User {0} does nto have administrator rights to view Processor Logs. ", currentUser.UserName));
                return null;
            }
            bool processorMessageTrimOnGrid = Convert.ToBoolean(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.ProcessorMessageTrimOnGrid].SettingValue);
            int processorMessageTrimLengthOnGrid = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.ProcessorMessageTrimLengthOnGrid].SettingValue);
            List<ProcessorLogView> processorLogViewList = null;
            processorLogViewList = context.GetProcessorLogViewsByFilter(model.SearchText, startDate, endDate, null);

            List<ProcessorLogModel> modelList = new List<ProcessorLogModel>();
            foreach (ProcessorLogView v in processorLogViewList)
            {
                ProcessorLogModel m = new ProcessorLogModel();
                m.CopyPropertiesFromProcessorLogView(v);
                if (processorMessageTrimOnGrid)
                {
                    m.MessageTrimmed = DataEditor.GetTrimmedMessageContents(v.Message, processorMessageTrimLengthOnGrid);
                }
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllProcessorLogsCount();
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
                FilterModel<ProcessorLogModel> model = GetProcessorLogFilterModel(context, new FilterModel<ProcessorLogModel>(), null);
                if (model == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return View(new FilterModel<ProcessorLogModel>());
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
        public ActionResult Index(FilterModel<ProcessorLogModel> model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                FilterModel<ProcessorLogModel> resultModel = GetProcessorLogFilterModel(context, model, null);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(PROCESSOR_LOG_GRID_PARTIAL_VIEW_NAME, new FilterModel<ProcessorLogModel>());
                }
                return PartialView(PROCESSOR_LOG_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid processorId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                ProcessorLog processorLog = context.GetProcessorLog(processorId, true);
                context.Delete<ProcessorLog>(processorLog, processorLog.Message);
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
                ProcessorLog processorLog = context.GetProcessorLog(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (processorLog != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete Processor Log message at '{0}' ?", processorLog.DateCreated);
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
                ProcessorLog processorLog = context.GetProcessorLog(model.Identifier, true);
                context.Delete<ProcessorLog>(processorLog, processorLog.ProcessorLogId);
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
                        "Delete all Processor Log messages currently loaded between {0}-{1}-{2} and {3}-{4}-{5} ?",
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
                context.DeleteProcessorLogByFilter(model.SearchText, model.StartDate.Value, model.EndDate.Value, null);
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
                List<ProcessorLogView> processorLogViewList = context.GetProcessorLogViewsByFilter(searchText, startDate.Value, endDate.Value, null);
                EntityCache<Guid, ProcessorLogCsv> cache = new EntityCache<Guid, ProcessorLogCsv>();
                foreach (ProcessorLogView v in processorLogViewList)
                {
                    ProcessorLogCsv csv = new ProcessorLogCsv();
                    csv.CopyPropertiesFromProcessorLogView(v);
                    cache.Add(csv.ProcessorLogId, csv);
                }
                return GetCsvFileResult<ProcessorLogCsv>(cache);
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