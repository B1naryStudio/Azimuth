using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;
using Azimuth.Shared.Enums;

namespace Azimuth.Services.Interfaces
{
    public interface INotificationService
    {
        Notification CreateNotification(Notifications type, User user, User recentlyUser = null, Playlist recentlyPlaylist = null);
        Task<List<NotificationDto>> GetRecentActivity(long userId);
        Task<List<NotificationDto>> GetFollowingsActivity(long userId);
    }
}