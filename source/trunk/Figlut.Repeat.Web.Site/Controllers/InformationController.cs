namespace Figlut.Repeat.Web.Site.Controllers
{
    #region Using Directives

    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Repeat.Web.Site.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class InformationController : Controller
    {
        #region Actions

        public ActionResult Error(string errorMessage)
        {
            try
            {
                ViewBag.ErrorMessage = errorMessage;
                return View();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                throw ex;
            }
        }

        public ActionResult Information(string informationMessage)
        {
            try
            {
                ViewBag.InformationMessage = informationMessage;
                return View();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                RepeatWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                throw ex;
            }
        }

        #endregion //Actions
    }
}
