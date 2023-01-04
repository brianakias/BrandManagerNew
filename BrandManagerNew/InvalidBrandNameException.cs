using System;

namespace BrandManagerNew
{
    public class InvalidBrandNameException : Exception
    {
        public InvalidBrandNameException(string message) : base(message)
        {

        }
    }
}
