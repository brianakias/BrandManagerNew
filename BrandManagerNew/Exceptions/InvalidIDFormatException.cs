using System;

namespace BrandManagerNew
{
    public class InvalidIDFormatException : Exception
    {
        public InvalidIDFormatException(string message) : base(message)
        {

        }
    }
}
