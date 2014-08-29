using System.Security.Claims;
using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure.Concrete;
using Microsoft.Owin.Security;

namespace Azimuth.Services.Interfaces
{
    public interface IAccountService
    {
        bool SaveOrUpdateUserData(User user, UserCredential userCredential, AzimuthIdentity loggedIdentity);
        bool DisconnectUserAccount(string provider);
        void SignIn(AzimuthIdentity identity, User userInfo);
        void SignOut();
        ClaimsAuthenticationManager ClaimsAuthenticationManager { get; }
        IAuthenticationManager AuthenticationManager { get; }
    }
}