using System;

namespace Azimuth.Infrastructure.Exceptions
{
    public class BadParametersException : VkApiException
    {
        public BadParametersException(string message, int code)
            :base(message, code)
        {
        }
    }
}