namespace Figlut.Repeat.Web.Site.Controllers
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    #endregion //Using Directives

    public class InvoiceController : RepeatController
    {
        //
        // GET: /Invoice/

        public ActionResult Index()
        {
            return View();
        }

    }
}
