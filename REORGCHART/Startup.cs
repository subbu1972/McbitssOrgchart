using Microsoft.Owin;
using Owin;
using System.Linq;
using System.Web;
using System.Configuration;

[assembly: OwinStartupAttribute(typeof(REORGCHART.Startup))]
namespace REORGCHART
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Models.DBContext db = new Models.DBContext();
            string wcCompanyName = ConfigurationManager.AppSettings["wcCompanyName"].ToString();
            var INI = (from ini in db.InitializeTables
                       where ini.Authentication.ToUpper() == "WINDOWS" && ini.CompanyName == wcCompanyName
                       select ini).FirstOrDefault();
            if (INI != null)
            {
            }
            else ConfigureAuth(app);
        }
    }
}
