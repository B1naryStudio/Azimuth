using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;
using Azimuth.Shared.Enums;

namespace Azimuth.Services.Concrete
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationRepository _notificationRepository; 

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _notificationRepository = _unitOfWork.GetRepository<Notification>() as NotificationRepository;
        }
        public Task CreateNotification(Notifications type, User user, User recentlyUser, Playlist recentlyPlaylist)
        {
            return Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var notification = new Notification
                    {
                        NotificationType = type,
                        User = user,
                        RecentlyUser = recentlyUser,
                        RecentlyPlaylist = recentlyPlaylist
                    };

                    _notificationRepository.AddItem(notification);

                    _unitOfWork.Commit();
                }
            });
        }

        public Task<List<NotificationDto>> GetRecentActivity(int userId)
        {
            return Task.Run(() =>
            {
                var notificationsDto = new List<NotificationDto>();
                using (_unitOfWork)
                {
                    var notifications = _notificationRepository.GetAll(n => n.User.Id == userId);

                    notificationsDto.AddRange(notifications.Select(notification => new NotificationDto
                    {
                        RecentlyPlaylistId = (notification.RecentlyPlaylist != null) ? notification.RecentlyPlaylist.Id : -1,
                        RecentlyUserId = (notification.RecentlyUser != null) ? notification.RecentlyUser.Id : -1,
                        Message = GetMessage(notification)
                    }));
                }

                return notificationsDto;
            });
        }

        private string GetMessage(Notification notification)
        {
            string message = "";
            switch(notification.NotificationType)
            {
                case Notifications.AddedNewListener:
                    break;
                case Notifications.ChangedPlaylistAccessebilty:
                    message = string.Format("User {0} {1} changed accessebilty for playlist {2}",
                        notification.User.Name.FirstName,
                        notification.User.Name.LastName,
                        notification.RecentlyPlaylist.Name);
                    break;
                case Notifications.FavoritedPlaylist:
                    message = string.Format("User {0} {1} favorited playlist {2}",
                        notification.User.Name.FirstName,
                        notification.User.Name.LastName,
                        notification.RecentlyPlaylist.Name);
                    break;
                case Notifications.Followed:
                    message = string.Format("User {0} {1} followed to {2} {3}",
                        notification.User.Name.FirstName,
                        notification.User.Name.LastName,
                        notification.RecentlyUser.Name.FirstName,
                        notification.RecentlyUser.Name.LastName);
                    break;
                case Notifications.LikedPlaylist:
                    message = string.Format("User {0} {1} liked playlist {2}",
                        notification.User.Name.FirstName,
                        notification.User.Name.LastName,
                        notification.RecentlyPlaylist.Name);
                    break;
                case Notifications.PlaylistCreated:
                    message = string.Format("User {0} {1} created new playlist {2}",
                        notification.User.Name.FirstName,
                        notification.User.Name.LastName,
                        notification.RecentlyPlaylist.Name);
                    break;
                case Notifications.PlaylistRemoved:
                    message = string.Format("User {0} {1} removed playlist",
                        notification.User.Name.FirstName,
                        notification.User.Name.LastName);
                    break;
                case Notifications.PlaylistShared:
                    message = string.Format("User {0} {1} shared playlist {2}",
                        notification.User.Name.FirstName,
                        notification.User.Name.LastName,
                        notification.RecentlyPlaylist.Name);
                    break;
                case Notifications.RemovedListener:
                    break;
                case Notifications.UnfavoritedPlaylist:
                    message = string.Format("User {0} {1} unfavorited playlist {2}",
                        notification.User.Name.FirstName,
                        notification.User.Name.LastName,
                        notification.RecentlyPlaylist.Name);
                    break;
                case Notifications.Unfollowed:
                    message = string.Format("User {0} {1} unfollowed to {2} {3}",
                        notification.User.Name.FirstName,
                        notification.User.Name.LastName,
                        notification.RecentlyUser.Name.FirstName,
                        notification.RecentlyUser.Name.LastName);
                    break;
                case Notifications.UnlikedPlaylist:
                    message = string.Format("User {0} {1} unliked playlist {2}",
                        notification.User.Name.FirstName,
                        notification.User.Name.LastName,
                        notification.RecentlyPlaylist.Name);
                    break;
            }

            return message;
        }
    }
}