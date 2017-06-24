namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Csv;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class OrganizationController : SpreadController
    {
        #region Constants

        private const string ORGANIZATION_GRID_PARTIAL_VIEW_NAME = "_OrganizationGrid";
        private const string EDIT_ORGANIZATION_DIALOG_PARTIAL_VIEW_NAME = "_EditOrganizationDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<OrganizationModel> GetOrganizationFilterModel(SpreadEntityContext context, FilterModel<OrganizationModel> model)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            string currencySymbol = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.DefaultCurrencySymbol].SettingValue;
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<Organization> organizationList = context.GetOrganizationsByFilter(model.SearchText);
            List<OrganizationModel> modelList = new List<OrganizationModel>();
            foreach (Organization o in organizationList)
            {
                OrganizationModel m = new OrganizationModel();
                m.CopyPropertiesFromOrganization(o, currencySymbol);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllOrganizationCount();
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
                FilterModel<OrganizationModel> model = GetOrganizationFilterModel(context, new FilterModel<OrganizationModel>());
                if (model == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return View(new FilterModel<OrganizationModel>());
                }
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
        public ActionResult Index(FilterModel<OrganizationModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                FilterModel<OrganizationModel> resultModel = GetOrganizationFilterModel(context, model);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(ORGANIZATION_GRID_PARTIAL_VIEW_NAME, new FilterModel<OrganizationModel>());
                }
                return PartialView(ORGANIZATION_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid organizationId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                Organization organization = context.GetOrganization(organizationId, true);
                context.Delete<Organization>(organization);
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
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                Organization organization = context.GetOrganization(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (organization != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete {0} '{1}'?", 
                        typeof(Organization).Name,
                        !string.IsNullOrEmpty(organization.Name) ? organization.Name : organization.Identifier);
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
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                Organization organization = context.GetOrganization(model.Identifier, true);
                context.Delete<Organization>(organization);
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
                model.ConfirmationMessage = "Delete all Organizations currently loaded?";
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
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                context.DeleteOrganizationsByFilter(model.SearchText);
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
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<Organization> organizationList = context.GetOrganizationsByFilter(searchText);
                EntityCache<Guid, OrganizationCsv> cache = new EntityCache<Guid, OrganizationCsv>();
                foreach (Organization o in organizationList)
                {
                    OrganizationCsv csv = new OrganizationCsv();
                    csv.CopyPropertiesFromOrganization(o);
                    cache.Add(csv.OrganizationId, csv);
                }
                return GetCsvFileResult<OrganizationCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditProfile()
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                User currentUser = GetCurrentUser(context);
                if (!currentUser.OrganizationId.HasValue)
                {
                    return RedirectToError(string.Format("You are not assigned to an Organization."));
                }
                Organization organization = context.GetOrganization(currentUser.OrganizationId.Value, true);
                OrganizationProfileModel model = new OrganizationProfileModel();
                model.CopyPropertiesFromOrganization(organization);
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
        public ActionResult EditProfile(OrganizationProfileModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                Organization organization = context.GetOrganization(model.OrganizationId, true);
                bool organizationIdentifierHasChanged = organization.Identifier != model.OrganizationIdentifier;
                if (organizationIdentifierHasChanged)
                {
                    Organization original = context.GetOrganizationByIdentifier(model.OrganizationIdentifier, false);
                    if (original != null)
                    {
                        return GetJsonResult(false, string.Format("An {0} with the {1} of '{2}' already exists.",
                            typeof(Organization).Name,
                            EntityReader<Organization>.GetPropertyName(p => p.Identifier, false),
                            model.OrganizationIdentifier));
                    }
                }
                model.CopyPropertiesToOrganization(organization);
                context.Save<Organization>(organization, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> organizationId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                if (!organizationId.HasValue)
                {
                    return PartialView(EDIT_ORGANIZATION_DIALOG_PARTIAL_VIEW_NAME, new OrganizationModel());
                }
                Organization organization = context.GetOrganization(organizationId.Value, true);
                string currencySymbol = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.DefaultCurrencySymbol].SettingValue;
                OrganizationModel model = new OrganizationModel();
                model.CopyPropertiesFromOrganization(organization, currencySymbol);
                PartialViewResult result = PartialView(EDIT_ORGANIZATION_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(OrganizationModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                Organization organization = context.GetOrganization(model.OrganizationId, true);
                bool organizationIdentifierHasChanged = organization.Identifier != model.Identifier;
                if (organizationIdentifierHasChanged)
                {
                    Organization original = context.GetOrganizationByIdentifier(model.Identifier, false);
                    if (original != null)
                    {
                        return GetJsonResult(false, string.Format("An {0} with the {1} of '{2}' already exists.",
                            typeof(Organization).Name,
                            EntityReader<Organization>.GetPropertyName(p => p.Identifier, false),
                            model.Identifier));
                    }
                }
                model.CopyPropertiesToOrganization(organization);
                context.Save<Organization>(organization, false);
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