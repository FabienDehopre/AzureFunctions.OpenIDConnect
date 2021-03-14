namespace AzureFunctions.OpenIDConnect
{
    using AzureFunctions.OpenIDConnect.Abstractions;
    using AzureFunctions.OpenIDConnect.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServicesConfigurationExtensions
    {
        public static void AddOidcApiAuthorization(this IServiceCollection services)
        {
            // Setup injection of OidcApiAuthorizationSettings configured in the
            // Function's app settings (or local.settings.json)
            // as IOptions<OidcApiAuthorizationSettings>.
            // See https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection#working-with-options-and-settings
            _ = services.AddOptions<OidcApiAuthorizationSettings>()
                .Configure<IConfiguration>((settings, configuration) => configuration.GetSection(nameof(OidcApiAuthorizationSettings)).Bind(settings));

            // These are created as a singletons, so that only one instance of each
            // is created for the lifetime of the hosting Azure Function App.
            // That helps reduce the number of calls to the authorization service
            // for the signing keys and other stuff that can be used across multiple
            // calls to the HTTP triggered Azure Functions.

            _ = services.AddSingleton<IAuthorizationHeaderBearerTokenExtractor, AuthorizationHeaderBearerTokenExtractor>();
            _ = services.AddSingleton<IJwtSecurityTokenHandlerWrapper, JwtSecurityTokenHandlerWrapper>();
            _ = services.AddSingleton<IOidcConfigurationManager, OidcConfigurationManager>();

            _ = services.AddSingleton<IApiAuthorization, OidcApiAuthorizationService>();
        }
    }
}
