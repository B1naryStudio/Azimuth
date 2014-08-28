using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;
using Azimuth.Infrastructure.Interfaces;

namespace Azimuth.DataProviders.Concrete
{
    public abstract class AccountProvider : IAccountProvider
    {
        protected IWebClient _webClient;
        public abstract Task<User> GetUserInfoAsync(string email = "");

        protected AccountProvider(IWebClient webClient)
        {
            _webClient = webClient;
        }
    }
}