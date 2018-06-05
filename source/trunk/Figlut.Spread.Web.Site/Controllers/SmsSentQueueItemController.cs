namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Csv;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;

    #endregion //Using Directives

    public class SmsSentQueueItemController : SpreadController
    {
        #region Constants

        private const string SMS_SENT_QUEUE_ITEM_GRID_PARTIAL_VIEW_NAME = "_SmsSentQueueItemGrid";
        private const string EDIT_SMS_SENT_QUEUE_ITEM_DIALOG_PARTIAL_VIEW_NAME = "_EditSmsSentQueueItemDialog";

        #endregion //Constants

        #region Methods

        public FilterModel<SmsSentQueueItemModel> GetSmsSentQueueItemFilterModel(
            SpreadEntityContext context,
            FilterModel<SmsSentQueueItemModel> model,
            Nullable<Guid> smsCampaignId)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<SmsSentQueueItem> smsSentQueueItems = context.GetSmsSentQueueItemsByFilter(model.SearchText, smsCampaignId);
            List<SmsSentQueueItemModel> modelList = new List<SmsSentQueueItemModel>();
            foreach (SmsSentQueueItem i in smsSentQueueItems)
            {
                SmsSentQueueItemModel m = new SmsSentQueueItemModel();
                m.CopyPropertiesFromSmsSentQueueItem(i);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllSmsSentQueueItemCount(false);
            if (smsCampaignId.HasValue)
            {
                SmsCampaign smsCampaign = context.GetSmsCampaign(smsCampaignId.Value, false);
                if (smsCampaign != null)
                {
                    model.ParentId = smsCampaign.SmsCampaignId;
                    model.ParentCaption = smsCampaign.Name;
                }
            }
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index(Nullable<Guid> smsCampaignId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                Organization currentOrganization = GetCurrentOrganization(context, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(currentOrganization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                FilterModel<SmsSentQueueItemModel> model = GetSmsSentQueueItemFilterModel(context, new FilterModel<SmsSentQueueItemModel>(), smsCampaignId);
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
        public ActionResult Index(FilterModel<SmsSentQueueItemModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                Organization currentOrganization = GetCurrentOrganization(context, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(currentOrganization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                FilterModel<SmsSentQueueItemModel> resultModel = GetSmsSentQueueItemFilterModel(context, model, model.ParentId);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(SMS_SENT_QUEUE_ITEM_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsSentQueueItemModel>());
                }
                return PartialView(SMS_SENT_QUEUE_ITEM_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid smsSentQueueItemId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                Organization currentOrganization = GetCurrentOrganization(context, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(currentOrganization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                SmsSentQueueItem smsSentQueueItem = context.GetSmsSentQueueItem(smsSentQueueItemId, true);
                context.Delete<SmsSentQueueItem>(smsSentQueueItem);
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
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                SmsSentQueueItem smsSentQueueItem = context.GetSmsSentQueueItem(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (smsSentQueueItem != null && smsSentQueueItem.SmsCampaignId.HasValue)
                {
                    SmsCampaign smsCampaign = context.GetSmsCampaign(smsSentQueueItem.SmsCampaignId.Value, true);
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete selected SMS Queue Item for Campaign '{0}'?", smsCampaign.Name);
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
                Organization currentOrganization = GetCurrentOrganization(context, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(currentOrganization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                SmsSentQueueItem smsSentQueueItem = context.GetSmsSentQueueItem(model.Identifier, true);
                context.Delete<SmsSentQueueItem>(smsSentQueueItem);
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
                Organization currentOrganization = GetCurrentOrganization(context, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(currentOrganization.OrganizationId, context))
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

                List<SmsSentQueueItem> smsSentQueueItemList = null;
                string smsCampaignIdString = searchParameters[searchParameters.Length - 1];
                Nullable<Guid> smsCampaignId = null;
                SmsCampaign smsCampaign = null;
                if (!string.IsNullOrEmpty(smsCampaignIdString))
                {
                    smsCampaignId = Guid.Parse(smsCampaignIdString);
                    smsCampaign = context.GetSmsCampaign(smsCampaignId.Value, true);
                }
                if (smsCampaign == null)
                {
                    model.ConfirmationMessage = string.Format("Delete all SMS Queue Items loaded for all Campaigns?");
                }
                else
                {
                    model.ParentId = smsCampaign.SmsCampaignId;
                    model.ParentCaption = smsCampaign.Name;
                    model.ConfirmationMessage = string.Format("Delete all SMS Queue Items currently loaded for Campaign '{0}'?", smsCampaign.Name);
                }
                smsSentQueueItemList = context.GetSmsSentQueueItemsByFilter(searchText, smsCampaignId);
                model.SearchText = searchText;
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
                Organization currentOrganinzation = GetCurrentOrganization(context, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(currentOrganinzation.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                if (model.ParentId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<SmsSentQueueItem>.GetPropertyName(p => p.SmsCampaignId, false)));
                }
                context.DeleteSmsSentQueueItemsByFilter(model.SearchText, model.ParentId);
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
                Organization currentOrganinzation = GetCurrentOrganization(context, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(currentOrganinzation.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<SmsSentQueueItem> smsSentQueueItemList = null;
                string smsCampaignIdString = searchParameters[searchParameters.Length - 1];
                Nullable<Guid> smsCampaignId = null;
                SmsCampaign smsCampaign = null;
                if (!string.IsNullOrEmpty(smsCampaignIdString))
                {
                    smsCampaignId = Guid.Parse(smsCampaignIdString);
                    smsCampaign = context.GetSmsCampaign(smsCampaignId.Value, true);
                }
                smsSentQueueItemList = context.GetSmsSentQueueItemsByFilter(searchText, smsCampaignId);

                EntityCache<Guid, SmsSentQueueItemCsv> cache = new EntityCache<Guid, SmsSentQueueItemCsv>();
                foreach (SmsSentQueueItem i in smsSentQueueItemList)
                {
                    SmsSentQueueItemCsv csv = new SmsSentQueueItemCsv();
                    csv.CopyPropertiesFromSmsSentQueueItem(i);
                    cache.Add(csv.SmsSentQueueItemId, csv);
                }
                return GetCsvFileResult<SmsSentQueueItemCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> smsSentQueueItemId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!smsSentQueueItemId.HasValue || smsSentQueueItemId.Value == Guid.Empty)
                {
                    return PartialView(EDIT_SMS_SENT_QUEUE_ITEM_DIALOG_PARTIAL_VIEW_NAME, new SmsSentQueueItemModel());
                }
                SmsSentQueueItem smsSentQueueItem = context.GetSmsSentQueueItem(smsSentQueueItemId.Value, true);
                SmsSentQueueItemModel model = new SmsSentQueueItemModel();
                model.CopyPropertiesFromSmsSentQueueItem(smsSentQueueItem);
                PartialViewResult result = PartialView(EDIT_SMS_SENT_QUEUE_ITEM_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(SmsSentQueueItemModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                Organization currentOrganization = GetCurrentOrganization(context, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(currentOrganization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SmsSentQueueItem smsSentQueueItem = context.GetSmsSentQueueItem(model.SmsSentQueueItemId, true);
                model.CopyPropertiesToSmsSentQueueItem(smsSentQueueItem);
                context.Save<SmsSentQueueItem>(smsSentQueueItem, false);
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
        public ActionResult RetrySend(Guid smsSentQueueItemId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                Organization currentOrganization = GetCurrentOrganization(context, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(currentOrganization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                SmsSentQueueItem smsSentQueueItem = context.GetSmsSentQueueItem(smsSentQueueItemId, true);
                smsSentQueueItem.FailedToSend = false;
                smsSentQueueItem.FailedToSendErrorMessage = null;
                context.Save<SmsSentQueueItem>(smsSentQueueItem, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult RetrySendAll(string searchParametersString)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                Organization currentOrganization = GetCurrentOrganization(context, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(currentOrganization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                string smsCampaignIdString = searchParameters[searchParameters.Length - 1];
                Nullable<Guid> smsCampaignId = null;
                SmsCampaign smsCampaign = null;
                if (!string.IsNullOrEmpty(smsCampaignIdString))
                {
                    smsCampaignId = Guid.Parse(smsCampaignIdString);
                    if (smsCampaignId == Guid.Empty)
                    {
                        throw new Exception(string.Format("{0} not specified correctly.", EntityReader<SmsCampaign>.GetPropertyName(p => p.SmsCampaignId, false)));
                    }
                    smsCampaign = context.GetSmsCampaign(smsCampaignId.Value, true);
                }
                if (!smsCampaignId.HasValue && IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                List<SmsSentQueueItem> smsSentQueueItems = context.GetSmsSentQueueItemsByFilter(searchText, smsCampaignId);
                foreach (SmsSentQueueItem s in smsSentQueueItems)
                {
                    if (s.FailedToSend)
                    {
                        SmsSentQueueItem smsSentQueueItem = context.GetSmsSentQueueItem(s.SmsSentQueueItemId, true);
                        smsSentQueueItem.FailedToSend = false;
                        smsSentQueueItem.FailedToSendErrorMessage = null;
                        context.Save<SmsSentQueueItem>(smsSentQueueItem, false);
                    }
                }
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        #endregion //Actions
    }
}
