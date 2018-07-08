namespace Figlut.Repeat.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Data;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.ORM.Helpers;
    using Figlut.Repeat.Web.Site.Configuration;
    using Figlut.Repeat.Web.Site.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class HomeController : RepeatController
    {
        #region Actions

        public ActionResult Index()
        {
            try
            {
                HomePageImageGalleryModel model = new HomePageImageGalleryModel();
                if (!Directory.Exists(RepeatWebApp.Instance.Settings.HowitWorksImageGalleryDirectory))
                {
                    throw new DirectoryNotFoundException(string.Format("Could not find {0}: '{1}'.",
                        EntityReader<RepeatWebSettings>.GetPropertyName(p => p.HowitWorksImageGalleryDirectory, true),
                        RepeatWebApp.Instance.Settings.HowitWorksImageGalleryDirectory));
                }
                model.HowItWorksImages = Directory.GetFiles(RepeatWebApp.Instance.Settings.HowitWorksImageGalleryDirectory).ToList();
                model.WhyItWorksImages = Directory.GetFiles(RepeatWebApp.Instance.Settings.WhyItWorksImageGalleryDirectory).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
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
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        #endregion //Actions
    }
}
