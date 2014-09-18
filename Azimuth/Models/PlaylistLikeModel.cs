namespace Azimuth.Models
{
    public class PlaylistLikeModel
    {
        public string Name { get; set; }

        public long Id { get; set; }
        public bool IsFavorite { get; set; }

        public bool IsLiked { get; set; }
    }
}