namespace Figlut.Repeat.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Csv;
    using Figlut.Repeat.Web.Site.Configuration;
    using Figlut.Repeat.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class ProcessorController : RepeatController
    {
        #region Constants

        private const string PROCESSOR_GRID_PARTIAL_VIEW_NAME = "_ProcessorGrid";
        private const string EDIT_PROCESSOR_DIALOG_PARTIAL_VIEW_NAME = "_EditProcessorDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<ProcessorModel> GetProcessorFilterModel(RepeatEntityContext context, FilterModel<ProcessorModel> model)
        {
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<Processor> processorList = context.GetProcessorsByFilter(model.SearchText);
            List<ProcessorModel> modelList = new List<ProcessorModel>();
            foreach (Processor s in processorList)
            {
                ProcessorModel m = new ProcessorModel();
                m.CopyPropertiesFromProcessor(s);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllProcessorCount();
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
                FilterModel<ProcessorModel> model = GetProcessorFilterModel(context, new FilterModel<ProcessorModel>());
                if (model == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return View(new FilterModel<ProcessorModel>());
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
        public ActionResult Index(FilterModel<ProcessorModel> model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                FilterModel<ProcessorModel> resultModel = GetProcessorFilterModel(context, model);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(PROCESSOR_GRID_PARTIAL_VIEW_NAME, new FilterModel<ProcessorModel>());
                }
                return PartialView(PROCESSOR_GRID_PARTIAL_VIEW_NAME, resultModel);
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
                Processor processor = context.GetProcessor(processorId, true);
                context.Delete<Processor>(processor);
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
                Processor processor = context.GetProcessor(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (processor != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete Processor '{0}'?", processor.Name);
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
                Processor processor = context.GetProcessor(model.Identifier, true);
                context.Delete<Processor>(processor);
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
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                model.SearchText = searchText;
                model.ConfirmationMessage = "Delete all Processsors currently loaded?";
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
                context.DeleteProcessorsByFilter(model.SearchText);
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
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<Processor> processorsList = context.GetProcessorsByFilter(searchText);
                EntityCache<Guid, ProcessorCsv> cache = new EntityCache<Guid, ProcessorCsv>();
                foreach (Processor o in processorsList)
                {
                    ProcessorCsv csv = new ProcessorCsv();
                    csv.CopyPropertiesFromProcessor(o);
                    cache.Add(csv.ProcessorId, csv);
                }
                return GetCsvFileResult<ProcessorCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> processorId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                if (!processorId.HasValue)
                {
                    return PartialView(EDIT_PROCESSOR_DIALOG_PARTIAL_VIEW_NAME, new ProcessorModel());
                }
                Processor processor = context.GetProcessor(processorId.Value, true);
                ProcessorModel model = new ProcessorModel();
                model.CopyPropertiesFromProcessor(processor);
                PartialViewResult result = PartialView(EDIT_PROCESSOR_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(ProcessorModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                Processor processor = context.GetProcessor(model.ProcessorId, true);
                model.CopyPropertiesToProcessor(processor);
                context.Save<Processor>(processor, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        #endregion //Actions
    }
}
