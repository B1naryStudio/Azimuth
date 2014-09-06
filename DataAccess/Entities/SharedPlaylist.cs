using Azimuth.DataAccess.Infrastructure;

namespace Azimuth.DataAccess.Entities
{
    public class SharedPlaylist : BaseEntity
    {
        public virtual string Guid { get; set; }
         public virtual Playlist Playlist { get; set; }
    }
}