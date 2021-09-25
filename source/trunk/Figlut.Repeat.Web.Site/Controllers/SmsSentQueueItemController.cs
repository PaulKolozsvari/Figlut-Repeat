namespace Figlut.Repeat.Web.Site.Controllers
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Csv;
    using Figlut.Repeat.Web.Site.Configuration;
    using Figlut.Repeat.Web.Site.Models;

    #endregion //Using Directives

    public class SmsSentQueueItemController : RepeatController
    {
        #region Constants

        private const string SMS_SENT_QUEUE_ITEM_GRID_PARTIAL_VIEW_NAME = "_SmsSentQueueItemGrid";
        private const string EDIT_SMS_SENT_QUEUE_ITEM_DIALOG_PARTIAL_VIEW_NAME = "_EditSmsSentQueueItemDialog";

        #endregion //Constants

        #region Methods

        public FilterModel<SmsSentQueueItemModel> GetSmsSentQueueItemFilterModel(
            RepeatEntityContext context,
            FilterModel<SmsSentQueueItemModel> model,
            Nullable<Guid> smsCampaignId)
        {
            if (context == null)
            {
                context = RepeatEntityContext.Create();
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
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated ||
                    (!smsCampaignId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    (smsCampaignId.HasValue && !CurrentUserHasAccessToSmsCampaign(smsCampaignId.Value, context)))
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
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Index(FilterModel<SmsSentQueueItemModel> model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                Nullable<Guid> smsCampaignId = null;
                if (model.ParentId != Guid.Empty && model.ParentId != Guid.Empty)
                {
                    smsCampaignId = model.ParentId;
                }
                if (!Request.IsAuthenticated ||
                    (!smsCampaignId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    (smsCampaignId.HasValue && !CurrentUserHasAccessToSmsCampaign(smsCampaignId.Value, context)))
                {
                    return RedirectToHome();
                }
                FilterModel<SmsSentQueueItemModel> resultModel = GetSmsSentQueueItemFilterModel(context, model, smsCampaignId);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(SMS_SENT_QUEUE_ITEM_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsSentQueueItemModel>());
                }
                return PartialView(SMS_SENT_QUEUE_ITEM_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid smsSentQueueItemId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                SmsSentQueueItem smsSentQueueItem = context.GetSmsSentQueueItem(smsSentQueueItemId, true);
                Nullable<Guid> smsCampaignId = smsSentQueueItem.SmsCampaignId;
                if (!Request.IsAuthenticated ||
                    (!smsCampaignId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    (smsCampaignId.HasValue && !CurrentUserHasAccessToSmsCampaign(smsCampaignId.Value, context)))
                {
                    return RedirectToHome();
                }
                context.Delete<SmsSentQueueItem>(smsSentQueueItem, smsSentQueueItem.SmsSentQueueItemId);
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
                    Nullable<Guid> smsCampaignId = smsSentQueueItem.SmsCampaignId;
                    if ((!smsCampaignId.HasValue && !IsCurrentUserAdministrator(context)) ||
                        (smsCampaignId.HasValue && !CurrentUserHasAccessToSmsCampaign(smsCampaignId.Value, context)))
                    {
                        return RedirectToHome();
                    }
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
                SmsSentQueueItem smsSentQueueItem = context.GetSmsSentQueueItem(model.Identifier, true);
                Nullable<Guid> smsCampaignId = smsSentQueueItem.SmsCampaignId;
                if (!Request.IsAuthenticated ||
                    (!smsCampaignId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    (smsCampaignId.HasValue && !CurrentUserHasAccessToSmsCampaign(smsCampaignId.Value, context)))
                {
                    return RedirectToHome();
                }
                context.Delete<SmsSentQueueItem>(smsSentQueueItem, smsSentQueueItem.SmsSentQueueItemId);
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
                    if (smsCampaignId.Value == Guid.Empty)
                    {
                        smsCampaignId = null;
                    }
                    else
                    {
                        smsCampaign = context.GetSmsCampaign(smsCampaignId.Value, true);
                    }
                }
                if (!Request.IsAuthenticated ||
                    (!smsCampaignId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    (smsCampaignId.HasValue && !CurrentUserHasAccessToSmsCampaign(smsCampaignId.Value, context)))
                {
                    return RedirectToHome();
                }
                if (smsCampaign == null)
                {
                    model.ParentId = Guid.Empty;
                    model.ParentCaption = string.Empty;
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
                Nullable<Guid> smsCampaignId = null;
                if (model.ParentId != Guid.Empty)
                {
                    smsCampaignId = model.ParentId;
                }
                if (!Request.IsAuthenticated ||
                    (!smsCampaignId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    (smsCampaignId.HasValue && !CurrentUserHasAccessToSmsCampaign(smsCampaignId.Value, context)))
                {
                    return RedirectToHome();
                }
                context.DeleteSmsSentQueueItemsByFilter(model.SearchText, smsCampaignId);
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
                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<SmsSentQueueItem> smsSentQueueItemList = null;
                string smsCampaignIdString = searchParameters[searchParameters.Length - 1];
                Nullable<Guid> smsCampaignId = null;
                if (!string.IsNullOrEmpty(smsCampaignIdString))
                {
                    smsCampaignId = Guid.Parse(smsCampaignIdString);
                    if (smsCampaignId == Guid.Empty)
                    {
                        smsCampaignId = null;
                    }
                }
                if (!Request.IsAuthenticated ||
                    (!smsCampaignId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    (smsCampaignId.HasValue && !CurrentUserHasAccessToSmsCampaign(smsCampaignId.Value, context)))
                {
                    return RedirectToHome();
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
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> smsSentQueueItemId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!smsSentQueueItemId.HasValue || smsSentQueueItemId.Value == Guid.Empty)
                {
                    return PartialView(EDIT_SMS_SENT_QUEUE_ITEM_DIALOG_PARTIAL_VIEW_NAME, new SmsSentQueueItemModel());
                }
                SmsSentQueueItem smsSentQueueItem = context.GetSmsSentQueueItem(smsSentQueueItemId.Value, true);
                Nullable<Guid> smsCampaignId = smsSentQueueItem.SmsCampaignId;
                if (!Request.IsAuthenticated ||
                    (!smsCampaignId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    (smsCampaignId.HasValue && !CurrentUserHasAccessToSmsCampaign(smsCampaignId.Value, context)))
                {
                    return RedirectToHome();
                }
                SmsSentQueueItemModel model = new SmsSentQueueItemModel();
                model.CopyPropertiesFromSmsSentQueueItem(smsSentQueueItem);
                PartialViewResult result = PartialView(EDIT_SMS_SENT_QUEUE_ITEM_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(SmsSentQueueItemModel model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SmsSentQueueItem smsSentQueueItem = context.GetSmsSentQueueItem(model.SmsSentQueueItemId, true);
                Nullable<Guid> smsCampaignId = smsSentQueueItem.SmsCampaignId;
                if (!Request.IsAuthenticated ||
                    (!smsCampaignId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    (smsCampaignId.HasValue && !CurrentUserHasAccessToSmsCampaign(smsCampaignId.Value, context)))
                {
                    return RedirectToHome();
                }
                model.CopyPropertiesToSmsSentQueueItem(smsSentQueueItem);
                context.Save<SmsSentQueueItem>(smsSentQueueItem, smsSentQueueItem.SmsSentQueueItemId, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult RetrySend(Guid smsSentQueueItemId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                SmsSentQueueItem smsSentQueueItem = context.GetSmsSentQueueItem(smsSentQueueItemId, true);
                Nullable<Guid> smsCampaignId = smsSentQueueItem.SmsCampaignId;
                if (!Request.IsAuthenticated ||
                    (!smsCampaignId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    (smsCampaignId.HasValue && !CurrentUserHasAccessToSmsCampaign(smsCampaignId.Value, context)))
                {
                    return RedirectToHome();
                }
                smsSentQueueItem.FailedToSend = false;
                smsSentQueueItem.FailedToSendErrorMessage = null;
                context.Save<SmsSentQueueItem>(smsSentQueueItem, smsSentQueueItem.SmsSentQueueItemId, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult RetrySendAll(string searchParametersString)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                string smsCampaignIdString = searchParameters[searchParameters.Length - 1];
                Nullable<Guid> smsCampaignId = null;
                if (!string.IsNullOrEmpty(smsCampaignIdString))
                {
                    smsCampaignId = Guid.Parse(smsCampaignIdString);
                    if (smsCampaignId == Guid.Empty)
                    {
                        smsCampaignId = null;
                    }
                }
                if (!Request.IsAuthenticated ||
                    (!smsCampaignId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    (smsCampaignId.HasValue && !CurrentUserHasAccessToSmsCampaign(smsCampaignId.Value, context)))
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
                        context.Save<SmsSentQueueItem>(smsSentQueueItem, smsSentQueueItem.SmsSentQueueItemId, false);
                    }
                }
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        #endregion //Actions
    }
}
