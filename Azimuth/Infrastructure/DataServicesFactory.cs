namespace Azimuth.Infrastructure
{
    public class DataServicesFactory
    {
        public static IDataService GetService(string providerName, string userId, string accessToken = "")
        {
            switch (providerName)
            {
                case "Vkontakte":
                    return new VkDataService(userId, accessToken);
                case "Facebook":
                    return new FacebookDataService(accessToken);
                default:
                    return null;
            }
        }
    }
}