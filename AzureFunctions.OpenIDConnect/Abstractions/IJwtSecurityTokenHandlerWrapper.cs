using Microsoft.IdentityModel.Tokens;

namespace AzureFunctions.OpenIDConnect.Abstractions
{
    public interface IJwtSecurityTokenHandlerWrapper
    {
        void ValidateToken(string token, TokenValidationParameters tokenValidationParameters);
    }
}
