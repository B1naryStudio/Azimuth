using System;
using System.Collections.Generic;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.Shared.Enums;
using NHibernate.Proxy;
using NHibernate.Type;

namespace Azimuth.DataAccess.Entities
{
    public class Playlist : BaseEntity
    {
        public virtual string Name { get; set; }

        public virtual Accessibilty Accessibilty { get; set; }

        public virtual User Creator { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
        public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; }
        public virtual ICollection<PlaylistListener> PlaylistListeners { get; set; }
        public virtual ICollection<PlaylistLike> PlaylistLikes { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual int Listened { get; set; }

        public Playlist()
        {
            Tracks = new List<Track>();
            PlaylistTracks = new List<PlaylistTrack>();
            PlaylistListeners = new List<PlaylistListener>();
            Notifications = new List<Notification>();
        }

        //protected bool isProxy
        //{
        //    get
        //    {
        //        return (Id == 0 && Name == null &&
        //            Creator == null && Tracks == null &&
        //            PlaylistTracks == null &&
        //            PlaylistListeners == null &&
        //            PlaylistLikes == null && Notifications == null &&
        //            Listened == 0);
        //    }
        //}

        //public override bool Equals(object obj)
        //{
        //    return PlaylistEquals(obj as Playlist);
        //}

        //protected bool PlaylistEquals(Playlist playlist)
        //{
        //    if (playlist == null || !GetType().IsInstanceOfType(playlist))
        //    {
        //        return false;
        //    }
        //    else if (isProxy ^ playlist.IsProxy())
        //    {
        //        return false;
        //    }
        //    else if (isProxy && playlist.isProxy)
        //    {
        //        return ReferenceEquals(this, playlist);
        //    }
        //    else
        //    {
        //        return (Id == playlist.Id && Name == playlist.Name && Accessibilty == playlist.Accessibilty &&
        //                Creator == playlist.Creator && Tracks.Equals(playlist.Tracks) &&
        //                PlaylistTracks.Equals(playlist.PlaylistTracks) &&
        //                PlaylistListeners.Equals(playlist.PlaylistListeners) &&
        //                PlaylistLikes.Equals(playlist.PlaylistLikes) && Notifications.Equals(playlist.Notifications) &&
        //                Listened == playlist.Listened);
        //    }
        //}

        //private int? _cachedHashCode;

        //public override int GetHashCode()
        //{
        //    if (_cachedHashCode.HasValue) return _cachedHashCode.Value;

        //    _cachedHashCode = isProxy ? base.GetHashCode() : Id.GetHashCode();
        //    return _cachedHashCode.Value;
        //}

        //public static bool operator ==(Playlist x, Playlist y)
        //{
        //    // By default, == and Equals compares references. In order to 
        //    // maintain these semantics with entities, we need to compare by 
        //    // identity value. The Equals(x, y) override is used to guard 
        //    // against null values; it then calls EntityEquals().
        //    return Object.Equals(x, y);
        //}

        //// Maintain inequality operator semantics for entities. 
        //public static bool operator !=(Playlist x, Playlist y)
        //{
        //    return !(x == y);
        //}
    }

    public class AccessibiltyType : EnumStringType<Accessibilty>
    {
    }
}