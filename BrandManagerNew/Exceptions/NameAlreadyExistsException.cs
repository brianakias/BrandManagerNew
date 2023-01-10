using System;

namespace BrandManagerNew
{
    public class NameAlreadyExistsException : Exception
    {
        public NameAlreadyExistsException(string message) : base(message)
        {

        }
    }
}
