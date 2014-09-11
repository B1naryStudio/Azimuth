﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Hubs;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;
using Azimuth.Shared.Enums;

namespace Azimuth.Services.Concrete
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationRepository _notificationRepository;
        private readonly UserRepository _userRepository;
        public NotificationsHub NotificationsHub { get; set; }

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _notificationRepository = _unitOfWork.GetRepository<Notification>() as NotificationRepository;
            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
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
                    NotificationsHub.SendMessage(user.Id, notification);
                }
            });
        }

        public Task<List<NotificationDto>> GetRecentActivity(long userId)
        {
            return Task.Run(() =>
            {
                var notificationsDto = new List<NotificationDto>();
                using (_unitOfWork)
                {
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
                            if (notification.RecentlyPlaylist.Accessibilty == Accessibilty.Public)
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
            var user = _userRepository.GetOne(u => u.Id == userId);

            foreach (var following in user.Following)
            {
                notifications.AddRange(await GetRecentActivity(following.Id));
            }

            return notifications;
        }

        private string GetMessage(Notification notification)
        {
            string message = "";
            switch(notification.NotificationType)
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
                case Notifications.PlaylistRemoved:
                    message = "removed playlist";
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