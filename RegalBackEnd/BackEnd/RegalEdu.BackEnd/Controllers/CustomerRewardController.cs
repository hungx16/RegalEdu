using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.CustomerReward.Commands;
using RegalEdu.Application.CustomerReward.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class CustomerRewardController : BaseController
    {
        private readonly ILogger<CustomerRewardController> _logger;

        public CustomerRewardController(IMediator mediator, ILogger<CustomerRewardController> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("CreateCustomerReward")]
        public async Task<ActionResult> CreateCustomerReward([FromBody] CustomerRewardModel model)
        {
            if (!ModelState.IsValid) return ApiBadRequest(model);
            var result = await Mediator.Send(new CreateCustomerRewardCommand { CustomerRewardModel = model });
            return ApiOk(result);
        }

        [HttpPut("UpdateCustomerReward")]
        public async Task<ActionResult> UpdateCustomerReward([FromBody] CustomerRewardModel model)
        {
            if (!ModelState.IsValid) return ApiBadRequest(model);
            var result = await Mediator.Send(new UpdateCustomerRewardCommand { CustomerRewardModel = model });
            return ApiOk(result);
        }

        [HttpDelete("DeleteCustomerReward")]
        public async Task<ActionResult> DeleteCustomerReward([FromBody] Guid id)
        {
            var result = await Mediator.Send(new DeleteCustomerRewardCommand { Id = id });
            return ApiOk(result);
        }

        [HttpPost("ConfirmReceiveCustomerReward")]
        public async Task<ActionResult> ConfirmReceiveCustomerReward([FromBody] ConfirmReceiveCustomerRewardCommand command)
        {
            if (command == null) return ApiBadRequest(command);
            var result = await Mediator.Send(command);
            return ApiOk(result);
        }

        [HttpPost("ConfirmAcceptanceCustomerReward")]
        public async Task<ActionResult> ConfirmAcceptanceCustomerReward([FromBody] ConfirmAcceptanceCustomerRewardCommand command)
        {
            if (command == null) return ApiBadRequest(command);
            var result = await Mediator.Send(command);
            return ApiOk(result);
        }

        [HttpGet("GetPagedCustomerRewards")]
        public async Task<ActionResult> GetPagedCustomerRewards([FromQuery] CustomerRewardQuery query)
        {
            var result = await Mediator.Send(new GetPagedCustomerRewardsQuery { Query = query });
            return ApiOk(result);
        }

        [HttpGet("GetCustomerRewardById/{id}")]
        public async Task<ActionResult> GetCustomerRewardById(Guid id)
        {
            var result = await Mediator.Send(new GetCustomerRewardByIdQuery { Id = id });
            return ApiOk(result);
        }
    }
}
