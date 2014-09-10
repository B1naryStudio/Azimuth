using System.Collections.Generic;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Enums;

namespace Azimuth.Shared.Dto
{
    public class PlaylistData
    {
        public long Id { get; set; }
        public string Name;
        public Accessibilty Accessibilty;
        public List<string> TrackIds;
        public List<string> Genres;
        public UserBrief Creator { get; set; }
        public int Duration;
        public int ItemsCount;
        public int PlaylistListened { get; set; }
        public int PlaylistLikes { get; set; }
        public int PlaylistFavourited { get; set; }



        //public ICollection<PlaylistListener> PlaylistListeners { get; set; }
        //public ICollection<PlaylistLike> PlaylistLikes { get; set; }

        //public PlaylistData()
        //{
        //    PlaylistListeners = new List<PlaylistListener>();
        //    PlaylistLikes = new List<PlaylistLike>();
        //}
    }
}
