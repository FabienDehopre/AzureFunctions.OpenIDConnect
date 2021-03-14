namespace AzureFunctions.OpenIDConnect.Abstractions
{
    using System.Threading;
    using System.Threading.Tasks;
    using AzureFunctions.OpenIDConnect.Models;
    using Microsoft.AspNetCore.Http;

    public interface IApiAuthorization
    {
        Task<ApiAuthorizationResult> AuthorizeAsync(IHeaderDictionary httpRequestHeaders, CancellationToken cancellationToken = default);

        Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default);
    }
}
