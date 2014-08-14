using System;

namespace Azimuth.Infrastructure.Exceptions
{
    public class ApplicationDisabledException : VkApiException
    {
        public ApplicationDisabledException(string message, int code)
            :base(message, code)
        {
        }
    }
}