using System;

namespace Azimuth.Infrastructure.Exceptions
{
    public class VkApiException : Exception
    {
        private int _code;

        public VkApiException(string message, int code)
            :base(message)
        {
            _code = code;
        }
    }
}