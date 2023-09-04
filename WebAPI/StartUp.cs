using Owin;
using System.Web.Http;



/*
 * Web API Start up
 * 
 */
namespace Visualizations
{
    namespace WebAPI
    {
        public class StartUp
        {

            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// This code configures Web API.
            /// The Startup class is specified as a type parameter in the WebApp.Start method.
            /// </summary>
            /// <param name="appBuilder">The app builder.</param>
            public void Configuration(IAppBuilder appBuilder)
            {
                // Configure Web API for self-host. 
                HttpConfiguration config = new HttpConfiguration();
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

                appBuilder.UseWebApi(config);
            }
        }
    }
}
