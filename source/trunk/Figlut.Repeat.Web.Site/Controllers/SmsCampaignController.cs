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

    public class SmsCampaignController : RepeatController
    {
        #region Constants

        private const string SMS_CAMPAIGN_GRID_PARTIAL_VIEW_NAME = "_SmsCampaignGrid";
        private const string EDIT_SMS_CAMPAIGN_DIALOG_PARTIAL_VIEW_NAME = "_EditSmsCampaignDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<SmsCampaignModel> GetSmsCampaignFilterModel(
            RepeatEntityContext context,
            FilterModel<SmsCampaignModel> model,
            Nullable<Guid> organizationId)
        {
            if (organizationId == Guid.Empty)
            {
                base.SetViewBagErrorMessage(string.Format("{0} not specified.",
                    EntityReader<SmsCampaignModel>.GetPropertyName(p => p.OrganizationId, false)));
                return null;
            }
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<SmsCampaignView> smsCampaigns = context.GetSmsCampaignViewsByFilter(model.SearchText, organizationId);
            List<SmsCampaignModel> modelList = new List<SmsCampaignModel>();
            foreach (SmsCampaignView v in smsCampaigns)
            {
                SmsCampaignModel m = new SmsCampaignModel();
                m.CopyPropertiesFromSmsCampaignView(v);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllSmsCampaignCount(organizationId);
            if (organizationId.HasValue)
            {
                Organization organization = context.GetOrganization(organizationId.Value, false);
                if (organization != null)
                {
                    model.ParentId = organization.OrganizationId;
                    model.ParentCaption = organization.Name;
                }
            }
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index(Guid organizationId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(organizationId, context))
                {
                    return RedirectToHome();
                }
                FilterModel<SmsCampaignModel> model = GetSmsCampaignFilterModel(
                    context,
                    new FilterModel<SmsCampaignModel>(),
                    organizationId);
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
        public ActionResult Index(FilterModel<SmsCampaignModel> model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(model.ParentId, context))
                {
                    return RedirectToHome();
                }
                FilterModel<SmsCampaignModel> resultModel = model.ParentId != Guid.Empty ?
                    GetSmsCampaignFilterModel(context, model, model.ParentId) :
                    GetSmsCampaignFilterModel(context, model, null);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(SMS_CAMPAIGN_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsCampaignModel>());
                }
                return PartialView(SMS_CAMPAIGN_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid smsCampaignId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                SmsCampaign smsCampaign = context.GetSmsCampaign(smsCampaignId, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(smsCampaign.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                context.Delete<SmsCampaign>(smsCampaign, smsCampaign.Name);
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
                SmsCampaign smsCampaign = context.GetSmsCampaign(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (smsCampaign != null)
                {
                    Organization organization = context.GetOrganization(smsCampaign.OrganizationId, true);
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete SMS Campaign '{0}' for {1} '{2}'?", smsCampaign.Name, typeof(Organization).Name, organization.Name);
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
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                SmsCampaign smsCampaign = context.GetSmsCampaign(model.Identifier, true);
                if (!CurrentUserHasAccessToOrganization(smsCampaign.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                context.Delete<SmsCampaign>(smsCampaign, smsCampaign);
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
                model.ConfirmationMessage = string.Format("Delete all SMS Campaigns currently loaded for {0} '{1}'?", typeof(Organization).Name, organization.Name);
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
                if (model.ParentId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<SmsCampaignModel>.GetPropertyName(p => p.OrganizationId, false)));
                }
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(model.ParentId, context))
                {
                    return RedirectToHome();
                }
                context.DeleteSmsCampaignsByFilter(model.SearchText, model.ParentId);
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

                string organizationIdString = searchParameters[searchParameters.Length - 1];
                Guid organizationId = Guid.Parse(organizationIdString);
                if (organizationId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<SmsCampaignModel>.GetPropertyName(p => p.OrganizationId, false)));
                }
                Organization organization = context.GetOrganization(organizationId, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(organization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<SmsCampaignView> smsCampaignList = context.GetSmsCampaignViewsByFilter(searchText, organizationId);
                EntityCache<Guid, SmsCampaignCsv> cache = new EntityCache<Guid, SmsCampaignCsv>();
                foreach (SmsCampaignView v in smsCampaignList)
                {
                    SmsCampaignCsv csv = new SmsCampaignCsv();
                    csv.CopyPropertiesFromSmsCampaignView(v);
                    cache.Add(csv.SmsCampaignId, csv);
                }
                return GetCsvFileResult<SmsCampaignCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> smsCampaignId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!smsCampaignId.HasValue)
                {
                    return PartialView(EDIT_SMS_CAMPAIGN_DIALOG_PARTIAL_VIEW_NAME, new SmsCampaignModel());
                }
                SmsCampaignView smsCampaignView = context.GetSmsCampaignView(smsCampaignId.Value, true);
                SmsCampaignModel model = new SmsCampaignModel();
                model.CopyPropertiesFromSmsCampaignView(smsCampaignView);
                PartialViewResult result = PartialView(EDIT_SMS_CAMPAIGN_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(SmsCampaignModel model)
        {
            try
            {
                string errorMessage = null;
                int maxSmsSendMessageLength = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                if (!model.IsValid(out errorMessage, maxSmsSendMessageLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                SmsCampaign smsCampaign = context.GetSmsCampaign(model.SmsCampaignId, true);
                model.CopyPropertiesToSmsCampaign(smsCampaign);
                context.Save<SmsCampaign>(smsCampaign, smsCampaign.Name, false);
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