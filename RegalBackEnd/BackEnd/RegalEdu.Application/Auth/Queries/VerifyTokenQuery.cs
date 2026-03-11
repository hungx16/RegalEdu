using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Request;
using AutoMapper;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Auth.Queries
{
    public class VerifyTokenQuery : IRequest<VerifyTokenResponse>
    {
        public VerifyTokenRequest VerifyTokenRequest { get; set; }

    }

    public class VerifyTokenQueryHandler : IRequestHandler<VerifyTokenQuery, VerifyTokenResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetUserLoginResultQueryHandler> _logger;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IMapper _mapper;

        public VerifyTokenQueryHandler(
            ILogger<GetUserLoginResultQueryHandler> logger, IIdentityService identityService, IJwtAuthManager jwtAuthManager, IMapper mapper)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(identityService));
            _jwtAuthManager = jwtAuthManager ?? throw new ArgumentNullException(nameof(identityService));
            _mapper = mapper;
        }

        public async Task<VerifyTokenResponse> Handle(VerifyTokenQuery request, CancellationToken cancellationToken)
        {
            //try
            //{
            //    var (principal, jwtToken) = _jwtAuthManager.DecodeJwtToken(request.VerifyTokenRequest.AccessToken);
            //    if (principal == null || jwtToken == null)
            //    {
            //        return Vr.Error("Invalid token");
            //    }
            //    Claim? claim = jwtToken.Claims.FirstOrDefault(x => x.Type == "exp");
            //    if (claim == null)
            //    {
            //        return IdentityResult.Error("Invalid token");
            //    }
            //    if (claim == null)
            //    {
            //        return IdentityResult.Error("Invalid token");
            //    }
            //    int expTime = -1;
            //    int.TryParse(claim.Value, out expTime);
            //    if (expTime == -1)
            //    {
            //        return IdentityResult.Error("Invalid expired time");

            //    }
            //    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(expTime);

            //    if (dateTimeOffset.LocalDateTime > DateTime.Now)
            //    {
            //        //jwtAuthManager.RemoveExpiredRefreshTokens(request.RefreshToken);
            //        var userName = principal.Identity.Name;
            //        var roles = await _identityService.GetRolesUserAsync(userName);
            //        var userId = await _identityService.GetUserIdAsync(userName);
            //        var jwtResult = _jwtAuthManager.Refresh(request.VerifyTokenRequest.RefreshToken, request.VerifyTokenRequest.AccessToken, DateTime.Now);
            //        return IdentityResult.Success(userName, roles, userId.ToString(), jwtResult.AccessToken, jwtResult.RefreshToken);

            //        //if (refreshTokenData?.ExpireAt < DateTime.Now)
            //        //{
            //        //    var userName = principal.Identity.Name;
            //        //    var roles = await _identityService.GetRolesUserAsync(userName);
            //        //    var userId = await _identityService.GetUserIdAsync(userName);
            //        //    var jwtResult = jwtAuthManager.GenerateTokens(userName, principal.Claims.ToArray(), DateTime.Now);
            //        //    _usersRefreshTokens.TryRemove(refreshTokenData.TokenString, out _); // remove the old refresh token
            //        //    _usersRefreshTokens.AddOrUpdate(jwtResult.RefreshToken.TokenString, jwtResult.RefreshToken, (s, t) => jwtResult.RefreshToken);
            //        //    return IdentityResult.Success(userName, roles, userId, jwtResult.AccessToken, jwtResult.RefreshToken.TokenString);
            //        //}
            //        //return IdentityResult.Error("Expired token");
            //    }
            //    return IdentityResult.Error("Expired token"); ;
            //}
            //catch (SecurityTokenException)
            //{
            //    throw;
            //}
            try
            {
                // 1. Thử decode AccessToken trước (còn hạn thì trả user luôn)
                var (principal, jwtToken) = _jwtAuthManager.DecodeJwtToken(request.VerifyTokenRequest.AccessToken);

                if (principal != null && jwtToken != null)
                {
                    var userName = principal.Identity?.Name;
                    if (string.IsNullOrEmpty(userName))
                        return new VerifyTokenResponse { Succeeded = false, Message = "Không xác định được user" };

                    var user = await _identityService.GetUserByIdentifierAsync(userName);
                    if (user == null)
                        return new VerifyTokenResponse { Succeeded = false, Message = "User không tồn tại" };

                    return new VerifyTokenResponse
                    {
                        Succeeded = true,
                        User = _mapper.Map<ApplicationUserModel>(user),
                        AccessToken = request.VerifyTokenRequest.AccessToken,
                        RefreshToken = request.VerifyTokenRequest.RefreshToken
                    };
                }

                // 2. Nếu AccessToken hết hạn → thử RefreshToken
                var jwtResult = _jwtAuthManager.Refresh(request.VerifyTokenRequest.RefreshToken, request.VerifyTokenRequest.AccessToken, DateTime.UtcNow);
                if (jwtResult == null)
                {
                    return new VerifyTokenResponse { Succeeded = false, Message = "Token hết hạn hoặc không hợp lệ" };
                }

                // Lấy lại user (từ claims hoặc decode lại)
                var refreshPrincipal = _jwtAuthManager.GetPrincipalFromToken(jwtResult.AccessToken);
                var refreshUserName = refreshPrincipal?.Identity?.Name;
                if (string.IsNullOrEmpty(refreshUserName))
                    return new VerifyTokenResponse { Succeeded = false, Message = "Không xác định được user" };

                var refreshUser = await _identityService.GetUserByIdentifierAsync(refreshUserName);
                if (refreshUser == null)
                    return new VerifyTokenResponse { Succeeded = false, Message = "User không tồn tại" };

                return new VerifyTokenResponse
                {
                    Succeeded = true,
                    User = _mapper.Map<ApplicationUserModel>(refreshUser),
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Xác thực token thất bại");
                return new VerifyTokenResponse { Succeeded = false, Message = "Có lỗi xác thực token" };
            }
        }
    }
}
