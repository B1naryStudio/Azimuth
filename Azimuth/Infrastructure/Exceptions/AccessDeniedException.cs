
namespace Azimuth.Infrastructure.Exceptions
{
    public class AccessDeniedException: VkApiException
    {
        public AccessDeniedException(string message, int code) :base(message, code)
        { }
    }
}