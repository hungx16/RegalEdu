using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
using RegalEdu.Application.AccountGroup.Commands;
using RegalEdu.Application.AccountGroup.Queries;
using RegalEdu.Application.Common.Request;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class AccountGroupController : BaseController
    {
        readonly ILogger<AccountGroupController> _logger;
        public AccountGroupController(IMediator mediator, ILogger<AccountGroupController> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("AddAccountGroup")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(AccountGroupModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddAccountGroup(AccountGroupModel accountGroup)
        {
            var result = await Mediator.Send(new CreateAccountGroupCommand { AccountGroup = accountGroup });
            return (result);
        }
        [AllowAnonymous]
        [HttpGet("GetAccountGroups")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAccountGroups([FromQuery] AccountGroupQuery query)
        {
           
            var accountGroups = await Mediator.Send(new GetAccountGroupsQuery { AccountGroupQuery = query });
            return accountGroups;
        }
        [AllowAnonymous]
        [HttpGet("GetAllAccountGroups")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllAccountGroups()
        {

            var accountGroups = await Mediator.Send(new GetAllAccountGroupsQuery { });
            return accountGroups;
        }
        [HttpPut("UpdateAccountGroup")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(AccountGroupModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateAccountGroup([FromBody] AccountGroupModel model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(model);
            }

            var result = await Mediator.Send(new UpdateAccountGroupCommand { Entity = model });
            return result;
        }
        [HttpDelete("DeleteAccountGroup")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Result>> DeleteAccountGroup(List<string> listId)
        {
            var result = await Mediator.Send(new DeleteAccountGroupCommand { ListId = listId });
            return ApiOk(result);
        }

    }
}
