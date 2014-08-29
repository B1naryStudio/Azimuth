
using Azimuth.DataAccess.Infrastructure;

namespace Azimuth.DataAccess.Entities
{
    public class PlaylistTrack: BaseEntity
    {
        public virtual PlaylistTracksIdentifier Identifier { get; set; }
        public virtual int TrackPosition { get; set; }
    }

    public class PlaylistTracksIdentifier
    {
        public virtual  Playlist Playlist { get; set; }
        public virtual Track Track { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var id = (PlaylistTracksIdentifier)obj;
            if (Playlist == id.Playlist && Track == id.Track)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (Playlist + "|" + Track).GetHashCode();
        }
    }
}
