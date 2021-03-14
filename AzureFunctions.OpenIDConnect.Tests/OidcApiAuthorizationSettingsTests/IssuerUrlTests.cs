namespace AzureFunctions.OpenIDConnect.Tests.OidcApiAuthorizationSettingsTests
{
    using AzureFunctions.OpenIDConnect.Models;
    using Xunit;

    public class IssuerUrlTests
    {
        [Fact]
        public void Appends_missing_foward_slash()
        {
            const string withoutEndingSlash = "https://my.test.url";
            const string withEndingSlash = "https://my.test.url/";

            var settings = new OidcApiAuthorizationSettings()
            {
                IssuerUrl = withoutEndingSlash
            };

            Assert.Equal(withEndingSlash, settings.IssuerUrl);
        }

        [Fact]
        public void Defaults_to_null()
        {
            var settings = new OidcApiAuthorizationSettings();

            Assert.Null(settings.IssuerUrl);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")] // Spaces.
        [InlineData("https://my.test.url/")]
        public void Doesnt_append_foward_slash(string issuerUrl)
        {
            var settings = new OidcApiAuthorizationSettings()
            {
                IssuerUrl = issuerUrl
            };

            Assert.Equal(issuerUrl, settings.IssuerUrl);
        }
    }
}
