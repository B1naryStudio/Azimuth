using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;

namespace Azimuth.Infrastructure
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