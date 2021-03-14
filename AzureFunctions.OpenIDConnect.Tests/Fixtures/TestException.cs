using System;

namespace AzureFunctions.OpenIDConnect.Tests.Fixtures
{
    public class TestException : Exception
    {
        public TestException()
        {
        }

        public TestException(string message)
            : base(message)
        {
        }
    }
}
