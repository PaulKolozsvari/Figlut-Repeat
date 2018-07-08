namespace Figlut.Repeat.Web.Site.Controllers
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Repeat.Data;
    using Figlut.Repeat.ORM;
    using Figlut.Repeat.Web.Site.Configuration;
    using Figlut.Repeat.Web.Site.Models;

    #endregion //Using Directives

    public class OrderController : RepeatController
    {
        #region Actions

        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult CreateOrder()
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToHome();
                }
                RepeatEntityContext context = RepeatEntityContext.Create();
                User currentUser = GetCurrentUser(context);
                Organization currentOrganization = GetCurrentOrganization(context, true);
                OrganizationSubscriptionType currentOrganizationSubscriptionType = null;
                if (currentOrganization.OrganizationSubscriptionTypeId.HasValue)
                {
                    currentOrganizationSubscriptionType = context.GetOrganizationSubscriptionType(currentOrganization.OrganizationSubscriptionTypeId.Value, true);
                }
                CreateOrderModel model = new CreateOrderModel();
                model.CopyPropertiesFromOrganization(currentOrganization, currentOrganizationSubscriptionType);
                model.ProductName = ProductName.SmsCredits.ToString();
                model.Quantity = 100;
                return View(model);
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
