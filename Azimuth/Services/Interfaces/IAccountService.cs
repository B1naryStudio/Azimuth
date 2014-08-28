using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure.Concrete;

namespace Azimuth.Services.Interfaces
{
    public interface IAccountService
    {
        bool SaveOrUpdateUserData(User user, UserCredential userCredential, AzimuthIdentity loggedIdentity);
        bool DisconnectUserAccount(string provider);
    }
}