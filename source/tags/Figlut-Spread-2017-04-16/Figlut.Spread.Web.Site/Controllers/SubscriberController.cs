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

    public class SubscriberController : SpreadController
    {
        #region Constants

        private const string SUBSCRIBER_GRID_PARTIAL_VIEW_NAME = "_SubscriberGrid";
        private const string EDIT_SUBSCRIBER_DIALOG_PARTIAL_VIEW_NAME = "_EditSubscriberDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<SubscriberModel> GetSubscriberFilterModel(SpreadEntityContext context, FilterModel<SubscriberModel> model)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<Subscriber> subscriberList = context.GetSubscribersByFilter(model.SearchText, null);
            List<SubscriberModel> modelList = new List<SubscriberModel>();
            foreach (Subscriber s in subscriberList)
            {
                SubscriberModel m = new SubscriberModel();
                m.CopyPropertiesFromSubscriber(s);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllSubscriberCount();
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
                FilterModel<SubscriberModel> model = GetSubscriberFilterModel(context, new FilterModel<SubscriberModel>());
                if (model == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return View(new FilterModel<SubscriberModel>());
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
        public ActionResult Index(FilterModel<SubscriberModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                FilterModel<SubscriberModel> resultModel = GetSubscriberFilterModel(context, model);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(SUBSCRIBER_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsSentLogModel>());
                }
                return PartialView(SUBSCRIBER_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid subscriberId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                Subscriber subscriber = context.GetSubscriber(subscriberId, true);
                context.Delete<Subscriber>(subscriber);
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
                Subscriber subscriber = context.GetSubscriber(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (subscriber != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete {0} '{1}'?", 
                        typeof(Subscriber).Name,
                        !string.IsNullOrEmpty(subscriber.Name) ? subscriber.Name : subscriber.CellPhoneNumber);
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
                Subscriber subscriber = context.GetSubscriber(model.Identifier, true);
                context.Delete<Subscriber>(subscriber);
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
                model.ConfirmationMessage = "Delete all Subscribers currently loaded?";
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
                context.DeleteSubscribersByFilter(model.SearchText, null);
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
                List<Subscriber> subscriberList = context.GetSubscribersByFilter(searchText, null);
                EntityCache<Guid, SubscriberCsv> cache = new EntityCache<Guid, SubscriberCsv>();
                foreach (Subscriber s in subscriberList)
                {
                    SubscriberCsv csv = new SubscriberCsv();
                    csv.CopyPropertiesFromSubscriber(s);
                    cache.Add(csv.SubscriberId, csv);
                }
                return GetCsvFileResult<SubscriberCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> subscriberId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                if (!subscriberId.HasValue)
                {
                    return PartialView(EDIT_SUBSCRIBER_DIALOG_PARTIAL_VIEW_NAME, new SubscriberModel());
                }
                Subscriber subscriber = context.GetSubscriber(subscriberId.Value, true);
                SubscriberModel model = new SubscriberModel();
                model.CopyPropertiesFromSubscriber(subscriber);
                PartialViewResult result = PartialView(EDIT_SUBSCRIBER_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult EditDialog(SubscriberModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                Subscriber subscriber = context.GetSubscriber(model.SubscriberId, true);
                bool subscriberCellPhoneNUmberHasChanged = subscriber.CellPhoneNumber != model.CellPhoneNumber;
                if (subscriberCellPhoneNUmberHasChanged)
                {
                    Subscriber original = context.GetSubscriberByCellPhoneNumber(model.CellPhoneNumber, false);
                    if (original != null)
                    {
                        return GetJsonResult(false, string.Format("An {0} with the {1} of '{2}' already exists.",
                            typeof(Subscriber).Name,
                            EntityReader<Subscriber>.GetPropertyName(p => p.CellPhoneNumber, true),
                            model.CellPhoneNumber));
                    }
                }
                model.CopyPropertiesToSubsriber(subscriber);
                context.Save<Subscriber>(subscriber, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        #endregion //Actions
    }
}
