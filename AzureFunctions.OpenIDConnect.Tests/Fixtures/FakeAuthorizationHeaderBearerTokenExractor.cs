using AzureFunctions.OpenIDConnect.Abstractions;
using Microsoft.AspNetCore.Http;

namespace AzureFunctions.OpenIDConnect.Tests.Fixtures
{
    public class FakeAuthorizationHeaderBearerTokenExractor : IAuthorizationHeaderBearerTokenExtractor
    {
        public string TokenToReturn { get; set; }

        public string GetToken(IHeaderDictionary httpRequestHeaders)
        {
            return TokenToReturn;
        }
    }
}
