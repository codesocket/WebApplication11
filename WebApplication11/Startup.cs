using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Microsoft.Owin;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.WebApi;
using Owin;
using WebApplication11.App_Start;

[assembly: OwinStartup(typeof(WebApplication11.Startup))]

namespace WebApplication11
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var container = UnityConfig.GetConfiguredContainer();

            config.DependencyResolver = new UnityHierarchicalDependencyResolver(container);
            config.MessageHandlers.Add(new ContextBuilderHandler());
            config.Services.Add(typeof(IExceptionLogger), new NLogExceptionLogger());

            app.UseWebApi(config);
        }        
    }

    public class MyContext
    {
        public Uri RequestUri { get; set; }
    }

    public class ContextBuilderHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var ctx = new MyContext
            {
                RequestUri = request.RequestUri
            };

            throw new Exception("oops");

            var container = request.GetDependencyScope().GetService(typeof(IUnityContainer)) as IUnityContainer;

            container.RegisterInstance(ctx);

            return base.SendAsync(request, cancellationToken);
        }
    }

}
