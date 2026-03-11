using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RegalEdu.Application.Auth.Commands;
using RegalEdu.Application.Auth.Queries;
using RegalEdu.Application.Common.Request;
using RegalEdu.Application.Common.Results;
using System.Security.Claims;


namespace RegalEdu.Api.Controllers
{
    public class AuthController : BaseController
    {
        public AuthController(IMediator mediator) : base (mediator)
        {
        }

        [AllowAnonymous]
        [HttpPost ("login")]
        [ProducesResponseType (typeof (Result<IdentityResult>), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Result<IdentityResult>>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest (Result<IdentityResult>.Failure ("Invalid request data"));
            }

            IdentityResult identityResult = await Mediator.Send (new GetUserLoginResultQuery
            {
                UserName = request.UserName,
                Password = request.Password,
                RememberMe = request.RememberMe,
            });

            if (identityResult.Succeeded)
            {
                return Result<IdentityResult>.Success (identityResult);
            }
            else
            {
                return Result<IdentityResult>.Failure (identityResult.ErrorMessage ?? "Login failed");
            }
        }


        [AllowAnonymous]
        [HttpPost ("register")]
        [ProducesResponseType (typeof (RegisterRequest), StatusCodes.Status400BadRequest)]
        [ProducesResponseType (typeof (Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType (typeof (Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest (request);
            }

            Result result = await Mediator.Send (new AddUserCommand
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password

            });

            if (result.Succeeded)
            {
                return Ok (result);
            }

            return BadRequest (Result.Failure ("Register failed"));
        }

        [HttpGet ("logout")]
        [ProducesResponseType (StatusCodes.Status200OK)]
        public async Task<ActionResult> Logout( )
        {
            // optionally "revoke" JWT token on the server side --> add the current token to a block-list
            // https://github.com/auth0/node-jsonwebtoken/issues/375

            var userName = User.Identity.Name;

            await Mediator.Send (new DeleteRefreshTokenCommand
            {
                UserName = userName
            });
            return Ok ( );
        }
        [AllowAnonymous]

        [HttpPost ("refresh-token")]
        [ProducesResponseType (typeof (IdentityResult), StatusCodes.Status200OK)]
        [ProducesResponseType (typeof (string), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IdentityResult>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {


                if (string.IsNullOrWhiteSpace (request.RefreshToken))
                {
                    return Unauthorized ( );
                }

                IdentityResult result = await Mediator.Send (new GetNewTokenByRefreshTokenQuery
                {
                    AccessToken = request.AccessToken,
                    RefreshToken = request.RefreshToken,
                });
                //_logger.LogInformation($"User [{result.UserName}] is trying to refresh JWT token.");
                return Ok (result);
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized (e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }
        [AllowAnonymous]
        [HttpPost ("verify-token")]
        [ProducesResponseType (typeof (IdentityResult), StatusCodes.Status200OK)]
        [ProducesResponseType (typeof (string), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<VerifyTokenResponse>> VerifyToken([FromBody] VerifyTokenRequest request)
        {
            try
            {
                var userName = User.Identity.Name;

                if (string.IsNullOrWhiteSpace (request.AccessToken))
                {
                    return Unauthorized ( );
                }
                VerifyTokenResponse result = await Mediator.Send (new VerifyTokenQuery { VerifyTokenRequest = request });
                return Ok (result);

            }
            catch (SecurityTokenException e)
            {
                return Unauthorized (e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }
        [AllowAnonymous]
        [HttpPost ("ForgotPassword")]
        [ProducesResponseType (typeof (Result), StatusCodes.Status200OK)]
        [ProducesResponseType (typeof (Result), StatusCodes.Status404NotFound)]

        public async Task<ActionResult<Result>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await Mediator.Send (new ForgotPasswordCommand { ForgetPasswordRequest = request });
            return Ok (result);
        }
        //[AllowAnonymous]
        //[HttpGet ("ResetPassword")]
        //[ProducesResponseType (typeof (Result), StatusCodes.Status200OK)]
        //[ProducesResponseType (typeof (Result), StatusCodes.Status404NotFound)]

        //public async Task<ActionResult<Result>> ResetPassword(Guid userId, string token, string culture = "en-US")
        //{
        //    var result = await Mediator.Send (new ConfirmPasswordResetCommand { UserId = userId, Token = token, Culture = culture });
        //    if (result.Succeeded)
        //    {
        //        return Content (result.Data.ToString ( ), "text/html");
        //    }
        //    else
        //    {
        //        return Content (result.Errors, "text/html");
        //    }
        //}
        //[AllowAnonymous]
        //[HttpPost ("RequestPasswordReset")]
        //[ProducesResponseType (typeof (Result), StatusCodes.Status200OK)]
        //[ProducesResponseType (typeof (Result), StatusCodes.Status404NotFound)]

        //public async Task<ActionResult<Result>> RequestPasswordReset([FromBody] ForgotPasswordRequest request)
        //{
        //    var result = await Mediator.Send (new RequestPasswordResetCommand { ChangePasswordRequest = request });
        //    return Ok (result);
        //}
        [HttpPost ("ChangePassword")]
        [ProducesResponseType (typeof (Result), StatusCodes.Status200OK)]
        [ProducesResponseType (typeof (string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType (typeof (Result), StatusCodes.Status404NotFound)]

        public async Task<ActionResult<Result>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await Mediator.Send (new ChangePasswordCommand { ChangePasswordRequest = request });

            return Ok (result);
        }
        [HttpGet ("info")]
        [ProducesResponseType (typeof (IdentityResult), StatusCodes.Status200OK)]
        [ProducesResponseType (typeof (string), StatusCodes.Status401Unauthorized)]
        public ActionResult GetUserInfo( )
        {
            var roles = ((ClaimsIdentity)User.Identity).Claims
                .Where (c => c.Type == ClaimTypes.Role)
                .Select (c => c.Value)
                .ToList ( );
            var a = User;
            return Ok (new IdentityResult
            {
                UserName = User.Identity.Name,
                Roles = roles,
                OriginalUserName = User.FindFirst ("OriginalUserName")?.Value
            });
        }
    }
}
