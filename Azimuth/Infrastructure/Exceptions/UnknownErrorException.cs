namespace Azimuth.Infrastructure.Exceptions
{
    public class UnknownErrorException : VkApiException
    {
        public UnknownErrorException(string message, int code)
            :base(message, code)
        {
        }
    }
}