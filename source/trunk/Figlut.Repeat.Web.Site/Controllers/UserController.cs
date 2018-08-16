﻿namespace Figlut.Repeat.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.Email;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Csv;
    using Figlut.Repeat.ORM.Helpers;
    using Figlut.Repeat.Web.Site.Configuration;
    using Figlut.Repeat.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    #endregion //Using Directives

    public class UserController : RepeatController
    {
        #region Constants

        private const string USER_GRID_PARTIAL_VIEW_NAME = "_UserGrid";
        private const string LOGIN_DIALOG_PARTIAL_VIEW_NAME = "_LoginDialog";
        private const string RESET_PASSWORD_DIALOG_PARTIAL_VIEW_NAME = "_ResetPasswordDialog";
        private const string EDIT_USER_DIALOG_PARTIAL_VIEW_NAME = "_EditUserDialog";
        private const string CREATE_USER_DIALOG_PARTIAL_VIEW_NAME = "_CreateUserDialog";
        private const string EDIT_USER_PROFILE_DIALOG_PARTIAL_VIEW_NAME = "_EditUserProfileDialog";
        private const string EDIT_USER_PASSWORD_DIALOG_PARTIAL_VIEW_NAME = "_EditUserPasswordDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<UserModel> GetUserFilterModel(
            RepeatEntityContext context, 
            FilterModel<UserModel> model, 
            Nullable<Guid> organizationId)
        {
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<User> userList = context.GetUsersByFilter(model.SearchText, organizationId);
            List<UserModel> modelList = new List<UserModel>();
            foreach (User u in userList)
            {
                UserModel m = new UserModel();
                Organization organization = null;
                if (u.OrganizationId.HasValue)
                {
                    organization = context.GetOrganization(u.OrganizationId.Value, false);
                }
                m.CopyPropertiesFromUser(u, organization);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllUserCount();
            if (organizationId.HasValue)
            {
                Organization organization = context.GetOrganization(organizationId.Value, false);
                if (organization != null)
                {
                    model.ParentId = organization.OrganizationId;
                    model.ParentCaption = string.Format("{0}", organization.Name);
                }
            }
            return model;
        }

        private void RefreshUserRolesDropDownList(UserModel userModel, User currentUser, RepeatEntityContext context)
        {
            if (currentUser == null)
            {
                currentUser = GetCurrentUser(context);
            }
            List<SelectListItem> userRoles = new List<SelectListItem>();
            Array roleValues = EnumHelper.GetEnumValues(typeof(UserRole));
            for (int i = 0; i < roleValues.Length; i++)
            {
                string roleText = roleValues.GetValue(i).ToString();
                UserRole role = (UserRole)Enum.Parse(typeof(UserRole), roleText);
                int roleId = (int)role;
                if ((roleId > currentUser.RoleId) || //Cannot assign a role to a user greater than the current user's role.
                    (userModel != null && userModel.UserId == currentUser.UserId && roleId < currentUser.RoleId)) //Or if the user being edited is the same user that us currently logged in. Current user may not downgrade their own role.
                {
                    continue;
                }
                userRoles.Add(new SelectListItem()
                {
                    Text = roleText,
                    Value = roleText,
                    Selected = userModel != null ? (userModel.Role.ToString() == roleText) : false
                });
            }
            ViewBag.UserRolesList = userRoles;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index(Nullable<Guid> organizationId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated ||
                    (!organizationId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (organizationId.HasValue && !CurrentUserHasAccessToOrganization(organizationId.Value, context)))
                {
                    return RedirectToHome();
                }
                FilterModel<UserModel> model = GetUserFilterModel(context, new FilterModel<UserModel>(), organizationId);
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
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Index(FilterModel<UserModel> model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                Nullable<Guid> organizationId = null;
                if (model.ParentId != Guid.Empty)
                {
                    organizationId = model.ParentId;
                }
                if (!Request.IsAuthenticated ||
                    (!organizationId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (organizationId.HasValue && !CurrentUserHasAccessToOrganization(organizationId.Value, context)))
                {
                    return RedirectToHome();
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                FilterModel<UserModel> resultModel = GetUserFilterModel(context, model, organizationId);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(USER_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsSentLogModel>());
                }
                return PartialView(USER_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid userId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                Organization userOrganization = GetOrganizationFromUser(context, userId, false);
                if (!Request.IsAuthenticated ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (userId != Guid.Empty && userOrganization != null && !CurrentUserHasAccessToOrganization(userOrganization.OrganizationId, context)))
                {
                    return RedirectToHome();
                }
                User user = context.GetUser(userId, true);
                context.Delete<User>(user);
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
                Organization userOrganization = GetOrganizationFromUser(context, identifier, false);
                if (!Request.IsAuthenticated ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (identifier != Guid.Empty && userOrganization != null && !CurrentUserHasAccessToOrganization(userOrganization.OrganizationId, context)))
                {
                    return RedirectToHome();
                }
                User user = context.GetUser(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (user != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete User '{0}' ?", !string.IsNullOrEmpty(user.UserName) ? user.UserName : user.EmailAddress);
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
                Organization userOrganization = GetOrganizationFromUser(context, model.Identifier, false);
                if (!Request.IsAuthenticated ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (model.Identifier != Guid.Empty && userOrganization != null && !CurrentUserHasAccessToOrganization(userOrganization.OrganizationId, context)))
                {
                    return RedirectToHome();
                }
                User user = context.GetUser(model.Identifier, true);
                if (user.UserId == GetCurrentUser(context).UserId)
                {
                    return GetJsonResult(false, string.Format("Cannot delete the currently logged in user '{0}'.", user.UserName));
                }
                context.Delete<User>(user);
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
                string[] searchParameters;
                string searchText;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                string organizationIdString = searchParameters.Length == 4 ? searchParameters[3] : null;
                Nullable<Guid> organizationId = null;
                if (!string.IsNullOrEmpty(organizationIdString))
                {
                    if (!Guid.TryParse(organizationIdString, out Guid parsedOrganizationId))
                    {
                        return GetJsonResult(false, string.Format("Could not read {0} from parmeters.", EntityReader<UserModel>.GetPropertyName(p => p.OrganizationId, false)));
                    }
                    if (parsedOrganizationId != Guid.Empty)
                    {
                        organizationId = parsedOrganizationId;
                    }
                }
                if (!Request.IsAuthenticated ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (organizationId.HasValue && organizationId.Value != Guid.Empty && !CurrentUserHasAccessToOrganization(GetCurrentOrganization(context, true).OrganizationId, context)))
                {
                    return RedirectToHome();
                }
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (organizationId.HasValue && organizationId.Value != Guid.Empty)
                {
                    Organization organization = context.GetOrganization(organizationId.Value, true);
                    model.ConfirmationMessage = string.Format("Delete all Users currently loaded (except current user) for {0} '{1}'?",
                        typeof(Organization).Name,
                        organization.Name);
                    model.ParentId = organization.OrganizationId;
                }
                else
                {
                    model.ConfirmationMessage = "Delete all Users currently loaded (except current user)?";
                }
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
                User currentUser = GetCurrentUser(context);
                if (!Request.IsAuthenticated ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (model.ParentId != Guid.Empty && !CurrentUserHasAccessToOrganization(context.GetOrganization(model.ParentId, true).OrganizationId, context)))
                {
                    return RedirectToHome();
                }
                Nullable<Guid> organizationId = null;
                if (model.ParentId != Guid.Empty)
                {
                    organizationId = model.ParentId;
                }
                context.DeleteUsersByFilter(model.SearchText, currentUser, organizationId);
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
                string organizationIdString = searchParameters.Length == 4 ? searchParameters[3] : null;
                Nullable<Guid> organizationId = null;
                if (!string.IsNullOrEmpty(organizationIdString))
                {
                    if (!Guid.TryParse(organizationIdString, out Guid parsedOrganizationId))
                    {
                        return GetJsonResult(false, string.Format("Could not read {0} from parmeters.", EntityReader<UserModel>.GetPropertyName(p => p.OrganizationId, false)));
                    }
                    if (parsedOrganizationId != Guid.Empty)
                    {
                        organizationId = parsedOrganizationId;
                    }
                }
                if (!Request.IsAuthenticated ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (organizationId.HasValue && organizationId != Guid.Empty && !CurrentUserHasAccessToOrganization(context.GetOrganization(organizationId.Value, true).OrganizationId, context)))
                {
                    return RedirectToHome();
                }
                List<User> userList = context.GetUsersByFilter(searchText, organizationId);
                EntityCache<Guid, UserCsv> cache = new EntityCache<Guid, UserCsv>();
                foreach (User u in userList)
                {
                    UserCsv csv = new UserCsv();
                    Organization organization = null;
                    if (u.OrganizationId.HasValue)
                    {
                        organization = context.GetOrganization(u.OrganizationId.Value, false);
                    }
                    csv.CopyPropertiesFromUser(u, organization);
                    cache.Add(csv.UserId, csv);
                }
                return GetCsvFileResult<UserCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult LoginDialog()
        {
            try
            {
                PartialViewResult result = PartialView(LOGIN_DIALOG_PARTIAL_VIEW_NAME, new User());
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
        public ActionResult LoginDialog(User model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserName))
                {
                    return GetJsonResult(false, string.Format("{0}/{1} not entered.", 
                        EntityReader<User>.GetPropertyName(p => p.UserName, true),
                        EntityReader<User>.GetPropertyName(p => p.EmailAddress, true)));
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (context.IsUserAuthenticated(model.UserName, model.Password))
                {
                    User originalUser = context.GetUserByIdentifier(model.UserName, true); //The UserName in the model specified by the user may be the UserName or the EmailAddress. Hence we need to get the original user from the database to set the UserName as the auth cookie.
                    originalUser.LastLoginDate = DateTime.Now;
                    originalUser.LastActivityDate = DateTime.Now;
                    context.DB.SubmitChanges();
                    FormsAuthentication.SetAuthCookie(
                        originalUser.UserName,
                        Convert.ToBoolean(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.CreatePersistentAuthenticationCookie].SettingValue));
                    return GetJsonResult(true);
                }
                return GetJsonResult(false, string.Format("Invalid {0}/{1} or password.",
                    EntityReader<User>.GetPropertyName(p => p.UserName, true),
                        EntityReader<User>.GetPropertyName(p => p.EmailAddress, true)));
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult ResetPasswordDialog()
        {
            try
            {
                PartialViewResult result = PartialView(RESET_PASSWORD_DIALOG_PARTIAL_VIEW_NAME, new User());
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
        public ActionResult ResetPasswordDialog(User model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserName))
                {
                    return GetJsonResult(false, string.Format("{0}/{1} not entered.",
                        EntityReader<User>.GetPropertyName(p => p.UserName, true),
                        EntityReader<User>.GetPropertyName(p => p.EmailAddress, true)));
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                User user = context.GetUserByIdentifier(model.UserName, false); //The UserName in the model specified by the user may be the UserName or the EmailAddress. Hence we need to get the original user from the database to set the UserName as the auth cookie.
                if (user == null)
                {
                    return GetJsonResult(false, string.Format("{0} with {1}/{2} of '{3}' does not exist.",
                        typeof(User).Name,
                        EntityReader<User>.GetPropertyName(p => p.UserName, true),
                        EntityReader<User>.GetPropertyName(p => p.EmailAddress, true),
                        model.UserName));
                }
                Organization organization = null;
                string organizationName = null;
                Nullable<Guid> organizationId = null;
                if (user.OrganizationId.HasValue)
                {
                    organization = context.GetOrganization(user.OrganizationId.Value, true);
                    organizationName = organization.Name;
                    organizationId = organization.OrganizationId;
                }
                int generatedPasswordLength = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.GeneratedPasswordLength].SettingValue);
                int generatedPasswordNumberOfNonAlphanumericCharacters = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.GeneratedPasswordNumberOfNonAlphanumericCharacters].SettingValue);
                string figlutHomePageUrl = RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.HomePageUrl].SettingValue;
                string newPassword = DataShaper.GeneratePassword(generatedPasswordLength, generatedPasswordNumberOfNonAlphanumericCharacters);
                if (!RepeatWebApp.Instance.EmailSender.SendUserResetPasswordNotificationHtml(
                    user.UserName,
                    user.EmailAddress,
                    user.CellPhoneNumber,
                    newPassword,
                    organizationName,
                    figlutHomePageUrl,
                    organizationId,
                    RepeatWebApp.Instance.Settings.UserPasswordResetEmailDirectory,
                    RepeatWebApp.Instance.Settings.UserPasswordResetEmailFilesDirectory))
                {
                    return GetJsonResult(false, string.Format(
                        "Could not send email notification with your new password to {0}. Password has not been reset. Please try again later.", user.EmailAddress));
                }
                user.Password = newPassword;
                context.Save<User>(user, false);
                return GetJsonResult(true, 
                    string.Format("Password reset successfully. Please check your email, your new password has been emailed to {0}. You should receive it within the next few minutes. Check your Spam folder if you do not see the email in your Inbox.", 
                    user.EmailAddress));
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult Login()
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                return View();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Login(User model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserName))
                {
                    return GetJsonResult(false, string.Format("{0}/{1} not entered.",
                        EntityReader<User>.GetPropertyName(p => p.UserName, true),
                        EntityReader<User>.GetPropertyName(p => p.EmailAddress, true)));
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (context.IsUserAuthenticated(model.UserName, model.Password))
                {
                    User originalUser = context.GetUserByIdentifier(model.UserName, true); //The UserName in the model specified by the user may be the UserName or the EmailAddress. Hence we need to get the original user from the database to set the UserName as the auth cookie.
                    originalUser.LastLoginDate = DateTime.Now;
                    originalUser.LastActivityDate = DateTime.Now;
                    context.DB.SubmitChanges();
                    FormsAuthentication.SetAuthCookie(
                        originalUser.UserName,
                        Convert.ToBoolean(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.CreatePersistentAuthenticationCookie].SettingValue));
                    return GetJsonResult(true);
                }
                return GetJsonResult(false, string.Format("Invalid {0}/{1} or password.",
                    EntityReader<User>.GetPropertyName(p => p.UserName, true),
                        EntityReader<User>.GetPropertyName(p => p.EmailAddress, true)));
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult Register()
        {
            try
            {
                int organizationIdentifierMaxLength = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.OrganizationIdentifierMaxLength].SettingValue);
                return View(new RegisterModel() { OrganizationIdentifierMaxLength = organizationIdentifierMaxLength });
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            try
            {
                ////TODO: allow users to register at a later stage when SMS credits has been implemented.
                //return RedirectToHome();
                return RedirectToHome();

                int organizationIdentifierMaxLength = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.OrganizationIdentifierMaxLength].SettingValue);
                string homePageUrl = RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.HomePageUrl].SettingValue;
                string errorMessage = null;
                if (!model.IsValid(out errorMessage, organizationIdentifierMaxLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                Organization organization = model.CreateOrganization();
                User user = model.CreateUser(organization, UserRole.OrganizationAdmin);
                if (!context.Register(organization, user, out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                RepeatWebApp.Instance.EmailSender.SendUserRegistrationEmailNotificationHtml(
                    model.OrganizationEmailAddress,
                    model.OrganizationName,
                    model.UserEmailAddress,
                    model.UserName,
                    homePageUrl,
                    organization.OrganizationId,
                    RepeatWebApp.Instance.Settings.NewUserRegistrationEmailDirectory,
                    RepeatWebApp.Instance.Settings.NewUserRegistrationEmailFilesDirectory);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult ChangePassword()
        {
            try
            {
                return View(new ChangeUserPasswordModel());
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangeUserPasswordModel model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                User currentUser = GetCurrentUser(context);
                string errorMessage = null;
                if (!model.IsValid(currentUser, out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                currentUser.Password = model.NewPassword;
                context.Save<User>(currentUser, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult LogOff()
        {
            try
            {
                FormsAuthentication.SignOut();
                return RedirectToHome();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditProfileDialog()
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                User currentUser = GetCurrentUser(context);
                Organization organization = null;
                if (currentUser.OrganizationId.HasValue)
                {
                    organization = context.GetOrganization(currentUser.OrganizationId.Value, true);
                }
                UserProfileModel model = new UserProfileModel();
                model.CopyPropertiesFromUser(currentUser, organization);
                PartialViewResult result = PartialView(EDIT_USER_PROFILE_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult EditProfileDialog(UserProfileModel model)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                User user = context.GetUser(model.UserId, true);
                bool userNameHasChanged = user.UserName.ToLower() != model.UserName.ToLower();
                bool emailAddresHasChanged = user.EmailAddress.ToLower() != model.UserEmailAddress.ToLower();
                if (userNameHasChanged)
                {
                    User originalUser = context.GetUserByUserName(model.UserName, false);
                    if (originalUser != null)
                    {
                        return GetJsonResult(false, string.Format("A {0} with the {1} of '{2}' already exists.",
                            typeof(User).Name,
                            EntityReader<User>.GetPropertyName(p => p.UserName, true),
                            model.UserName));
                    }
                }
                if (emailAddresHasChanged)
                {
                    User originalUser = context.GetUserByEmailAddress(model.UserEmailAddress, false);
                    if (originalUser != null)
                    {
                        return GetJsonResult(false, string.Format("A {0} with the {1} of '{2}' already exists.",
                            typeof(User).Name,
                            EntityReader<User>.GetPropertyName(p => p.EmailAddress, true),
                            model.UserName));
                    }
                }
                model.CopyPropertiesToUser(user);
                context.Save<User>(user, false);
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie(
                    user.UserName,
                    Convert.ToBoolean(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.CreatePersistentAuthenticationCookie].SettingValue));
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
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (organizationId.HasValue && organizationId.Value != Guid.Empty && !CurrentUserHasAccessToOrganization(organizationId.Value, context)))
                {
                    return RedirectToHome();
                }
                RefreshUserRolesDropDownList(null, null, context);
                UserModel model = new UserModel();
                if (organizationId.HasValue)
                {
                    model.OrganizationId = organizationId.Value;
                    model.UserEnableEmailNotifications = true;
                }
                PartialViewResult result = PartialView(CREATE_USER_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult CreateDialog(UserModel model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated ||
                    (!model.OrganizationId.HasValue && !IsCurrentUserAdministrator(context)) ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (model.OrganizationId.HasValue && model.OrganizationId.Value != Guid.Empty  && !CurrentUserHasAccessToOrganization(model.OrganizationId.Value, context)))
                {
                    return RedirectToHome();
                }
                User currentUser = GetCurrentUser(context);
                RefreshUserRolesDropDownList(model, currentUser, context);

                int generatedPasswordLength = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.GeneratedPasswordLength].SettingValue);
                int generatedPasswordNumberOfNonAlphanumericCharacters = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.GeneratedPasswordNumberOfNonAlphanumericCharacters].SettingValue);
                model.UserId = Guid.NewGuid();
                model.UserPassword = model.UserPasswordConfirm = DataShaper.GeneratePassword(generatedPasswordLength, generatedPasswordNumberOfNonAlphanumericCharacters);
                model.DateCreated = DateTime.Now;

                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                if (!model.UserPassword.Equals(model.UserPasswordConfirm))
                {
                    return GetJsonResult(false, string.Format("Passwords entered do not match."));
                }
                User user = context.GetUserByUserName(model.UserName, false);
                if (user != null)
                {
                    return GetJsonResult(false, string.Format("A {0} with the {1} of '{2}' already exists.",
                        typeof(User).Name,
                        EntityReader<User>.GetPropertyName(p => p.UserName, true),
                        model.UserName));
                }
                user = context.GetUserByEmailAddress(model.UserEmailAddress, false);
                if (user != null)
                {
                    return GetJsonResult(false, string.Format("A {0} with the {1} of '{2}' already exists.",
                        typeof(User).Name,
                        EntityReader<User>.GetPropertyName(p => p.EmailAddress, true),
                        model.UserEmailAddress));
                }
                user = new ORM.User();
                model.CopyPropertiesToUser(user);
                Organization userOrganization = null;
                if (user.OrganizationId.HasValue)
                {
                    userOrganization = context.GetOrganization(user.OrganizationId.Value, true);
                }
                context.Save<User>(user, false);
                string figlutHomePageUrl = RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.HomePageUrl].SettingValue;
                RepeatWebApp.Instance.EmailSender.SendUserCreatedEmailHtml(
                    currentUser.UserName,
                    user.UserName,
                    user.EmailAddress,
                    user.CellPhoneNumber,
                    user.Password,
                    userOrganization != null ? userOrganization.Name : null,
                    figlutHomePageUrl,
                    userOrganization.OrganizationId,
                    RepeatWebApp.Instance.Settings.NewUserCreatedEmailDirectory,
                    RepeatWebApp.Instance.Settings.NewUserCreatedEmailFilesDirectory);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> userId)
        {
            try
            {
                if (!userId.HasValue)
                {
                    return PartialView(EDIT_USER_DIALOG_PARTIAL_VIEW_NAME, new UserModel());
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                Organization userOrganization = userId.HasValue ? GetOrganizationFromUser(context, userId.Value, false) : null;
                if (!Request.IsAuthenticated ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (userId.HasValue && userId.Value != Guid.Empty && userOrganization != null && !CurrentUserHasAccessToOrganization(userOrganization.OrganizationId, context)))
                {
                    return RedirectToHome();
                }
                User user = context.GetUser(userId.Value, true);
                UserModel model = new UserModel();
                model.CopyPropertiesFromUser(user, null);

                RefreshUserRolesDropDownList(model, null, context);
                //ViewBag.Role = model.Role;
                PartialViewResult result = PartialView(EDIT_USER_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(UserModel model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                Organization userOrganization = GetOrganizationFromUser(context, model.UserId, false);
                if (!Request.IsAuthenticated ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (model.UserId != Guid.Empty && userOrganization != null && !CurrentUserHasAccessToOrganization(userOrganization.OrganizationId, context)))
                {
                    return RedirectToHome();
                }
                User currentUser = GetCurrentUser(context);
                RefreshUserRolesDropDownList(model, currentUser, context);
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                User user = context.GetUser(model.UserId, true);
                if (currentUser.RoleId < user.RoleId)
                {
                    return GetJsonResult(false, string.Format("You may not edit a user that has a higher role than your own role."));
                }
                if (currentUser.UserId == user.UserId && ((int)model.Role) < user.RoleId) //Current user is editing their own user profile and assigning a lower role.
                {
                    return GetJsonResult(false, string.Format("You may not assign a lower role to the user profile that you are currently logged in as."));
                }
                bool userNameHasChanged = (user.UserName.ToLower() != model.UserName.ToLower());
                if (userNameHasChanged && context.UserExistsByUserName(model.UserName))
                {
                    return GetJsonResult(false, string.Format("A {0} with the {1} of '{2}' already exists.",
                        typeof(User).Name,
                        EntityReader<User>.GetPropertyName(p => p.UserName, false),
                        user.UserName));
                }
                bool emailAddressHasChanged = user.EmailAddress.ToLower() != model.UserEmailAddress.ToLower();
                if (emailAddressHasChanged && context.UserExistsByEmailAddress(model.UserEmailAddress))
                {
                    return GetJsonResult(false, string.Format("A {0} with the {1} of '{2}' already exists.",
                        typeof(User).Name,
                        EntityReader<User>.GetPropertyName(p => p.EmailAddress, false),
                        user.EmailAddress));
                }
                model.CopyPropertiesToUser(user);
                context.Save<User>(user, false);
                 if (currentUser.UserId == user.UserId && userNameHasChanged) //The current user is editing their own username.
                {
                    FormsAuthentication.SignOut();
                    FormsAuthentication.SetAuthCookie(
                        user.UserName,
                        Convert.ToBoolean(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.CreatePersistentAuthenticationCookie].SettingValue));
                    return GetJsonResult(true);
                }
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult EditUserPasswordDialog(Nullable<Guid> userId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!userId.HasValue)
                {
                    return PartialView(EDIT_USER_PASSWORD_DIALOG_PARTIAL_VIEW_NAME, new EditUserPasswordModel());
                }
                Organization userOrganization = userId.HasValue ? GetOrganizationFromUser(context, userId.Value, false) : null;
                if (!Request.IsAuthenticated ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (userId.HasValue && userId.Value != Guid.Empty && userOrganization != null && !CurrentUserHasAccessToOrganization(userOrganization.OrganizationId, context)))
                {
                    return RedirectToHome();
                }
                User user = context.GetUser(userId.Value, true);
                EditUserPasswordModel model = new EditUserPasswordModel() { UserId = user.UserId, UserName = user.UserName };
                PartialViewResult result = PartialView(EDIT_USER_PASSWORD_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditUserPasswordDialog(EditUserPasswordModel model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                Organization userOrganization = GetOrganizationFromUser(context, model.UserId, false);
                if (!Request.IsAuthenticated ||
                    !IsCurrentUserOfRole(UserRole.OrganizationAdmin, context) ||
                    (model.UserId != Guid.Empty && userOrganization != null && !CurrentUserHasAccessToOrganization(userOrganization.OrganizationId, context)))
                {
                    return RedirectToHome();
                }
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                User user = context.GetUser(model.UserId, true);
                user.Password = model.NewPassword;
                context.Save<User>(user, false);
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
