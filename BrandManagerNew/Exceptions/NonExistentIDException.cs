using System;

namespace BrandManagerNew
{
    public class NonExistentIDException : Exception
    {
        public NonExistentIDException(string message) : base(message)
        {

        }
    }
}
