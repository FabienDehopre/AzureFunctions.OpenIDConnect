using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.OpenIDConnect.Abstractions;
using AzureFunctions.OpenIDConnect.Models;
using Microsoft.AspNetCore.Http;

namespace AzureFunctions.OpenIDConnect.Tests.Fixtures
{
    public class FakeApiAuthorizationService : IApiAuthorization
    {
        public ApiAuthorizationResult ApiAuthorizationResultForTests { get; set; }

        public string BadHealthMessageForTests { get; set; }

        // IApiAuthorization members.

        public async Task<ApiAuthorizationResult> AuthorizeAsync(IHeaderDictionary httpRequestHeaders, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(ApiAuthorizationResultForTests);
        }

        public async Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(new HealthCheckResult(BadHealthMessageForTests));
        }
    }
}
