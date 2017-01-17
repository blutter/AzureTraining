using System.Web.Http;
using System.Web.Routing;

namespace AzureGuestbook.Server
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register();
        }
    }
}