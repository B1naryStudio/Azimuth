using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Azimuth.Shared.Dto;
using Ninject.Modules;

namespace Azimuth.Infrastructure
{
    public class AccountProviderModule:NinjectModule
    {
        public override void Load()
        {
            Bind<IAccountProvider>().To<FacebookAccountProvider>().Named("Facebook");
            Bind<IAccountProvider>().To<VKAccountProvider>().Named("Vkontakte");
            Bind<IAccountProvider>().To<GoogleAccountProvider>().Named("Google");
        }
    }
}