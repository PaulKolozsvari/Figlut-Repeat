namespace Figlut.Spread.Web.Site.Controllers
{
    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Spread.Data;
    using Figlut.Spread.ORM;
    using Figlut.Spread.Web.Site.Configuration;
    using Figlut.Spread.Web.Site.Models;
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class OrderController : SpreadController
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
                SpreadEntityContext context = SpreadEntityContext.Create();
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
                SpreadWebApp.Instance.EmailSender.SendExceptionEmailNotification(ex);
                return RedirectToError(ex.Message);
            }
        }

        #endregion //Actions
    }
}
