using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.OpenIDConnect.Models;
using Microsoft.AspNetCore.Http;

namespace AzureFunctions.OpenIDConnect.Abstractions
{
    public interface IApiAuthorization
    {
        Task<ApiAuthorizationResult> AuthorizeAsync(IHeaderDictionary httpRequestHeaders, CancellationToken cancellationToken = default);

        Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default);
    }
}
