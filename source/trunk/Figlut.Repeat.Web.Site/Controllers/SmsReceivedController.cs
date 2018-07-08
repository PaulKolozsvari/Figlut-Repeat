namespace Figlut.Repeat.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.ORM.Csv;
    using Figlut.Repeat.ORM.Helpers;
    using Figlut.Repeat.Web.Site.Configuration;
    using Figlut.Repeat.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class SmsReceivedController : RepeatController
    {
        #region Constants

        private const string SMS_RECEIVED_GRID_PARTIAL_VIEW_NAME = "_SmsReceivedGrid";

        #endregion //Constants

        #region Methods

        private FilterModel<SmsReceivedLogModel> GetSmsReceivedFilterModel(RepeatEntityContext context, FilterModel<SmsReceivedLogModel> model)
        {
            if (context == null)
            {
                context = RepeatEntityContext.Create();
            }
            DateTime startDate;
            DateTime endDate;
            if (!model.StartDate.HasValue || !model.EndDate.HasValue) //Has not been specified by the user on the page yet i.e. this is the first time the page is loading.
            {
                context.GetGlobalSettingSmsDaysDateRange(out startDate, out endDate);
                model.StartDate = startDate;
                model.EndDate = endDate;
            }
            else
            {
                if (model.StartDate > model.EndDate)
                {
                    SetViewBagErrorMessage(string.Format("{0} may not be later than {1}.",
                        EntityReader<FilterModel<SmsReceivedLogModel>>.GetPropertyName(p => p.StartDate, true),
                        EntityReader<FilterModel<SmsReceivedLogModel>>.GetPropertyName(p => p.EndDate, true)));
                    return null;
                }
                startDate = model.StartDate.Value;
                endDate = model.EndDate.Value;
            }
            int maxiumSmsDateRangeDaysToDisplay = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.MaximumSmsDateRangeDaysToDisplay].SettingValue);
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            if (!model.IsAdministrator && endDate.Date.Subtract(startDate.Date).Days > maxiumSmsDateRangeDaysToDisplay)
            {
                SetViewBagErrorMessage(string.Format("Date range may not be greater than {0} days.", maxiumSmsDateRangeDaysToDisplay));
                return null;
            }
            bool smsContentsTrimOnGrid = Convert.ToBoolean(RepeatWebApp.Instance.GlobalSettings [GlobalSettingName.SmsContentsTrimOnGrid].SettingValue);
            int smsContentsTrimLengthOnGrid = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.SmsContentsTrimLengthOnGrid].SettingValue);
            bool cellPhoneNumberTrimOnGrid = Convert.ToBoolean(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.CellPhoneNumberTrimOnGrid].SettingValue);
            int cellPhoneNumberTrimLengthOnGrid = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.CellPhoneNumberTrimLengthOnGrid].SettingValue);
            bool trimCellPhoneNumber = model.IsAdministrator ? false : cellPhoneNumberTrimOnGrid;
            List<SmsReceivedLog> smsReceivedLogList = null;
            if (model.IsAdministrator)
            {
                smsReceivedLogList = context.GetSmsReceivedLogByFilter(model.SearchText, startDate, endDate, null);
            }
            else
            {
                User currentUser = GetCurrentUser(context);
                if (!currentUser.OrganizationId.HasValue)
                {
                    SetViewBagErrorMessage(string.Format("User {0} is not part of an Organization.", currentUser.UserName));
                    return null;
                }
                smsReceivedLogList = context.GetSmsReceivedLogByFilter(model.SearchText, startDate, endDate, currentUser.OrganizationId.Value);
            }
            List<SmsReceivedLogModel> modelList = new List<SmsReceivedLogModel>();
            foreach (SmsReceivedLog s in smsReceivedLogList)
            {
                SmsReceivedLogModel m = new SmsReceivedLogModel();
                m.CopyPropertiesFromSmsReceivedLog(s);
                if (smsContentsTrimOnGrid)
                {
                    m.MessageContentsTrimmed = DataEditor.GetTrimmedMessageContents(s.MessageContents, smsContentsTrimLengthOnGrid);
                }
                if (trimCellPhoneNumber)
                {
                    m.CellPhoneNumber = DataEditor.GetTrimmedCellPhoneNumber(m.CellPhoneNumber, 8);
                }
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllSmsReceivedLogCount();
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index()
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<SmsReceivedLogModel> model = GetSmsReceivedFilterModel(context, new FilterModel<SmsReceivedLogModel>());
                if (model == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return View(new FilterModel<SmsReceivedLogModel>());
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
        public ActionResult Index(FilterModel<SmsReceivedLogModel> model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                ViewBag.SearchFieldIdentifier = model.SearchFieldIdentifier;
                FilterModel<SmsReceivedLogModel> resultModel = GetSmsReceivedFilterModel(context, model);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(SMS_RECEIVED_GRID_PARTIAL_VIEW_NAME, new FilterModel<SmsReceivedLogModel>());
                }
                return PartialView(SMS_RECEIVED_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid smsReceivedLogId)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                SmsReceivedLog smsReceived = context.GetSmsReceivedLog(smsReceivedLogId, true);
                context.Delete<SmsReceivedLog>(smsReceived);
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
                SmsReceivedLog smsReceived = context.GetSmsReceivedLog(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (smsReceived != null)
                {
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete SMS '{0}' ?", !string.IsNullOrEmpty(smsReceived.MessageId) ? smsReceived.MessageId : smsReceived.MessageContents);
                }
                PartialViewResult result = PartialView(CONFIRMATION_DIALOG_PARTIAL_VIEW_NAME, model);
                return result;
            }
            catch(Exception ex)
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
                SmsReceivedLog smsReceived = context.GetSmsReceivedLog(model.Identifier, true);
                context.Delete<SmsReceivedLog>(smsReceived);
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
                Nullable<DateTime> startDate;
                Nullable<DateTime> endDate;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText, out startDate, out endDate);

                model.SearchText = searchText;
                model.StartDate = startDate;
                model.EndDate = endDate;

                if (model.StartDate.HasValue && model.EndDate.HasValue)
                {
                    model.ConfirmationMessage = string.Format(
                        "Delete all SMS' currently loaded between {0}-{1}-{2} and {3}-{4}-{5} ?",
                        model.StartDate.Value.Year.ToString(),
                        model.StartDate.Value.Month.ToString(),
                        model.StartDate.Value.Day.ToString(),
                        model.EndDate.Value.Year.ToString(),
                        model.EndDate.Value.Month.ToString(),
                        model.EndDate.Value.Day.ToString());
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
        public ActionResult ConfirmDeleteAllDialog(ConfirmationModel model)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (IsCurrentUserAdministrator(context))
                {
                    context.DeleteSmsReceivedLogByFilter(model.SearchText, model.StartDate.Value, model.EndDate.Value, null);
                }
                else
                {
                    Organization organization = GetCurrentOrganization(context, true);
                    context.DeleteSmsReceivedLogByFilter(model.SearchText, model.StartDate.Value, model.EndDate.Value, organization.OrganizationId);
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

        public ActionResult DownloadCsvFile(string searchParametersString)
        {
            try
            {
                RepeatEntityContext context = RepeatEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                string[] searchParameters;
                string searchText;
                Nullable<DateTime> startDate;
                Nullable<DateTime> endDate;
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText, out startDate, out endDate);
                if (!startDate.HasValue)
                {
                    throw new ArgumentException("Start Date not specified.");
                }
                if (!endDate.HasValue)
                {
                    throw new ArgumentException("End Date not specified.");
                }
                List<SmsReceivedLog> smsList = null;
                if (IsCurrentUserAdministrator(context))
                {
                    smsList = context.GetSmsReceivedLogByFilter(searchText, startDate.Value, endDate.Value, null);
                }
                else
                {
                    Organization organization = GetCurrentOrganization(context, true);
                    smsList = context.GetSmsReceivedLogByFilter(searchText, startDate.Value, endDate.Value, organization.OrganizationId);
                }
                bool cellPhoneNumberTrimOnGrid = Convert.ToBoolean(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.CellPhoneNumberTrimOnGrid].SettingValue);
                int cellPhoneNumberTrimLengthOnGrid = Convert.ToInt32(RepeatWebApp.Instance.GlobalSettings[GlobalSettingName.CellPhoneNumberTrimLengthOnGrid].SettingValue);
                bool trimCellPhoneNumber = IsCurrentUserAdministrator(context) ? false : cellPhoneNumberTrimOnGrid;
                EntityCache<Guid, SmsReceivedCsv> cache = new EntityCache<Guid, SmsReceivedCsv>();
                foreach (SmsReceivedLog s in smsList)
                {
                    SmsReceivedCsv csv = new SmsReceivedCsv();
                    csv.CopyPropertiesFromSmsReceivedLog(s);
                    if (trimCellPhoneNumber)
                    {
                        csv.CellPhoneNumber = DataEditor.GetTrimmedCellPhoneNumber(csv.CellPhoneNumber, 8);
                    }
                    cache.Add(csv.SmsReceivedLogId, csv);
                }
                return GetCsvFileResult<SmsReceivedCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        #endregion //Actions
    }
}