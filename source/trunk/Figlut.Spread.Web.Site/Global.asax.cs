namespace Figlut.Spread.Web.Site
{
    #region Using Directives

    using Figlut.Server.Toolkit.Utilities;
    using Figlut.Server.Toolkit.Utilities.Logging;
    using Figlut.Spread.Web.Site.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.WebPages;

    #endregion //Using Directives

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        #region Methods

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            //Remove WebForms view engine.
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif

            SpreadWebApp.Instance.Initialize();

            GOC.Instance.Logger.LogMessage(new
                LogMessage("Application_Start executed", LogMessageType.Information, LoggingLevel.Normal));

            System.Reflection.PropertyInfo p = typeof(System.Web.HttpRuntime).GetProperty("FileChangesMonitor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            object o = p.GetValue(null, null);
            System.Reflection.FieldInfo f = o.GetType().GetField("_dirMonSubdirs", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase);
            object monitor = f.GetValue(o);
            System.Reflection.MethodInfo m = monitor.GetType().GetMethod("StopMonitoring", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic); m.Invoke(monitor, new object[] { });
        }

        /// <summary>
        /// http://www.codeproject.com/Articles/455627/MVC-Mobile-Friendly-Web-Applications
        /// </summary>
        private void EvaluateDisplayMode()
        {
            DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("Phone")
            {  //...modify file (view that is served)
                //Query condition
                ContextCondition = (ctx => (
                    //look at user agent
                        (ctx.GetOverriddenUserAgent() != null) &&
                        (//...either iPhone or iPad
                            (ctx.GetOverriddenUserAgent().IndexOf("iPhone", StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (ctx.GetOverriddenUserAgent().IndexOf("iPod", StringComparison.OrdinalIgnoreCase) >= 0)
                        )
                    ))
            });
            DisplayModeProvider.Instance.Modes.Insert(0,
                new DefaultDisplayMode("Tablet")
                {
                    ContextCondition = (ctx => (
                        (ctx.GetOverriddenUserAgent() != null) &&
                        (
                            (ctx.GetOverriddenUserAgent().IndexOf("iPad", StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (ctx.GetOverriddenUserAgent().IndexOf("Playbook", StringComparison.OrdinalIgnoreCase) >= 0)
                        )
                ))
                });
        }

        #endregion //Methods
    }
}