using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Parameters;

namespace Azimuth.Infrastructure
{
    public static class AccountProviderFactory
    {
        private static IKernel _kernel;

        static AccountProviderFactory()
        {
            _kernel = new StandardKernel(new AccountProviderModule());
        }

        public static IAccountProvider GetAccountProvider(string socialNetwork, string userId, string accessToken)
        {
            var userIdParam = new ConstructorArgument("userId", userId);
            var accessTokenParam = new ConstructorArgument("accessToken", accessToken);

            return _kernel.Get<IAccountProvider>(socialNetwork, userIdParam, accessTokenParam);
        }
    }
}