using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure;

namespace Azimuth.Services
{
    public interface IAccountService
    {
        bool SaveOrUpdateUserData(User user, UserCredential userCredential);
    }
}