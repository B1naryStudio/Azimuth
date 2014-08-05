using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;

namespace Azimuth.Infrastructure
{
    public interface IAccountProvider
    {
        Task<User> GetUserInfoAsync(string email = "");
    }
}