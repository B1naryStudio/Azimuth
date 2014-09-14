using Azimuth.DataAccess.Infrastructure;

namespace Azimuth.DataAccess.Entities
{
    public class PlaylistLike:BaseEntity
    {
        public virtual Playlist Playlist { get; set; }
        public virtual User Liker { get; set; }
        public virtual bool IsLiked { get; set; }
        public virtual bool IsFavorite { get; set; }
    }
}
