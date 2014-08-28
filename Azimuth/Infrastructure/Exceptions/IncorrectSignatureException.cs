namespace Azimuth.Infrastructure.Exceptions
{
    public class IncorrectSignatureException : VkApiException
    {
        public IncorrectSignatureException(string message, int code)
            :base(message, code)
        {
        }
    }
}