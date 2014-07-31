using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Azimuth.ApiControllers;
using Azimuth.DataAccess.Infrastructure;
using Ninject;

namespace Azimuth
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new NinjectControllerActivator());
        }


    }

    internal class NinjectControllerActivator : IHttpControllerActivator
    {
        private readonly IKernel _container;
        public NinjectControllerActivator()
        {
            _container = new StandardKernel(new DataAccessModule());
            _container.Bind<UserController>().ToSelf();
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            return (IHttpController)_container.Get(controllerType);
        }
    }
}
