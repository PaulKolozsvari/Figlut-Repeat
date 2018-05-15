namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Csv;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.ORM.Views;
    using Figlut.Spread.SMS;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class OrganizationSubscriptionController : SpreadController
    {
        #region Constants

        private const string ORGANIZATION_SUBSCRIPTION_GRID_PARTIAL_VIEW_NAME = "_OrganizationSubscriptionGrid";
        private const string EDIT_ORGANIZATION_SUBSCRIPTION_DIALOG_PARTIAL_VIEW_NAME = "_EditOrganizationSubscriptionDialog";
        private const string CREATE_ORGANIZATION_SUBSCRIPTION_DIALOG_PARTIAL_VIEW_NAME = "_CreateOrganizationSubscriptionDialog";
        private const string CREATE_SMS_CAMPAIGN_DIALOG_PARTIAL_VIEW_NAME = "_CreateSmsCampaignDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<OrganizationSubscriptionModel> GetOrganizationSubscriptionFilterModel(
            SpreadEntityContext context, 
            FilterModel<OrganizationSubscriptionModel> model,
            Guid organizationId)
        {
            if (organizationId == Guid.Empty)
            {
                base.SetViewBagErrorMessage(string.Format("{0} not specified.",
                    EntityReader<OrganizationSubscriptionModel>.GetPropertyName(p => p.OrganizationId, false)));
                return null;
            }
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<OrganizationSubscriptionView> subscriptions = context.GetOrganizationSubscriptionViewsByFilter(model.SearchText, organizationId, false);
            List<OrganizationSubscriptionModel> modelList = new List<OrganizationSubscriptionModel>();
            foreach (OrganizationSubscriptionView view in subscriptions)
            {
                OrganizationSubscriptionModel m = new OrganizationSubscriptionModel();
                m.CopyPropertiesFromOrganizationSubscriptionView(view);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllOrganizationSubscriptionsCount(organizationId);
            Organization organization = context.GetOrganization(organizationId, false);
            if (organization != null)
            {
                model.ParentId = organization.OrganizationId;
                model.ParentCaption = organization.Name;
            }
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index(Guid organizationId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(organizationId, context))
                {
                    return RedirectToHome();
                }
                FilterModel<OrganizationSubscriptionModel> model = GetOrganizationSubscriptionFilterModel(
                    context,
                    new FilterModel<OrganizationSubscriptionModel>(),
                    organizationId);
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
        public ActionResult Index(FilterModel<OrganizationSubscriptionModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(model.ParentId, context))
                {
                    return RedirectToHome();
                }
                FilterModel<OrganizationSubscriptionModel> resultModel = GetOrganizationSubscriptionFilterModel(context, model, model.ParentId);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(ORGANIZATION_SUBSCRIPTION_GRID_PARTIAL_VIEW_NAME, new FilterModel<OrganizationSubscriptionModel>());
                }
                return PartialView(ORGANIZATION_SUBSCRIPTION_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid subscriptionId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                Subscription subscription = context.GetSubscription(subscriptionId, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(subscription.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                context.Delete<Subscription>(subscription);
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
                Subscription subscription = context.GetSubscription(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (subscription != null)
                {
                    Subscriber subscriber = context.GetSubscriber(subscription.SubscriberId, true);
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete Subscription for {0} '{1}'?", typeof(Subscriber).Name, subscriber.CellPhoneNumber);
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
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                Subscription subscription = context.GetSubscription(model.Identifier, true);
                if(!CurrentUserHasAccessToOrganization(subscription.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                context.Delete<Subscription>(subscription);
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
                if (!Request.IsAuthenticated)
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

                string organizationIdString = searchParameters[searchParameters.Length - 1];
                Guid organizationId = Guid.Parse(organizationIdString);
                Organization organization = context.GetOrganization(organizationId, true);
                if (!CurrentUserHasAccessToOrganization(organization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                model.ParentId = organization.OrganizationId;
                model.ParentCaption = organization.Name;
                model.SearchText = searchText;
                model.ConfirmationMessage = string.Format("Delete all Subscriptions currently loaded for {0} '{1}'?", typeof(Organization).Name, organization.Name);
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
                if (model.ParentId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<OrganizationSubscriptionModel>.GetPropertyName(p => p.OrganizationId, false)));
                }
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(model.ParentId, context))
                {
                    return RedirectToHome();
                }
                context.DeleteOrganizationSubscriptionsByFilter(model.SearchText, model.ParentId);
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

                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);

                string organizationIdString = searchParameters[searchParameters.Length - 1];
                Guid organizationId = Guid.Parse(organizationIdString);
                Organization organization = context.GetOrganization(organizationId, true);
                if (organizationId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<OrganizationSubscriptionModel>.GetPropertyName(p => p.OrganizationId, false)));
                }
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(organization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<OrganizationSubscriptionView> subscriptionsList = context.GetOrganizationSubscriptionViewsByFilter(searchText, organizationId, false);
                EntityCache<Guid, OrganizationSubscriptionCsv> cache = new EntityCache<Guid, OrganizationSubscriptionCsv>();
                foreach (OrganizationSubscriptionView v in subscriptionsList)
                {
                    OrganizationSubscriptionCsv csv = new OrganizationSubscriptionCsv();
                    csv.CopyPropertiesFromOrganizationSubscriptionView(v);
                    cache.Add(csv.SubscriptionId, csv);
                }
                return GetCsvFileResult<OrganizationSubscriptionCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> subscriptionId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!subscriptionId.HasValue || subscriptionId == Guid.Empty)
                {
                    return PartialView(EDIT_ORGANIZATION_SUBSCRIPTION_DIALOG_PARTIAL_VIEW_NAME, new OrganizationSubscriptionModel());
                }
                OrganizationSubscriptionView subscription = context.GetOrganizationSubscriptionView(subscriptionId.Value, true);
                OrganizationSubscriptionModel model = new OrganizationSubscriptionModel();
                model.CopyPropertiesFromOrganizationSubscriptionView(subscription);
                PartialViewResult result = PartialView(EDIT_ORGANIZATION_SUBSCRIPTION_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(OrganizationSubscriptionModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                Subscription subscription = context.GetSubscription(model.SubscriptionId, true);
                model.CopyPropertiesToSubscription(subscription);
                context.Save<Subscription>(subscription, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult CreateDialog()
        {
            try
            {
                return PartialView(CREATE_ORGANIZATION_SUBSCRIPTION_DIALOG_PARTIAL_VIEW_NAME, new OrganizationSubscriptionModel() { Enabled = true });
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult CreateDialog(OrganizationSubscriptionModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValidSubscriberDetails(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                Subscriber subscriber = context.GetSubscriberByCellPhoneNumber(model.SubscriberCellPhoneNumber, false);
                if (subscriber == null)
                {
                    subscriber = new Subscriber()
                    {
                        SubscriberId = Guid.NewGuid(),
                        CellPhoneNumber = model.SubscriberCellPhoneNumber,
                        Name = !string.IsNullOrEmpty(model.CustomerFullName) ? model.CustomerFullName : null,
                        Enabled = true,
                        DateCreated = DateTime.Now
                    };
                    context.Save<Subscriber>(subscriber, false);
                }
                Organization currentOrganization = GetCurrentOrganization(context, true);
                if (context.IsSubscriberSubscribedToOrganization(currentOrganization.OrganizationId, subscriber.SubscriberId))
                {
                    return GetJsonResult(false, string.Format("{0} '{1}' is already subscribed to '{2}'.",
                        typeof(Subscriber).Name,
                        model.SubscriberCellPhoneNumber,
                        currentOrganization.Name));
                }
                model.SubscriptionId = Guid.NewGuid();
                model.OrganizationId = currentOrganization.OrganizationId;
                model.SubscriberId = subscriber.SubscriberId;
                model.DateCreated = DateTime.Now;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                Subscription subscription = new Subscription();
                model.CopyPropertiesToSubscription(subscription);
                context.Save<Subscription>(subscription, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult CreateSmsCampaignDialog(string searchParametersString)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (string.IsNullOrEmpty(searchParametersString))
                {
                    return PartialView(CREATE_SMS_CAMPAIGN_DIALOG_PARTIAL_VIEW_NAME, new CreateSmsCampaignModel());
                }
                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);

                string organizationIdString = searchParameters[searchParameters.Length - 1];
                Guid organizationId = Guid.Parse(organizationIdString);
                Organization organization = context.GetOrganization(organizationId, true);
                if (!CurrentUserHasAccessToOrganization(organization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                int maxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                CreateSmsCampaignModel model = new CreateSmsCampaignModel();
                model.OrganizationId = organization.OrganizationId;
                model.OrganizationName = organization.Name;
                model.SearchText = searchText;
                model.CampaignSmsCount = context.GetOrganizationSubscriptionViewsByFilterCount(searchText, organization.OrganizationId);
                model.MaxSmsSendMessageLength = maxSmsSendMessageLength;
                PartialViewResult result = PartialView(CREATE_SMS_CAMPAIGN_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult CreateSmsCampaignDialog(CreateSmsCampaignModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                model.SmsCampaignId = Guid.NewGuid();
                model.DateCreated = DateTime.Now;
                string errorMessage = null;
                int maxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                if (!model.IsValid(out errorMessage, maxSmsSendMessageLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                Organization currentOrganization = context.GetOrganization(model.OrganizationId, true);
                if (!CurrentUserHasAccessToOrganization(currentOrganization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                if ((currentOrganization.SmsCreditsBalance < model.CampaignSmsCount) && !currentOrganization.AllowSmsCreditsDebt)
                {
                    return GetJsonResult(false, string.Format("{0} '{1}' has insufficient SMS credits to send {2} messages.", 
                        typeof(Organization).Name, 
                        currentOrganization.Name,
                        model.CampaignSmsCount));
                }
                User user = GetCurrentUser(context);
                string errorMessageCreateCampaign = null;
                long enqueuedSmsCount = context.CreateSmsCampaign(
                    model.SmsCampaignId,
                    model.Name,
                    model.MessageContents,
                    model.OrganizationSelectedCode,
                    model.OrganizationId,
                    model.DateCreated,
                    model.SearchText,
                    (int)SmsProvider.Zoom,
                    user.UserId,
                    out errorMessageCreateCampaign);
                if (!string.IsNullOrEmpty(errorMessageCreateCampaign))
                {
                    return GetJsonResult(false, errorMessageCreateCampaign);
                }
                if (model.CampaignSmsCount != enqueuedSmsCount)
                {
                    GOC.Instance.Logger.LogMessage(new LogMessage(string.Format("Warning Error: {0} enqueued SMS' for {1} '{2}', but model property {3} = {4}. Mismatch error.",
                        enqueuedSmsCount,
                        typeof(Organization).Name,
                        currentOrganization.Name,
                        EntityReader<CreateSmsCampaignModel>.GetPropertyName(p => p.CampaignSmsCount, false),
                        model.CampaignSmsCount),
                        LogMessageType.Warning,
                        LoggingLevel.Normal));
                }
                long smsCredits = context.DeductSmsCreditsFromOrganization(currentOrganization.OrganizationId, enqueuedSmsCount).SmsCreditsBalance;
                GOC.Instance.Logger.LogMessage(new LogMessage(
                    string.Format("{0} '{1}' has enqueued {2} SMS'. Credits remaining: {3}.",
                    typeof(Organization).Name,
                    currentOrganization.Name,
                    enqueuedSmsCount,
                    smsCredits),
                    LogMessageType.SuccessAudit,
                    LoggingLevel.Normal));

                System.Threading.Thread.Sleep(3000);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        #endregion //Actions
    }
}
