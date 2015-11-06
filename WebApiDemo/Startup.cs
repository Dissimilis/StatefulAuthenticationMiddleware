using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Microsoft.Owin.Host.HttpListener;
using Newtonsoft.Json;
using Owin;
using OwinStatefulAuthentication;

namespace WebApiDemo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            ); 

            app.UseStatefullAuthentication(Program.GetUser);

            app.UseWebApi(config);
        }
    }
}