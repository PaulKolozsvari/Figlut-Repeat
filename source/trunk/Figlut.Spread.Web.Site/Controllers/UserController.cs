namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Spread.Data;
    using Figlut.Spread.Email;
    using Figlut.Spread.ORM;
    using Figlut.Spread.ORM.Csv;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    #endregion //Using Directives

    public class UserController : SpreadController
    {
        #region Constants

        private const string USER_GRID_PARTIAL_VIEW_NAME = "_UserGrid";
        private const string EDIT_USER_DIALOG_PARTIAL_VIEW_NAME = "_EditUserDialog";
        private const string EDIT_USER_PASSWORD_DIALOG_PARTIAL_VIEW_NAME = "_EditUserPasswordDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<UserModel> GetUserFilterModel(SpreadEntityContext context, FilterModel<UserModel> model)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<User> userList = context.GetUsersByFilter(model.SearchText);
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
                FilterModel<UserModel> model = GetUserFilterModel(context, new FilterModel<UserModel>());
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
        public ActionResult Index(FilterModel<UserModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                FilterModel<UserModel> resultModel = GetUserFilterModel(context, model);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(USER_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsSentLogModel>());
                }
                return PartialView(USER_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid userId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated || !IsCurrentUserAdministrator(context))
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
                model.ConfirmationMessage = "Delete all Users currently loaded (except current user)?";
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
                User currentUser = GetCurrentUser(context);
                context.DeleteUsersByFilter(model.SearchText, currentUser);
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
                List<User> userList = context.GetUsersByFilter(searchText);
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
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
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
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
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
                    return GetJsonResult(false, string.Format("{0} not entered.", EntityReader<User>.GetPropertyName(p => p.UserName, true)));
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (context.IsUserAuthenticated(model.UserName, model.Password))
                {
                    User originalUser = context.GetUserByIdentifier(model.UserName, true); //The UserName in the model specified by the user may be the UserName or the EmailAddress. Hence we need to get the original user from the database to set the UserName as the auth cookie.
                    originalUser.LastLoginDate = DateTime.Now;
                    context.DB.SubmitChanges();
                    FormsAuthentication.SetAuthCookie(
                        originalUser.UserName,
                        Convert.ToBoolean(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.CreatePersistentAuthenticationCookie].SettingValue));
                    return GetJsonResult(true);
                }
                return GetJsonResult(false, "Invalid user name or password.");
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult Register()
        {
            try
            {
                int organizationIdentifierMaxLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.OrganizationIdentifierMaxLength].SettingValue);
                return View(new RegisterModel() { OrganizationIdentifierMaxLength = organizationIdentifierMaxLength });
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
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
                int organizationIdentifierMaxLength = Convert.ToInt32(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.OrganizationIdentifierMaxLength].SettingValue);
                string errorMessage = null;
                if (!model.IsValid(out errorMessage, organizationIdentifierMaxLength))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                Organization organization = model.CreateOrganization();
                User user = model.CreateUser(organization, UserRole.OrganizationAdmin);
                if (!context.Register(organization, user, out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SendRegistrationEmailNotification(model);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        private void SendRegistrationEmailNotification(RegisterModel registerModel)
        {
            List<EmailNotificationRecipient> recipients = new List<EmailNotificationRecipient>();
            recipients.Add(new EmailNotificationRecipient() { EmailAddress = registerModel.OrganizationEmailAddress, DisplayName = registerModel.OrganizationName });
            recipients.Add(new EmailNotificationRecipient() { EmailAddress = registerModel.UserEmailAddress, DisplayName = registerModel.UserName });
            
            string subject = "Welcome to Figlut";
            StringBuilder body = new StringBuilder();
            body.AppendLine(string.Format("Hi {0},", registerModel.UserName));
            body.AppendLine();
            body.AppendLine("Thank you for registering with Figlut. Someone will be in contact with you shortly to help you get started.");
            body.AppendLine("We look forward to working with you to promote your business through our platform.");
            body.AppendLine();
            body.AppendLine("Regards,");
            body.AppendLine();
            body.AppendLine("The Figlut Team");

            SpreadWebApp.Instance.EmailSender.SendEmail(subject, body.ToString(), null, false, recipients, null);
        }

        public ActionResult EditProfile()
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                User currentUser = GetCurrentUser(context);
                Organization organization = null;
                if (currentUser.OrganizationId.HasValue)
                {
                    organization = context.GetOrganization(currentUser.OrganizationId.Value, true);
                }
                UserProfileModel model = new UserProfileModel();
                model.CopyPropertiesFromUser(currentUser, organization);
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
        public ActionResult EditProfile(UserProfileModel model)
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
                SpreadEntityContext context = SpreadEntityContext.Create();
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
                    Convert.ToBoolean(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.CreatePersistentAuthenticationCookie].SettingValue));
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
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
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangeUserPasswordModel model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
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
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
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
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        private void RefreshUserRolesDropDownList(UserModel userModel)
        {
            List<SelectListItem> userRoles = new List<SelectListItem>();
            Array roleValues = EnumHelper.GetEnumValues(typeof(UserRole));
            for (int i = 0; i < roleValues.Length; i++)
            {
                string text = roleValues.GetValue(i).ToString();
                userRoles.Add(new SelectListItem()
                {
                    Text = text,
                    Value = text,
                    Selected = userModel != null ? (userModel.Role.ToString() == text) : false
                });
            }
            ViewBag.UserRolesList = userRoles;
        }

        public ActionResult EditDialog(Nullable<Guid> userId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                if (!userId.HasValue)
                {
                    return PartialView(EDIT_USER_DIALOG_PARTIAL_VIEW_NAME, new UserModel());
                }
                User user = context.GetUser(userId.Value, true);
                UserModel model = new UserModel();
                model.CopyPropertiesFromUser(user, null);

                RefreshUserRolesDropDownList(model);
                //ViewBag.Role = model.Role;
                PartialViewResult result = PartialView(EDIT_USER_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(UserModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                User currentUser = GetCurrentUser(context);
                User user = context.GetUser(model.UserId, true);
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
                        Convert.ToBoolean(SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.CreatePersistentAuthenticationCookie].SettingValue));
                    return GetJsonResult(true);
                }
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult EditUserPasswordDialog(Nullable<Guid> userId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!IsCurrentUserAdministrator(context))
                {
                    return RedirectToHome();
                }
                if (!userId.HasValue)
                {
                    return PartialView(EDIT_USER_PASSWORD_DIALOG_PARTIAL_VIEW_NAME, new EditUserPasswordModel());
                }
                User user = context.GetUser(userId.Value, true);
                EditUserPasswordModel model = new EditUserPasswordModel() { UserId = user.UserId, UserName = user.UserName };
                PartialViewResult result = PartialView(EDIT_USER_PASSWORD_DIALOG_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditUserPasswordDialog(EditUserPasswordModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                User user = context.GetUser(model.UserId, true);
                user.Password = model.NewPassword;
                context.Save<User>(user, false);
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
