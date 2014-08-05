namespace Azimuth.Infrastructure
{
    public class AccountProviderFactory
    {
        public static IAccountProvider GetService(string providerName, string userId, string accessToken = "")
        {
            switch (providerName)
            {
                case "Vkontakte":
                    return new VKAccountProvider(userId, accessToken);
                case "Facebook":
                    return new FacebookAccountProvider(accessToken);
                default:
                    return null;
            }
        }
    }
}