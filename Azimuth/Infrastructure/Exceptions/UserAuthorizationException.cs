namespace Azimuth.Infrastructure.Exceptions
{
    public class UserAuthorizationException : VkApiException
    {
        public UserAuthorizationException(string message, int code)
            :base(message, code)
        {
        }
    }
}