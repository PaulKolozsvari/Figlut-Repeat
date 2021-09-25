namespace Figlut.Repeat.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Csv;
    using Figlut.Repeat.ORM.Views;
    using Figlut.Repeat.Web.Site.Configuration;
    using Figlut.Repeat.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class SubscriberSubscriptionController : RepeatController
    {
        #region Constants

        private const string SUBSCRIBER_SUBSCRIPTION_GRID_PARTIAL_VIEW_NAME = "_SubscriberSubscriptionGrid";
        private const string EDIT_SUBSCRIBER_SUBSCRIPTION_DIALOG_PARTIAL_VIEW_NAME = "_EditSubscriberSubscriptionDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<SubscriberSubscriptionModel> GetSubscriberSubscriptionFilterModel(
            RepeatEntityContext context,
            FilterModel<SubscriberSubscriptionModel> model,
            Guid subscriberId)
        {
            if (subscriberId == Guid.Empty)
            {
                base.SetViewBagErrorMessage(string.Format("{0} not specified.",
                    EntityReader<SubscriberSubscriptionModel>.GetPropertyName(p => p.SubscriberId, false)));
                return null;
            }
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<SubscriberSubscriptionView> subscriptions = context.GetSubscriberSubscriptionViewsByFilter(model.SearchText, subscriberId);
            List<SubscriberSubscriptionModel> modelList = new List<SubscriberSubscriptionModel>();
            foreach (SubscriberSubscriptionView view in subscriptions)
            {
                SubscriberSubscriptionModel m = new SubscriberSubscriptionModel();
                m.CopyPropertiesFromSubscriberSubscriptionView(view);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllSubscriberSubscriptionsCount(subscriberId);
            Subscriber subscriber = context.GetSubscriber(subscriberId, false);
            if (subscriber != null)
            {
                model.ParentId = subscriber.SubscriberId;
                model.ParentCaption = string.IsNullOrEmpty(subscriber.Name) ? 
                    subscriber.CellPhoneNumber : 
                    string.Format("{0} ({1})", subscriber.CellPhoneNumber, subscriber.Name);
            }
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index(Guid subscriberId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                FilterModel<SubscriberSubscriptionModel> model = GetSubscriberSubscriptionFilterModel(
                    context,
                    new FilterModel<SubscriberSubscriptionModel>(),
                    subscriberId);
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
        public ActionResult Index(FilterModel<SubscriberSubscriptionModel> model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                FilterModel<SubscriberSubscriptionModel> resultModel = GetSubscriberSubscriptionFilterModel(context, model, model.ParentId);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(SUBSCRIBER_SUBSCRIPTION_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsSentLogModel>());
                }
                return PartialView(SUBSCRIBER_SUBSCRIPTION_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid subscriptionId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                Subscription subscription = context.GetSubscription(subscriptionId, true);
                context.Delete<Subscription>(subscription, subscription.SubscriptionId);
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
                Subscription subscription = context.GetSubscription(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (subscription != null)
                {
                    Organization organization = context.GetOrganization(subscription.OrganizationId, true);
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete Subscription to {0} '{1}'?", typeof(Organization).Name, organization.Name);
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
                Subscription subscription = context.GetSubscription(model.Identifier, true);
                context.Delete<Subscription>(subscription, subscription.SubscriptionId);
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

                string subscriberIdString = searchParameters[searchParameters.Length - 1];
                Guid subscriberId = Guid.Parse(subscriberIdString);
                Subscriber subscriber = context.GetSubscriber(subscriberId, true);
                model.ParentId = subscriber.SubscriberId;
                model.ParentCaption = subscriber.CellPhoneNumber;

                model.SearchText = searchText;
                model.ConfirmationMessage = string.Format("Delete all Subscriptions currently loaded for {0} '{1}'?", typeof(Subscriber).Name, subscriber.CellPhoneNumber);
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
                if (model.ParentId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<SubscriberSubscriptionModel>.GetPropertyName(p => p.SubscriberId, false)));
                }
                context.DeleteSubscriberSubscriptionsByFilter(model.SearchText, model.ParentId);
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

                string subscriberIdString = searchParameters[searchParameters.Length - 1];
                Guid subscriberId = Guid.Parse(subscriberIdString);
                Subscriber subscriber = context.GetSubscriber(subscriberId, true);
                if (subscriberId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<SubscriberSubscriptionModel>.GetPropertyName(p => p.SubscriberId, false)));
                }
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<SubscriberSubscriptionView> subscriptionsList = context.GetSubscriberSubscriptionViewsByFilter(searchText, subscriberId);
                EntityCache<Guid, SubscriberSubscriptionCsv> cache = new EntityCache<Guid, SubscriberSubscriptionCsv>();
                foreach (SubscriberSubscriptionView view in subscriptionsList)
                {
                    SubscriberSubscriptionCsv csv = new SubscriberSubscriptionCsv();
                    csv.CopyPropertiesFromSubscriberSubscriptionView(view);
                    cache.Add(csv.SubscriptionId, csv);
                }
                return GetCsvFileResult<SubscriberSubscriptionCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> subscriptionId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!subscriptionId.HasValue)
                {
                    return PartialView(EDIT_SUBSCRIBER_SUBSCRIPTION_DIALOG_PARTIAL_VIEW_NAME, new SubscriberSubscriptionModel());
                }
                SubscriberSubscriptionView subscription = context.GetSubscriberSubscriptionView(subscriptionId.Value, true);
                SubscriberSubscriptionModel model = new SubscriberSubscriptionModel();
                model.CopyPropertiesFromSubscriberSubscriptionView(subscription);
                PartialViewResult result = PartialView(EDIT_SUBSCRIBER_SUBSCRIPTION_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(SubscriberSubscriptionModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                Subscription subscription = context.GetSubscription(model.SubscriptionId, true);
                model.CopyPropertiesToSubscription(subscription);
                context.Save<Subscription>(subscription, subscription.SubscriptionId, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult ViewSubscriberSubscriptions()
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                CaptureSubscriberQueryDetailsModel model = new CaptureSubscriberQueryDetailsModel();
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
        public ActionResult ViewSubscriberSubscriptions(CaptureSubscriberQueryDetailsModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                Subscriber subscriber = context.GetSubscriberByCellPhoneNumber(model.SubscriberCellPhoneNumber, false);
                if (subscriber == null)
                {
                    return GetJsonResult(false, string.Format("Your {0} is not listed with us.",
                        EntityReader<Subscriber>.GetPropertyName(p => p.CellPhoneNumber, true)));
                }
                long subscriptionsCount = context.GetAllSubscriberSubscriptionsCount(subscriber.SubscriberId);
                if (subscriptionsCount < 1)
                {
                    return GetJsonResult(false, string.Format("Your {0} is listed with us, but you do not have any subscriptions.",
                        EntityReader<Subscriber>.GetPropertyName(p => p.CellPhoneNumber, true)));
                }
                return Json(new { Success = true, ErrorMsg = string.Empty, SubscriberId = subscriber.SubscriberId });
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
