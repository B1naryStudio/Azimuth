using System;

namespace Azimuth.Exceptions
{
    public class EndUserException : Exception
    {
        public EndUserException(string message) : base(message)
        {
        }
    }
}