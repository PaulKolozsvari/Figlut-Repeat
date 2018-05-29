namespace Figlut.Spread.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM.Helpers;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class HomeController : SpreadController
    {
        #region Actions

        public ActionResult Index()
        {
            try
            {
                HomePageImageGalleryModel model = new HomePageImageGalleryModel();
                if (!Directory.Exists(SpreadWebApp.Instance.Settings.HowitWorksImageGalleryDirectory))
                {
                    throw new DirectoryNotFoundException(string.Format("Could not find {0}: '{1}'.",
                        EntityReader<SpreadWebSettings>.GetPropertyName(p => p.HowitWorksImageGalleryDirectory, true),
                        SpreadWebApp.Instance.Settings.HowitWorksImageGalleryDirectory));
                }
                model.HowItWorksImages = Directory.GetFiles(SpreadWebApp.Instance.Settings.HowitWorksImageGalleryDirectory).ToList();
                model.WhyItWorksImages = Directory.GetFiles(SpreadWebApp.Instance.Settings.WhyItWorksImageGalleryDirectory).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult HowItWorks()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        public ActionResult Contact()
        {
            try
            {
                SpreadEntityContext context = SpreadEntityContext.Create();
                GlobalSettingsModel model = new GlobalSettingsModel();
                model.FiglutPhoneNumber = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.FiglutPhoneNumber].SettingValue;
                model.FiglutSupportEmailAddress = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.FiglutSupportEmailAddress].SettingValue;
                model.FiglutMarketingEmailAddress = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.FiglutMarketingEmailAddress].SettingValue;
                model.FiglutGeneralEmailAddress = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.FiglutGeneralEmailAddress].SettingValue;
                model.FiglutAddress = SpreadWebApp.Instance.GlobalSettings[GlobalSettingName.FiglutAddress].SettingValue;
                return View(model);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return RedirectToError(ex.Message);
            }
        }

        #endregion //Actions
    }
}
