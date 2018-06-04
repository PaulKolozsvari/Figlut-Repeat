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
        private const string CREATE_ORGANIZATION_DIALOG_PARTIAL_VIEW_NAME = "_CreateOrganizationDialog";
        private const string EDIT_ORGANIZATION_DIALOG_PARTIAL_VIEW_NAME = "_EditOrganizationDialog";
        private const string EDIT_ORGANIZATION_PROFILE_DIALOG_PARTIAL_VIEW_NAME = "_EditOrganizationProfileDialog";

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
                if (o.AccountManagerUserId.HasValue)
                {
                    User accountManager = context.GetUser(o.AccountManagerUserId.Value, false);
                    if (accountManager != null)
                    {
                        m.AccountManagerUserName = accountManager.UserName;
                        m.AccountManagerEmailAddress = accountManager.EmailAddress;
                    }
                }
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllOrganizationCount();
            return model;
        }

        private void RefreshAccountManagersList(SpreadEntityContext context, User defaultUser)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            List<SelectListItem> usersList = new List<SelectListItem>();
            List<User> users = context.GetUsersOfRole(UserRole.AccountManager);
            usersList.Add(new SelectListItem() //Add an empty entry i.e. Account Manager user not selected.
            {
                Text = string.Empty,
                Value = string.Empty,
                Selected = true
            });
            foreach (User u in users)
            {
                usersList.Add(new SelectListItem()
                {
                    Text = u.UserName,
                    Value = u.UserId.ToString(),
                    Selected = false,
                });
            }
            ViewBag.AccountManagersList = usersList;
            if (defaultUser != null)
            {
                ViewBag.AccountManagerUserId = defaultUser.UserId;
            }
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

        public ActionResult EditProfileDialog()
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
                if (organization.AccountManagerUserId.HasValue)
                {
                    User accountManager = context.GetUser(organization.AccountManagerUserId.Value, true);
                    model.AccountManagerUserName = accountManager.UserName;
                    model.AccountManagerEmailAddress = accountManager.EmailAddress;
                    model.AccountManagerCellPhoneNumber = accountManager.CellPhoneNumber;
                }
                PartialViewResult result = PartialView(EDIT_ORGANIZATION_PROFILE_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult EditProfileDialog(OrganizationProfileModel model)
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

        public ActionResult CreateDialog()
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                RefreshAccountManagersList(context, null);
                int organizationIdentifierMaxLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.OrganizationIdentifierMaxLength].SettingValue);
                return PartialView(CREATE_ORGANIZATION_DIALOG_PARTIAL_VIEW_NAME, new OrganizationModel()
                {
                    OrganizationSubscriptionEnabled = true,
                    BillingDayOfTheMonth = 1,
                    AutomaticallySendDailyScheduleEntriesSms = false,
                    DailyScheduleEntriesEmailNotificationTime = new TimeSpan(8, 0, 0),
                    OrganizationIdentifierMaxLength = organizationIdentifierMaxLength,
                    IsMondayWorkDay = true,
                    IsTuesdayWorkDay = true,
                    IsWednesdayWorkDay = true,
                    IsThursdayWorkDay = true,
                    IsFridayWorkDay = true,
                    IsSaturdayWorkDay = false,
                    IsSundayWorkDay = false
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(true);
            }
        }

        [HttpPost]
        public ActionResult CreateDialog(OrganizationModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                RefreshAccountManagersList(context, null);
                int organizationIdentifierMaxLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.OrganizationIdentifierMaxLength].SettingValue);
                model.OrganizationId = Guid.NewGuid();
                model.DateCreated = DateTime.Now;
                string errorMessage = null;
                if (!model.IsValid(out errorMessage, organizationIdentifierMaxLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                Organization organization = new Organization();
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
                User accountManager = null;
                RefreshAccountManagersList(context, accountManager);
                if (!organizationId.HasValue)
                {
                    return PartialView(EDIT_ORGANIZATION_DIALOG_PARTIAL_VIEW_NAME, new OrganizationModel());
                }
                Organization organization = context.GetOrganization(organizationId.Value, true);
                string currencySymbol = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.DefaultCurrencySymbol].SettingValue;
                int organizationIdentifierMaxLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.OrganizationIdentifierMaxLength].SettingValue);
                OrganizationModel model = new OrganizationModel();
                model.CopyPropertiesFromOrganization(organization, currencySymbol);
                if (organization.AccountManagerUserId.HasValue)
                {
                    accountManager = context.GetUser(organization.AccountManagerUserId.Value, true);
                    model.AccountManagerUserName = accountManager.UserName;
                    model.AccountManagerEmailAddress = accountManager.EmailAddress;
                    model.AccountManagerCellPhoneNumber = accountManager.CellPhoneNumber;
                    RefreshAccountManagersList(context, accountManager);
                }
                model.OrganizationIdentifierMaxLength = organizationIdentifierMaxLength;
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
                SpreadEntityContext context = SpreadEntityContext.Create();
                User accountManager = null;
                RefreshAccountManagersList(context, accountManager);
                if (model.AccountManagerUserId.HasValue)
                {
                    accountManager = context.GetUser(model.AccountManagerUserId.Value, true);
                    model.AccountManagerUserName = accountManager.UserName;
                    model.AccountManagerEmailAddress = accountManager.EmailAddress;
                    RefreshAccountManagersList(context, accountManager);
                }
                int organizationIdentifierMaxLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.OrganizationIdentifierMaxLength].SettingValue);
                string errorMessage = null;
                if (!model.IsValid(out errorMessage, organizationIdentifierMaxLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
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

        /// <summary>
        /// Uses a post because of this security exception: System.InvalidOperationException: This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.
        /// https://stackoverflow.com/questions/21452925/what-sensitive-information-could-be-disclosed-when-setting-jsonrequestbehavior
        /// https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCurrentOrganizationSmsCreditsBalance()
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                Organization currentOrganization = GetCurrentOrganization(context, true);
                return GetJsonResult(true, string.Format("SMS Credits: {0}", currentOrganization.SmsCreditsBalance));
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        #endregion //Actions
    }
}