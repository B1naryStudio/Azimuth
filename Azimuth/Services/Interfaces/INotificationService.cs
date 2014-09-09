using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Enums;

namespace Azimuth.Services.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotification(Notifications type, User user);
    }
}