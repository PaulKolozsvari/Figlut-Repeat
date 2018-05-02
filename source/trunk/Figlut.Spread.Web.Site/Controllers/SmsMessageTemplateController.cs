namespace Figlut.Spread.Web.Site.Controllers
{
    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Csv;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.ORM.Views;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class SmsMessageTemplateController : SpreadController
    {
        #region Constants

        private const string SMS_MESSAGE_TEMPLATE_GRID_PARTIAL_VIEW_NAME = "_SmsMessageTemplateGrid";
        private const string EDIT_SMS_MESSAGE_TEMPLATE_PARTIAL_VIEW_NAME = "_EditSmsMessageTemplateDialog";
        private const string CREATE_SMS_MESSAGE_TEMPLATE_PARTIAL_VIEW_NAME = "_InsertSmsMessageTemplateDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<SmsMessageTemplateModel> GetSmsMessageTemplateFilterModel(
            SpreadEntityContext context, 
            FilterModel<SmsMessageTemplateModel> model,
            Nullable<Guid> organizationId)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<SmsMessageTemplateView> smsMessageTemplateViewList = context.GetSmsMessageTemplateViewsByFilter(model.SearchText, organizationId);
            List<SmsMessageTemplateModel> modelList = new List<SmsMessageTemplateModel>();
            foreach (SmsMessageTemplateView v in smsMessageTemplateViewList)
            {
                SmsMessageTemplateModel m = new SmsMessageTemplateModel();
                m.CopyPropertiesFromSmsMessageTemplateView(v);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllSmsMessageTemplateCount();
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
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(organizationId, context))
                {
                    return RedirectToHome();
                }
                FilterModel<SmsMessageTemplateModel> model = GetSmsMessageTemplateFilterModel(
                    context, 
                    new FilterModel<SmsMessageTemplateModel>(), 
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
        public ActionResult Index(FilterModel<SmsMessageTemplateModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(model.ParentId, context))
                {
                    return RedirectToHome();
                }
                FilterModel<SmsMessageTemplateModel> resultModel = model.ParentId != Guid.Empty ?
                    GetSmsMessageTemplateFilterModel(context, model, model.ParentId) :
                    GetSmsMessageTemplateFilterModel(context, model, null);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(SMS_MESSAGE_TEMPLATE_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsCampaignModel>());
                }
                return PartialView(SMS_MESSAGE_TEMPLATE_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid smsMessageTemplateId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                SmsMessageTemplate smsMessageTemplate = context.GetSmsMessageTemplate(smsMessageTemplateId, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(smsMessageTemplate.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                context.Delete<SmsMessageTemplate>(smsMessageTemplate);
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
                SmsMessageTemplate smsMessageTemplate = context.GetSmsMessageTemplate(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (smsMessageTemplate != null)
                {
                    Organization organization = context.GetOrganization(smsMessageTemplate.OrganizationId, true);
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete SMS Message Template '{0}' for {1} '{2}'?", smsMessageTemplate.Message, typeof(Organization).Name, organization.Name);
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
                SmsMessageTemplate smsMessageTemplate = context.GetSmsMessageTemplate(model.Identifier, true);
                if (!CurrentUserHasAccessToOrganization(smsMessageTemplate.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                context.Delete<SmsMessageTemplate>(smsMessageTemplate);
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
                model.ConfirmationMessage = string.Format("Delete all SMS Message Templates currently loaded for {0} '{1}'?", typeof(Organization).Name, organization.Name);
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
                        EntityReader<SmsMessageTemplate>.GetPropertyName(p => p.OrganizationId, false)));
                }
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(model.ParentId, context))
                {
                    return RedirectToHome();
                }
                context.DeleteSmsMessageTemplatesByFilter(model.SearchText, model.ParentId);
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
                        EntityReader<SmsMessageTemplate>.GetPropertyName(p => p.OrganizationId, false)));
                }
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(organization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<SmsMessageTemplateView> smsMessageTemplateList = context.GetSmsMessageTemplateViewsByFilter(searchText, organizationId);
                EntityCache<Guid, SmsMessageTemplateCsv> cache = new EntityCache<Guid, SmsMessageTemplateCsv>();
                foreach (SmsMessageTemplateView v in smsMessageTemplateList)
                {
                    SmsMessageTemplateCsv csv = new SmsMessageTemplateCsv();
                    csv.CopyPropertiesFromSmsMessageTemplateView(v);
                    cache.Add(csv.SmsMessageTemplateId, csv);
                }
                return GetCsvFileResult<SmsMessageTemplateCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> smsMessageTemplateId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!smsMessageTemplateId.HasValue)
                {
                    return PartialView(EDIT_SMS_MESSAGE_TEMPLATE_PARTIAL_VIEW_NAME, new SmsMessageTemplateModel());
                }
                SmsMessageTemplateView smsMessageTemplateView = context.GetSmsMessageTemplateView(smsMessageTemplateId.Value, true);
                SmsMessageTemplateModel model = new SmsMessageTemplateModel();
                model.CopyPropertiesFromSmsMessageTemplateView(smsMessageTemplateView);
                PartialViewResult result = PartialView(EDIT_SMS_MESSAGE_TEMPLATE_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(SmsMessageTemplateModel model)
        {
            try
            {
                string errorMessage = null;
                int maxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                if (!model.IsValid(out errorMessage, maxSmsSendMessageLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                SmsMessageTemplate smsMessageTemplate = context.GetSmsMessageTemplate(model.SmsMessageTemplateId, true);
                model.CopyPropertiesToSmsMessageTemplate(smsMessageTemplate);
                context.Save<SmsMessageTemplate>(smsMessageTemplate, false);
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
                return PartialView(CREATE_SMS_MESSAGE_TEMPLATE_PARTIAL_VIEW_NAME, new SmsMessageTemplateModel());
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(true);
            }
        }

        [HttpPost]
        public ActionResult CreateDialog(SmsMessageTemplateModel model)
        {
            try
            {
                string errorMessage = null;
                int maxSmsSendMessageLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.MaxSmsSendMessageLength].SettingValue);
                if (!model.IsValid(out errorMessage, maxSmsSendMessageLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                model.SmsMessageTemplateId = Guid.NewGuid();
                model.DateCreated = DateTime.Now;
                SmsMessageTemplate smsMessageTemplate = new SmsMessageTemplate();
                model.CopyPropertiesToSmsMessageTemplate(smsMessageTemplate);
                context.Save<SmsMessageTemplate>(smsMessageTemplate, false);
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
