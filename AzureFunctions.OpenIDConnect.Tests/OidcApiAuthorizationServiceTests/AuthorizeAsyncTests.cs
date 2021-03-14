namespace AzureFunctions.OpenIDConnect.Tests.OidcApiAuthorizationServiceTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AzureFunctions.OpenIDConnect.Models;
    using AzureFunctions.OpenIDConnect.Tests.Fixtures;
    using Microsoft.AspNetCore.Http;
    using Microsoft.IdentityModel.Tokens;
    using Xunit;

    public class AuthorizeAsyncTests
    {
        [Fact]
        public static async Task Retrys_once_if_SecurityTokenSignatureKeyNotFoundException()
        {
            const string audienceForTest = "AudienceForTest";
            const string issuerUrlForTest = "https://issuerUrl.for.test/";
            const string extractedTokenForTest = "ExtractedTokenForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = audienceForTest,
                        IssuerUrl = issuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenExractor = new FakeAuthorizationHeaderBearerTokenExractor()
            {
                TokenToReturn = extractedTokenForTest
            };

            var fakeJwtSecurityTokenHandlerWrapper = new FakeJwtSecurityTokenHandlerWrapper()
            {
                ThrowFirstTime = true
            };

            var fakeOidcConfigurationManager = new FakeOidcConfigurationManager()
            {
                SecurityKeysForTest = new List<SecurityKey>()
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenExractor,
                fakeJwtSecurityTokenHandlerWrapper,
                fakeOidcConfigurationManager);

            var result = await service.AuthorizeAsync(
                httpRequestHeaders);

            Assert.True(result.Success);

            Assert.Equal(2, fakeJwtSecurityTokenHandlerWrapper.ValidateTokenCalledCount);

            Assert.Equal(1, fakeOidcConfigurationManager.RequestRefreshCalledCount);
        }

        [Fact]
        public async Task Returns_failure_if_SecurityTokenSignatureKeyNotFoundException_on_retry()
        {
            const string audienceForTest = "AudienceForTest";
            const string issuerUrlForTest = "https://issuerUrl.for.test/";
            const string extractedTokenForTest = "ExtractedTokenForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = audienceForTest,
                        IssuerUrl = issuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenExractor = new FakeAuthorizationHeaderBearerTokenExractor()
            {
                TokenToReturn = extractedTokenForTest
            };

            var fakeJwtSecurityTokenHandlerWrapper = new FakeJwtSecurityTokenHandlerWrapper()
            {
                ThrowFirstTime = true,
                ThrowSecondTime = true
            };

            var fakeOidcConfigurationManager = new FakeOidcConfigurationManager()
            {
                SecurityKeysForTest = new List<SecurityKey>()
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenExractor,
                fakeJwtSecurityTokenHandlerWrapper,
                fakeOidcConfigurationManager);

            var result = await service.AuthorizeAsync(httpRequestHeaders);

            Assert.False(result.Success);

            Assert.Equal(2, fakeJwtSecurityTokenHandlerWrapper.ValidateTokenCalledCount);

            Assert.Equal(1, fakeOidcConfigurationManager.RequestRefreshCalledCount);
        }

        [Fact]
        public async Task Returns_failure_for_unauthorized_token()
        {
            const string audienceForTest = "AudienceForTest";
            const string issuerUrlForTest = "https://issuerUrl.for.test/";
            const string extractedTokenForTest = "ExtractedTokenForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = audienceForTest,
                        IssuerUrl = issuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenExractor = new FakeAuthorizationHeaderBearerTokenExractor()
            {
                TokenToReturn = extractedTokenForTest
            };

            var fakeJwtSecurityTokenHandlerWrapper = new FakeJwtSecurityTokenHandlerWrapper()
            {
                // Normally a SecurityTokenException will be thrown when the token is not authorized.
                ExceptionToThrow = new SecurityTokenException()
            };

            var fakeOidcConfigurationManager = new FakeOidcConfigurationManager()
            {
                SecurityKeysForTest = new List<SecurityKey>()
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenExractor,
                fakeJwtSecurityTokenHandlerWrapper,
                fakeOidcConfigurationManager);

            var result = await service.AuthorizeAsync(
                httpRequestHeaders);

            Assert.True(result.Failed);

            Assert.Equal(1, fakeJwtSecurityTokenHandlerWrapper.ValidateTokenCalledCount);

            Assert.Equal(0, fakeOidcConfigurationManager.RequestRefreshCalledCount);
        }

        [Fact]
        public async Task Returns_failure_if_bad_Aurthorization_header()
        {
            const string audienceForTest = "AudienceForTest";
            const string issuerUrlForTest = "https://issuerUrl.for.test/";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = audienceForTest,
                        IssuerUrl = issuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenExractor = new FakeAuthorizationHeaderBearerTokenExractor()
            {
                TokenToReturn = null // No Authorization token was found.
            };

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenExractor,
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                oidcConfigurationManager: null); // Not accessed in this test.

            var result = await service.AuthorizeAsync(
                httpRequestHeaders: null);

            Assert.True(result.Failed);

            Assert.Equal(
                "Authorization header is missing, invalid format, or is not a Bearer token.",
                result.FailureReason);
        }

        [Fact]
        public async Task Returns_failure_if_cant_get_signing_keys_from_issuer()
        {
            const string audienceForTest = "AudienceForTest";
            const string issuerUrlForTest = "https://issuerUrl.for.test/";
            const string extractedTokenForTest = "ExtractedTokenForTest";
            const string exceptionMessageForTest = "ExceptionMessageForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = audienceForTest,
                        IssuerUrl = issuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenExractor = new FakeAuthorizationHeaderBearerTokenExractor()
            {
                TokenToReturn = extractedTokenForTest
            };

            var fakeOidcConfigurationManager = new FakeOidcConfigurationManager()
            {
                ExceptionMessageForTest = exceptionMessageForTest,
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenExractor,
                jwtSecurityTokenHandlerWrapper: null, // Not accessed in this test.
                fakeOidcConfigurationManager);

            var result = await service.AuthorizeAsync(
                httpRequestHeaders);

            Assert.True(result.Failed);

            Assert.StartsWith(
                "Problem getting signing keys from Open ID Connect provider (issuer).",
                result.FailureReason);
        }

        [Fact]
        public async Task Returns_success_for_happy_path()
        {
            const string audienceForTest = "AudienceForTest";
            const string issuerUrlForTest = "https://issuerUrl.for.test/";
            const string extractedTokenForTest = "ExtractedTokenForTest";

            var fakeApiAuthorizationSettingsOptions
                = new FakeOptions<OidcApiAuthorizationSettings>()
                {
                    Value = new OidcApiAuthorizationSettings()
                    {
                        Audience = audienceForTest,
                        IssuerUrl = issuerUrlForTest
                    }
                };

            var fakeAuthorizationHeaderBearerTokenExractor = new FakeAuthorizationHeaderBearerTokenExractor()
            {
                TokenToReturn = extractedTokenForTest
            };

            var fakeJwtSecurityTokenHandlerWrapper = new FakeJwtSecurityTokenHandlerWrapper();

            var fakeOidcConfigurationManager = new FakeOidcConfigurationManager()
            {
                SecurityKeysForTest = new List<SecurityKey>()
            };

            IHeaderDictionary httpRequestHeaders = null;

            var service = new OidcApiAuthorizationService(
                fakeApiAuthorizationSettingsOptions,
                fakeAuthorizationHeaderBearerTokenExractor,
                fakeJwtSecurityTokenHandlerWrapper,
                fakeOidcConfigurationManager);

            var result = await service.AuthorizeAsync(httpRequestHeaders);

            Assert.True(result.Success);
        }
    }
}
