using RegalEdu.Application.Common.Results;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RegalEdu.Application.Common.Interfaces
{
    public interface IJwtAuthManager
    {
        IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary { get; }
        JwtAuthResult GenerateTokens(string username, Claim[] claims, DateTime now);
        JwtAuthResult Refresh(string refreshToken, string accessToken, DateTime now);
        void RemoveExpiredRefreshTokens(DateTime now);
        void RemoveRefreshTokenByUserName(string userName);
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
        (ClaimsPrincipal, JwtSecurityToken) GetTokenPrincipal(string token);
        ClaimsPrincipal? GetPrincipalFromToken(string accessToken, bool validateLifetime = true);
    }
}
