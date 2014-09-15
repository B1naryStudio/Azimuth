
namespace Azimuth.Shared.Dto
{
    public class PublicPlaylistInfo
    {
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; }
        public bool IsLiked { get; set; }
        public bool IsFavourited { get; set; }
        public int? LikesCount { get; set; }
        public int? FavouritesCount { get; set; }

    }
}
