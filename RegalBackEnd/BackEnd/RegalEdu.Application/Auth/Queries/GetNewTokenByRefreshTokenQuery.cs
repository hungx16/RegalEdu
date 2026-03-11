using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;

namespace RegalEdu.Application.Auth.Queries
{
    public class GetNewTokenByRefreshTokenQuery : IRequest<IdentityResult>
    {
        public required string RefreshToken { get; set; }
        public required string AccessToken { get; set; }
    }

    public class GetNewTokenByRefreshTokenQueryHandler : IRequestHandler<GetNewTokenByRefreshTokenQuery, IdentityResult>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetUserLoginResultQueryHandler> _logger;
        private readonly IJwtAuthManager _jwtAuthManager;

        public GetNewTokenByRefreshTokenQueryHandler(
             ILogger<GetUserLoginResultQueryHandler> logger,
             IIdentityService identityService,
             IJwtAuthManager jwtAuthManager)
        {
            _identityService = identityService ?? throw new ArgumentNullException (nameof (identityService));
            _logger = logger ?? throw new ArgumentNullException (nameof (identityService));
            _jwtAuthManager = jwtAuthManager ?? throw new ArgumentNullException (nameof (identityService));
        }

        public async Task<IdentityResult> Handle(GetNewTokenByRefreshTokenQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var (principal, jwtToken) = _jwtAuthManager.GetTokenPrincipal (request.AccessToken);
                IdentityResult identityResult = new IdentityResult ( );
                if (principal?.Identity?.Name is null)
                {
                    identityResult = IdentityResult.Error ("Invalid token");
                    return identityResult;
                }

                var identityUser = await _identityService.GetUserByIdentifierAsync (principal.Identity.Name);
                if (identityUser is null)
                {
                    identityResult = IdentityResult.Error ("Invalid token");
                    return identityResult;
                }
                if (identityUser.RefreshToken != request.RefreshToken)
                {
                    identityResult = IdentityResult.Error ("Invalid refresh token");
                    _logger.LogInformation ($"User [{identityUser?.UserName}] has invalid refresh token.");
                    return identityResult;
                }
                if (identityUser.RefreshTokenExpiry < DateTime.Now)
                {
                    identityResult = IdentityResult.Error ("Token is expired");
                    _logger.LogInformation ($"User [{identityUser?.UserName}] has expired refresh token.");
                    return identityResult;
                }
                var roles = await _identityService.GetRolesUserAsync (identityUser.UserName);
                var jwtResult = _jwtAuthManager.GenerateTokens (identityUser.UserName, principal.Claims.ToArray ( ), DateTime.Now);
                _logger.LogInformation ($"User [{identityUser.UserName}] has refreshed JWT token.");
                identityUser.RefreshToken = jwtResult.RefreshToken;
                identityUser.RefreshTokenExpiry = DateTime.Now.AddMinutes (2);
                await _identityService.UpdateUserAsync (identityUser);
                return IdentityResult.Success (identityUser.UserName, roles, identityUser.UserName, jwtResult.AccessToken, jwtResult.RefreshToken, "");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
