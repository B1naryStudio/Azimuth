using System;

namespace Azimuth.Infrastructure.Exceptions
{
    public class EndUserException : Exception
    {
        public EndUserException(string message) : base(message)
        {
        }
    }
}