using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.OpenIDConnect.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace AzureFunctions.OpenIDConnect.Tests.Fixtures
{
    public class FakeOidcConfigurationManager : IOidcConfigurationManager
    {
        public string ExceptionMessageForTest { get; set; }

        public int GetIssuerSigningKeysAsyncCalledCount { get; set; }

        public int RequestRefreshCalledCount { get; set; }

        public IEnumerable<SecurityKey> SecurityKeysForTest { get; set; }

        // IOidcConfigurationManager members

        public async Task<IEnumerable<SecurityKey>> GetIssuerSigningKeysAsync(CancellationToken cancellationToken = default)
        {
            ++GetIssuerSigningKeysAsyncCalledCount;

            if (ExceptionMessageForTest != null)
            {
                throw new TestException(ExceptionMessageForTest);
            }
            return await Task.FromResult(SecurityKeysForTest);
        }

        public void RequestRefresh()
        {
            ++RequestRefreshCalledCount;
        }
    }
}
