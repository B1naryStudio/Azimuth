using System;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Enums;

namespace Azimuth.Shared.Dto
{
    public class NotificationDto
    {
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string RecentlyUserFirstName { get; set; }
        public string RecentlyUserLastName { get; set; }
        public string RecentlyPlaylistName { get; set; }
        public long RecentlyPlaylistId { get; set; }
        public long RecentlyUserId { get; set; }
        public long UserId { get; set; }
        public string Message { get; set; }
        public Notifications NotificationType { get; set; }
        public string UserPhoto { get; set; }
        public string DateTime { get; set; }
    }
}