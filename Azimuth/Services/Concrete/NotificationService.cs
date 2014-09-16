using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Hubs.Concrete;
using Azimuth.Hubs.Interfaces;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;
using Azimuth.Shared.Enums;

namespace Azimuth.Services.Concrete
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly INotificationsHub _notificationHub;

        public NotificationService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;

            _notificationHub = NotificationsHub.Instance;
        }
        public Notification CreateNotification(Notifications type, User user, User recentlyUser, Playlist recentlyPlaylist)
        {
            var notification = new Notification
            {
                NotificationType = type,
                User = user,
                RecentlyUser = recentlyUser,
                RecentlyPlaylist = recentlyPlaylist
            };

            var notificationDto = new NotificationDto();
            Mapper.Map(notification, notificationDto);
            notificationDto.Message = GetMessage(notification);

            _notificationHub.SendNotification(user.Id, notificationDto);

            return notification;
        }

        public Task<List<NotificationDto>> GetRecentActivity(long userId)
        {
            return Task.Run(() =>
            {
                var notificationsDto = new List<NotificationDto>();
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var _notificationRepository = unitOfWork.GetRepository<Notification>();

                    //TODO: move ToList() to the end of the expression
                    var notifications = _notificationRepository.GetAll(n => n.User.Id == userId).ToList().OrderByDescending(s => s.Id).Take(15);

                    foreach (var notification in notifications)
                    {
                        var notifDto = new NotificationDto
                        {
                            UserFirstName = notification.User.Name.FirstName,
                            UserLastName = notification.User.Name.LastName,
                            Message = GetMessage(notification)
                        };
                        if (notification.RecentlyUser != null)
                        {
                            notifDto.RecentlyUserId = notification.RecentlyUser.Id;
                            notifDto.RecentlyUserFirstName = notification.RecentlyUser.Name.FirstName;
                            notifDto.RecentlyUserLastName = notification.RecentlyUser.Name.LastName;
                        }
                        if (notification.RecentlyPlaylist != null)
                        {
                            if (notification.RecentlyPlaylist.Accessibilty != Accessibilty.Private)
                            {
                                notifDto.RecentlyPlaylistId = notification.RecentlyPlaylist.Id;
                            }
                            notifDto.RecentlyPlaylistName = notification.RecentlyPlaylist.Name;
                        }

                        notificationsDto.Add(notifDto);
                    }
                }

                return notificationsDto;
            });
        }

        public async Task<List<NotificationDto>> GetFollowingsActivity(long userId)
        {
            var notifications = new List<NotificationDto>();
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var user = unitOfWork.UserRepository.GetOne(u => u.Id == userId);

                foreach (var following in user.Following)
                {
                    notifications.AddRange(await GetRecentActivity(following.Id));
                }
            }

            return notifications;
        }

        private string GetMessage(Notification notification)
        {
            string message = "";
            switch (notification.NotificationType)
            {
                case Notifications.AddedNewListener:
                    break;
                case Notifications.ChangedPlaylistAccessebilty:
                    message = "changed accessebilty for playlist";
                    break;
                case Notifications.FavoritedPlaylist:
                    message = "favorited playlist ";
                    break;
                case Notifications.Followed:
                    message = "followed to";
                    break;
                case Notifications.LikedPlaylist:
                    message = "liked playlist";
                    break;
                case Notifications.PlaylistCreated:
                    message = "created new playlist";
                    break;
                case Notifications.RemovedTracks:
                    message = "removed tracks from";
                    break;
                case Notifications.PlaylistShared:
                    message = "shared playlist";
                    break;
                case Notifications.RemovedListener:
                    break;
                case Notifications.UnfavoritedPlaylist:
                    message = "unfavorited playlist";
                    break;
                case Notifications.Unfollowed:
                    message = "unfollowed to";
                    break;
                case Notifications.UnlikedPlaylist:
                    message = "unliked playlist";
                    break;
            }

            return message;
        }
    }
}