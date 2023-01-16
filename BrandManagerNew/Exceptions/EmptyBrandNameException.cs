using System;

namespace BrandManagerNew.Exceptions
{
    public class EmptyBrandNameException : Exception
    {
        public EmptyBrandNameException(string message) : base(message)
        {

        }

    }
}
