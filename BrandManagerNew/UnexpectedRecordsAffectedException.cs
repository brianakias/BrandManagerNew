using System;

namespace BrandManagerNew
{
    public class UnexpectedRecordsAffectedException : Exception
    {
        public UnexpectedRecordsAffectedException(string message) : base(message)
        {

        }
    }
}
