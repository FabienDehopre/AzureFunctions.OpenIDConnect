namespace AzureFunctions.OpenIDConnect.Abstractions
{
    using Microsoft.IdentityModel.Tokens;

    public interface IJwtSecurityTokenHandlerWrapper
    {
        void ValidateToken(string token, TokenValidationParameters tokenValidationParameters);
    }
}
