using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Common.Request;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.User.Commands;
using RegalEdu.Application.User.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        public UserController(ILogger<UserController> logger,
            IConfiguration configuration, IMediator mediator) : base (mediator)
        {
        }

        [HttpGet ("GetApplicationUserInfo")]
        [ProducesResponseType (typeof (ApplicationUserModel), StatusCodes.Status200OK)]
        [ProducesResponseType (typeof (ApplicationUserModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApplicationUserModel>> GetApplicationUserInfo( )
        {

            var userInfo = await Mediator.Send (new GetApplicationUserInfoQuery { });
            return Ok (userInfo);
        }

        [HttpDelete ("DeleteListUser")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListUser(List<string> arrUserId)
        {
            var result = await Mediator.Send (new DeleteListUserCommand { ListIds = arrUserId });
            return result;
        }
        [HttpDelete ("RestoreListUser")]
        [ProducesResponseType (typeof (Result), StatusCodes.Status200OK)]
        [ProducesResponseType (typeof (Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Result>> RestoreListUser(List<string> arrUserId)
        {
            var result = await Mediator.Send (new RestoreListUserCommand { ListIds = arrUserId });
            return Ok (result);
        }
        [HttpGet ("GetPagedApplicationUsers")]
        [ProducesResponseType (typeof (List<ApplicationUserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedApplicationUsers([FromQuery] ApplicationUserQuery query)
        {
            var users = await Mediator.Send (new GetPagedApplicationUserQuery { ApplicationUserQuery = query });
            return users;
        }

        [HttpGet ("GetAllApplicationUsers")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllApplicationUsers( )
        {

            var users = await Mediator.Send (new GetAllApplicationUsersQuery { });
            return users;
        }
        [HttpPost ("AddApplicationUser")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType (typeof (string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddApplicationUser([FromBody] ApplicationUserModel applicationUserModel)
        {
            var result = await Mediator.Send (new AddApplicationUserCommand { ApplicationUserModel = applicationUserModel });
            return result;
        }

        [HttpPut ("UpdateApplicationUser")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType (typeof (ApplicationUserModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateApplicationUser([FromBody] ApplicationUserModel applicationUserModel)
        {
            var result = await Mediator.Send (new UpdateApplicationUserCommand { ApplicationUserModel = applicationUserModel });
            return result;
        }

    }
}
