namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Csv;
    using Figlut.Spread.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class SmsProcessorController : SpreadController
    {
        #region Constants

        private const string SMS_PROCESSOR_GRID_PARTIAL_VIEW_NAME = "_SmsProcessorGrid";

        #endregion //Constants

        #region Methods

        private FilterModel<SmsProcessorModel> GetSmsProcessorFilterModel(SpreadEntityContext context, FilterModel<SmsProcessorModel> model)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<SmsProcessor> processorList = context.GetSmsProcessorsByFilter(model.SearchText);
            List<SmsProcessorModel> modelList = new List<SmsProcessorModel>();
            foreach (SmsProcessor s in processorList)
            {
                SmsProcessorModel m = new SmsProcessorModel();
                m.CopyPropertiesFromSmsProcessor(s);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllSmsProcessorCount();
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
                FilterModel<SmsProcessorModel> model = GetSmsProcessorFilterModel(context, new FilterModel<SmsProcessorModel>());
                if (model == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return View(new FilterModel<SmsProcessorModel>());
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                return View(model);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Index(FilterModel<SmsProcessorModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                FilterModel<SmsProcessorModel> resultModel = GetSmsProcessorFilterModel(context, model);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(SMS_PROCESSOR_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsSentLogModel>());
                }
                return PartialView(SMS_PROCESSOR_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid smsProcessorId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                SmsProcessor smsProcessor = context.GetSmsProcessor(smsProcessorId, true);
                context.Delete<SmsProcessor>(smsProcessor);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
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
                SmsProcessor smsProcessor = context.GetSmsProcessor(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (smsProcessor != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete Sms Processor '{0}'?", smsProcessor.Name);
                }
                PartialViewResult result = PartialView(CONFIRMATION_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
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
                SmsProcessor smsProcessor = context.GetSmsProcessor(model.Identifier, true);
                context.Delete<SmsProcessor>(smsProcessor);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
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
                model.ConfirmationMessage = "Delete all SMS Processsors currently loaded?";
                PartialViewResult result = PartialView(CONFIRMATION_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
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
                context.DeleteSmsProcessorsByFilter(model.SearchText);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
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
                List<SmsProcessor> smsProcessorList = context.GetSmsProcessorsByFilter(searchText);
                EntityCache<Guid, SmsProcessorCsv> cache = new EntityCache<Guid, SmsProcessorCsv>();
                foreach (SmsProcessor o in smsProcessorList)
                {
                    SmsProcessorCsv csv = new SmsProcessorCsv();
                    csv.CopyPropertiesFromSmsProcessor(o);
                    cache.Add(csv.SmsProcessorId, csv);
                }
                return GetCsvFileResult<SmsProcessorCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        #endregion //Actions
    }
}
