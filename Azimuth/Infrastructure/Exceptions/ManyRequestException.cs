using System;

namespace Azimuth.Infrastructure.Exceptions
{
    public class ManyRequestException : VkApiException
    {
        public ManyRequestException(string message, int code)
            :base(message, code)
        {
        }
    }
}