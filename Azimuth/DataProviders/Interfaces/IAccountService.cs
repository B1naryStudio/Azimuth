using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure;

namespace Azimuth.DataProviders.Interfaces
{
    public interface IAccountService
    {
        bool SaveOrUpdateUserData(User user, UserCredential userCredential);
    }
}