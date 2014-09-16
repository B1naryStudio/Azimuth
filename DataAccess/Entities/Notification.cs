using System;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.Shared.Enums;
using NHibernate.Type;

namespace Azimuth.DataAccess.Entities
{
    public class Notification : BaseEntity
    {
        public virtual User User { get; set; }

        public virtual Notifications NotificationType { get; set; }

        public virtual User RecentlyUser { get; set; }

        public virtual Playlist RecentlyPlaylist { get; set; }
        public virtual DateTime NotificationDate { get; set; }
    }

    public class NotificationType : EnumStringType<Notifications>
    {
        
    }
}