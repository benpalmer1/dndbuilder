using System.Web;
using System.Web.Http;
using TMWeb;

namespace DndBuilder
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
