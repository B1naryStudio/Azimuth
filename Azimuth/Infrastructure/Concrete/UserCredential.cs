namespace Azimuth.Infrastructure.Concrete
{
    public class UserCredential
    {
        public string SocialNetworkId { get; set; }

        public string SocialNetworkName { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }

        public string AccessTokenExpiresIn { get; set; }

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string Email { get; set; }
    }
}