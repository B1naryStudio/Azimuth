using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;

namespace Azimuth.Infrastructure
{
    public interface IDataService
    {
        User GetUserInfo();
    }
}