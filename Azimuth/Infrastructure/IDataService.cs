using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;

namespace Azimuth.Infrastructure
{
    public interface IDataService
    {
        Task<User> GetUserInfoAsync();
    }
}