using Azimuth.DataAccess.Infrastructure;

namespace Azimuth.DataAccess.Entities
{
    public class PlaylistLike:BaseEntity
    {
        public virtual Playlist Playlist { get; set; }
        public virtual User Liker { get; set; }
        public virtual bool IsLiked { get; set; }
        public virtual bool IsFavorite { get; set; }

        //protected bool isProxy
        //{
        //    get
        //    {
        //        return (Id == 0 && Playlist == null &&
        //                Liker == null);
        //    }
        //}

        //public override bool Equals(object obj)
        //{
        //    return PlaylistEquals(obj as PlaylistLike);
        //}

        //protected bool PlaylistEquals(PlaylistLike playlistLike)
        //{
        //    if (playlistLike == null || !GetType().IsInstanceOfType(playlistLike))
        //    {
        //        return false;
        //    }
        //    else if (isProxy ^ playlistLike.IsProxy())
        //    {
        //        return false;
        //    }
        //    else if (isProxy && playlistLike.isProxy)
        //    {
        //        return ReferenceEquals(this, playlistLike);
        //    }
        //    else
        //    {
        //        return (Id == playlistLike.Id && Liker == playlistLike.Liker && IsFavorite == playlistLike.IsFavorite && IsLiked == playlistLike.IsLiked);
        //    }
        //}

        //private int? _cachedHashCode;

        //public override int GetHashCode()
        //{
        //    if (_cachedHashCode.HasValue) return _cachedHashCode.Value;

        //    _cachedHashCode = isProxy ? base.GetHashCode() : Id.GetHashCode();
        //    return _cachedHashCode.Value;
        //}

        //public static bool operator ==(PlaylistLike x, PlaylistLike y)
        //{
        //    // By default, == and Equals compares references. In order to 
        //    // maintain these semantics with entities, we need to compare by 
        //    // identity value. The Equals(x, y) override is used to guard 
        //    // against null values; it then calls EntityEquals().
        //    return Object.Equals(x, y);
        //}

        //// Maintain inequality operator semantics for entities. 
        //public static bool operator !=(PlaylistLike x, PlaylistLike y)
        //{
        //    return !(x == y);
        //}
    }
}
