using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;

namespace Azimuth.DataProviders.Interfaces
{
    public interface IAccountProvider
    {
        Task<User> GetUserInfoAsync(string email = "");
    }
}