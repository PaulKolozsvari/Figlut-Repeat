namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Figlut.Spread.Web.Site.Models;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM.Views;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.ORM;
    using Figlut.Server.Toolkit.Data;
    using Figlut.Spread.ORM.Csv;

    #endregion //Using Directives

    public class RepeatScheduleEntryController : SpreadController
    {
        #region Constants

        private const string REPEAT_SCHEDULE_ENTRY_GRID_PARTIAL_VIEW_NAME = "_RepeatScheduleEntryGrid";
        private const string EDIT_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME = "_EditRepeatScheduleEntryDialog";
        private const string CREATE_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME = "_CreateRepeatScheduleEntryDialog";

        #endregion //Constants

        #region Methods

        private FilterModel<RepeatScheduleEntryModel> GetRepeatScheduleEntryFilterModel(
            SpreadEntityContext context,
            FilterModel<RepeatScheduleEntryModel> model,
            Nullable<Guid> repeatScheduleId)
        {
            if (context == null)
            {
                context = SpreadEntityContext.Create();
            }
            model.IsAdministrator = IsCurrentUserAdministrator(context);
            List<RepeatScheduleEntryView> repeatScheduleEntryViewList = context.GetRepeatScheduleEntryViewsByFilter(model.SearchText, repeatScheduleId);
            List<RepeatScheduleEntryModel> modelList = new List<RepeatScheduleEntryModel>();
            foreach (RepeatScheduleEntryView v in repeatScheduleEntryViewList)
            {
                RepeatScheduleEntryModel m = new RepeatScheduleEntryModel();
                m.CopyPropertiesFromRepeatScheduleEntryView(v);
                modelList.Add(m);
            }
            model.DataModel.Clear();
            model.DataModel = modelList;
            model.TotalTableCount = context.GetAllRepeatScheduleEntryCount();
            if (repeatScheduleId.HasValue)
            {
                RepeatScheduleView repeatSchedule = context.GetRepeatScheduleView(repeatScheduleId.Value, false);
                if (repeatSchedule != null)
                {
                    model.ParentId = repeatSchedule.RepeatScheduleId;
                    model.ParentCaption = string.Format("{0} {1} {2}", repeatSchedule.ScheduleName, repeatSchedule.CustomerFullName, repeatSchedule.CellPhoneNumber);
                }
            }
            return model;
        }

        #endregion //Methods

        #region Actions

        public ActionResult Index(Guid repeatScheduleId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<RepeatScheduleEntryModel> model = GetRepeatScheduleEntryFilterModel(
                    context,
                    new FilterModel<RepeatScheduleEntryModel>(),
                    repeatScheduleId);
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
        public ActionResult Index(FilterModel<RepeatScheduleEntryModel> model)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                FilterModel<RepeatScheduleEntryModel> resultModel = model.ParentId != Guid.Empty ?
                    GetRepeatScheduleEntryFilterModel(context, model, model.ParentId) :
                    GetRepeatScheduleEntryFilterModel(context, model, null);
                if (resultModel == null) //There was an error and ViewBag.ErrorMessage has been set. So just return an empty model.
                {
                    return PartialView(REPEAT_SCHEDULE_ENTRY_GRID_PARTIAL_VIEW_NAME, new FilterModel<PublicHolidayModel>());
                }
                return PartialView(REPEAT_SCHEDULE_ENTRY_GRID_PARTIAL_VIEW_NAME, resultModel);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid repeatScheduleEntryId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                RepeatScheduleEntry repeatScheduleEntry = context.GetRepeatScheduleEntry(repeatScheduleEntryId, true);
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                context.Delete<RepeatScheduleEntry>(repeatScheduleEntry);
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
                RepeatScheduleEntry repeatScheduleEntry = context.GetRepeatScheduleEntry(identifier, false);
                ConfirmationModel model = new ConfirmationModel();
                model.PostBackControllerAction = GetCurrentActionName();
                model.PostBackControllerName = GetCurrentControllerName();
                model.DialogDivId = CONFIRMATION_DIALOG_DIV_ID;
                if (repeatScheduleEntry != null)
                {
                    RepeatScheduleView repeatSchedule = context.GetRepeatScheduleView(repeatScheduleEntry.RepeatScheduleId, true);
                    model.Identifier = identifier;
                    model.ConfirmationMessage = string.Format("Delete Repeat Schedule Entry '{0}' for {1} '{2} ({3})'?",
                        repeatScheduleEntry.RepeatDate,
                        DataShaper.ShapeCamelCaseString(typeof(RepeatSchedule).Name),
                        repeatSchedule.ScheduleName,
                        repeatSchedule.CustomerFullName);
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
                RepeatScheduleEntry repeatScheduleEntry = context.GetRepeatScheduleEntry(model.Identifier, true);
                context.Delete<RepeatScheduleEntry>(repeatScheduleEntry);
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

                string repeatScheduleIdString = searchParameters[searchParameters.Length - 1];
                Guid repeatScheduleId = Guid.Parse(repeatScheduleIdString);
                RepeatScheduleView repeatSchedule = context.GetRepeatScheduleView(repeatScheduleId, true);
                model.ParentId = repeatSchedule.RepeatScheduleId;
                model.ParentCaption = string.Format("{0} {1} {2}", repeatSchedule.ScheduleName, repeatSchedule.CustomerFullName, repeatSchedule.CellPhoneNumber);
                model.SearchText = searchText;
                model.ConfirmationMessage = string.Format("Delete all Repeat Schedules Entries currently loaded for {0} '{1}' for subscription {2} {3}'?", 
                    DataShaper.ShapeCamelCaseString(typeof(RepeatSchedule).Name), 
                    repeatSchedule.ScheduleName,
                    repeatSchedule.CustomerFullName,
                    repeatSchedule.CellPhoneNumber);
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
                        EntityReader<Country>.GetPropertyName(p => p.CountryId, false)));
                }
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                context.DeleteRepeatScheduleEntriesbyFilter(model.SearchText, model.ParentId);
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

                string repeatScheduleIdString = searchParameters[searchParameters.Length - 1];
                Guid repeatScheduleId = Guid.Parse(repeatScheduleIdString);
                RepeatSchedule repeatSchedule = context.GetRepeatSchedule(repeatScheduleId, true);
                if (repeatScheduleId == Guid.Empty)
                {
                    return RedirectToError(string.Format("{0} not specified.",
                        EntityReader<RepeatSchedule>.GetPropertyName(p => p.RepeatScheduleId, false)));
                }
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                GetConfirmationModelFromSearchParametersString(searchParametersString, out searchParameters, out searchText);
                List<RepeatScheduleEntryView> repeatScheduleEntryViewList = context.GetRepeatScheduleEntryViewsByFilter(searchText, repeatScheduleId);
                EntityCache<Guid, RepeatScheduleEntryCsv> cache = new EntityCache<Guid, RepeatScheduleEntryCsv>();
                foreach (RepeatScheduleEntryView v in repeatScheduleEntryViewList)
                {
                    RepeatScheduleEntryCsv csv = new RepeatScheduleEntryCsv();
                    csv.CopyPropertiesFromRepeatScheduleEntryView(v);
                    cache.Add(csv.RepeatScheduleEntryId, csv);
                }
                return GetCsvFileResult<RepeatScheduleEntryCsv>(cache);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult EditDialog(Nullable<Guid> repeatScheduleEntryId)
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                if (!repeatScheduleEntryId.HasValue)
                {
                    return PartialView(EDIT_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, new RepeatScheduleEntryModel());
                }
                RepeatScheduleEntryView repeatScheduleEntryView = context.GetRepeatScheduleEntryView(repeatScheduleEntryId.Value, true);
                RepeatScheduleEntryModel model = new RepeatScheduleEntryModel();
                model.CopyPropertiesFromRepeatScheduleEntryView(repeatScheduleEntryView);
                PartialViewResult result = PartialView(EDIT_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, model);
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
        public ActionResult EditDialog(RepeatScheduleEntryModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                RepeatScheduleEntry repeatScheduleEntry = context.GetRepeatScheduleEntry(model.RepeatScheduleEntryId, true);
                model.CopyPropertiesToRepeatScheduleEntry(repeatScheduleEntry);
                context.Save<RepeatScheduleEntry>(repeatScheduleEntry, false);
                return GetJsonResult(true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(false, ex.Message);
            }
        }

        public ActionResult CreateDialog(Nullable<Guid> repeatScheduleId)
        {
            try
            {
                return PartialView(CREATE_REPEAT_SCHEDULE_ENTRY_PARTIAL_VIEW_NAME, new RepeatScheduleEntryModel());
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return GetJsonResult(true);
            }
        }

        [HttpPost]
        public ActionResult CreateDialog(RepeatScheduleEntryModel model)
        {
            try
            {
                string errorMessage = null;
                if (!model.IsValid(out errorMessage))
                {
                    return GetJsonResult(false, errorMessage);
                }
                SpreadEntityContext context = SpreadEntityContext.Create();
                model.RepeatScheduleId = Guid.NewGuid();
                model.DateCreated = DateTime.Now;
                RepeatScheduleEntry repeatScheduleEntry = new RepeatScheduleEntry();
                model.CopyPropertiesToRepeatScheduleEntry(repeatScheduleEntry);
                context.Save<RepeatScheduleEntry>(repeatScheduleEntry, false);
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
