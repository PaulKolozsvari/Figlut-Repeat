namespace Figlut.Repeat.Web.Site.Controllers
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Figlut.Repeat.ORM.Csv;
    using Figlut.Repeat.Web.Site.Configuration;
    using Figlut.Server.Toolkit.Utilities;
    using System.Web.Mvc;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.Web.Site.Models;
    using Figlut.Server.Toolkit.Data;

    #endregion //Using Directives

    public class OrganizationLeadController : RepeatController
    {
        #region Constants

        private const string ORGANIZATION_LEAD_GRID_PARTIAL_VIEW_NAME = "_OrganizationLeadGrid";
        private const string EDIT_ORGANIZATION_LEAD_DIALOG_PARTIAL_VIEW_NAME = "_EditOrganizationLeadDialog";
        private const string CREATE_ORGANIZATION_LEAD_DIALOG_PARTIAL_VIEW_NAME = "_CreateOrganizationLeadDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<OrganizationLeadModel> GetOrganizationLeadFilterModel(
            RepeatEntityContext context,
            FilterModel<OrganizationLeadModel> model,
            Guid organizationId)
        {
            if (organizationId == Guid.Empty)
            {
                base.SetViewBagErrorMessage(string.Format("{0} not specified.",
                    EntityReader<OrganizationLeadModel>.GetPropertyName(p => p.OrganizationId, false)));
                return null;
            }
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<OrganizationLead> leads = context.GetOrganizationLeadsByFilter(model.SearchText, organizationId);
            List<OrganizationLeadModel> modelList = new List<OrganizationLeadModel>();
            foreach (OrganizationLead view in leads)
            {
                OrganizationLeadModel m = new OrganizationLeadModel();
                m.CopyPropertiesFromOrganizationLead(view);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllOrganizationLeadsCount(organizationId);
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
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(organizationId, context))
                {
                    return RedirectToHome();
                }
                FilterModel<OrganizationLeadModel> model = GetOrganizationLeadFilterModel(
                    context,
                    new FilterModel<OrganizationLeadModel>(),
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
        public ActionResult Index(FilterModel<OrganizationLeadModel> model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(model.ParentId, context))
                {
                    return RedirectToHome();
                }
                FilterModel<OrganizationLeadModel> resultModel = GetOrganizationLeadFilterModel(context, model, model.ParentId);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(ORGANIZATION_LEAD_GRID_PARTIAL_VIEW_NAME, new FilterModel<OrganizationLeadModel>());
                }
                return PartialView(ORGANIZATION_LEAD_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid organizationLeadId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                OrganizationLead lead = context.GetOrganizationLead(organizationLeadId, true);
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(lead.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                context.Delete<OrganizationLead>(lead, lead.OrganizationLeadId);
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
                OrganizationLead lead = context.GetOrganizationLead(identifier, true);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (lead != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete {0} '{1}'?", typeof(OrganizationLead).Name, lead.Name);
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
                OrganizationLead lead = context.GetOrganizationLead(model.Identifier, true);
                if (!CurrentUserHasAccessToOrganization(lead.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                context.Delete<OrganizationLead>(lead, lead.OrganizationLeadId);
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
                model.ConfirmationMessage = string.Format("Delete all Leads currently loaded for {0} '{1}'?", typeof(Organization).Name, organization.Name);
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
                        EntityReader<OrganizationLeadModel>.GetPropertyName(p => p.OrganizationId, false)));
                }
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(model.ParentId, context))
                {
                    return RedirectToHome();
                }
                context.DeleteOrganizationLeadsByFilter(model.SearchText, model.ParentId);
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
                Organization organization = context.GetOrganization(organizationId, true);
                if (organizationId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<OrganizationLeadModel>.GetPropertyName(p => p.OrganizationId, false)));
                }
                if (!Request.IsAuthenticated || !CurrentUserHasAccessToOrganization(organization.OrganizationId, context))
                {
                    return RedirectToHome();
                }
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<OrganizationLead> leads = context.GetOrganizationLeadsByFilter(searchText, organizationId);
                EntityCache<Guid, OrganizationLeadCsv> cache = new EntityCache<Guid, OrganizationLeadCsv>();
                foreach (OrganizationLead v in leads)
                {
                    OrganizationLeadCsv csv = new OrganizationLeadCsv();
                    csv.CopyPropertiesFromOrganizationLead(v);
                    cache.Add(csv.OrganizationLeadId, csv);
                }
                return GetCsvFileResult<OrganizationLeadCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> organizationLeadId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!organizationLeadId.HasValue || organizationLeadId == Guid.Empty)
                {
                    return PartialView(EDIT_ORGANIZATION_LEAD_DIALOG_PARTIAL_VIEW_NAME, new OrganizationLeadModel());
                }
                OrganizationLead lead = context.GetOrganizationLead(organizationLeadId.Value, true);
                OrganizationLeadModel model = new OrganizationLeadModel();
                model.CopyPropertiesFromOrganizationLead(lead);
                PartialViewResult result = PartialView(EDIT_ORGANIZATION_LEAD_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(OrganizationLeadModel model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                OrganizationLead lead = context.GetOrganizationLead(model.OrganizationLeadId, true);
                Organization organization = context.GetOrganization(model.OrganizationId, true);
                model.CopyPropertiesToOrganizationLead(lead);
                context.Save<OrganizationLead>(lead, lead.OrganizationLeadId, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult CreateDialog(Nullable<Guid> organizationId)
        {
            try
            {
                if (!organizationId.HasValue || organizationId.Value == Guid.Empty)
                {
                    return PartialView(CREATE_ORGANIZATION_LEAD_DIALOG_PARTIAL_VIEW_NAME, new OrganizationLeadModel());
                }
                return PartialView(CREATE_ORGANIZATION_LEAD_DIALOG_PARTIAL_VIEW_NAME, new OrganizationLeadModel()
                {
                    OrganizationId = organizationId.Value,
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult CreateDialog(OrganizationLeadModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (model.OrganizationId == Guid.Empty)
                {
                    return GetJsonResult(false, string.Format("{0} not specified.", EntityReader<OrganizationSubscriptionModel>.GetPropertyName(p => p.OrganizationId, false)));
                }
                Organization organization = context.GetOrganization(model.OrganizationId, true);
                model.OrganizationLeadId = Guid.NewGuid();
                model.DateCreated = DateTime.Now;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                OrganizationLead lead = new OrganizationLead();
                model.CopyPropertiesToOrganizationLead(lead);
                context.Save<OrganizationLead>(lead, lead.OrganizationLeadId, false);
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