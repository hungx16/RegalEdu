using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models.Request;
using RegalEdu.Application.AccountGroupEmployee.Commands;
using RegalEdu.Domain.Models;
using RegalEdu.Application.AccountGroupEmployee.Queries;


namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class AccountGroupEmployeeController : BaseController
    {
        readonly ILogger<AccountGroupEmployeeController> _logger;
        public AccountGroupEmployeeController(IMediator mediator, ILogger<AccountGroupEmployeeController> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("SaveAccountGroupEmployee")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(AccountGroupEmployeeRequestModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Result>> SaveAccountGroupEmployee(AccountGroupEmployeeRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(model);
            }

            var result = await Mediator.Send(new SaveAccountGroupEmployeeCommand { AccountGroupEmployeeRequestModel = model });
            return ApiOk(result);
        }
        [HttpPost("AddAccountGroupEmployee")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(AccountGroupEmployeeRequestModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Result>> AddAccountGroupEmployee(AccountGroupEmployeeRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return ApiBadRequest(model);
            }

            var result = await Mediator.Send(new AddAccountGroupEmployeeCommand { AccountGroupEmployeeRequestModel = model });
            return ApiOk(result);
        }
        [HttpGet("GetAccountGroupEmployeeByGroupId")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAccountGroupEmployeeByGroupId(string accountGroupId)
        {
            var res = await Mediator.Send(new GetAccountGroupEmployeeByGroupIdQuery() { AccountGroupId = accountGroupId });
            return res;

        }
        [HttpGet("GetEmployeeNoGroup")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetEmployeeNoGroup()
        {
            var res = await Mediator.Send(new GetEmployeeNoGroupQuery() { });
            return res;
        }
    }
}
