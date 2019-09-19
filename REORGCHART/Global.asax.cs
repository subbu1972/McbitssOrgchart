using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace REORGCHART
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ceTe.DynamicPDF.Document.AddLicense("GEN80NPSAOKGNLa2QVYhum3BNkocrDErhYTcxogGMCxGtGQPKVKxHcymXP2ZBU19ym1wLavPCEdQX/3wKDuZq9eLd87LChuSSyMg");

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();
        }

        void CallbackError(object sender, EventArgs e)
        {
            // Logging exceptions occur on callback events of DevExpress ASP.NET MVC controls. 
            // To learn more, see http://www.devexpress.com/Support/Center/Example/Details/E4588
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = System.Web.HttpContext.Current.Server.GetLastError();
            //TODO: Handle Exception
        }
    }
}
