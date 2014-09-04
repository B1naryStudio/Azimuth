using System.Collections.Generic;

namespace Azimuth.Shared.Dto
{
    public class TrackInfoDto
    {
        public string Title { get; set; }
        public int TrackRank { get; set; }
        public string TrackLastfmUrl { get; set; }
        public string TrackDeezerUrl { get; set; }
        public string Artist { get; set; }
        public string ArtistImage { get; set; }
        public string ArtistTopTracksUrl { get; set; }
        public List<TrackSearchInfo.SearchData> ArtistTopTrackList { get; set; }
        public int ArtistFans { get; set; }
        public string ArtistLastfmUrl { get; set; }
        public string AlbumTitle { get; set; }
        public string AlbumCover { get; set; }
        public string AlbumRelease { get; set; }
        public int AlbumRank { get; set; }
        public int AlbumFans { get; set; }
        public string AlbumLastfmUrl { get; set; }
        public List<string> Genres { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Published { get; set; }
        public string[]  Lyric { get; set; }

        public TrackInfoDto()
        {
            Genres = new List<string>();
            ArtistTopTrackList = new List<TrackSearchInfo.SearchData>();
        }
    }
}