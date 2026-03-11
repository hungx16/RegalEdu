using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Reward.Commands;
using RegalEdu.Application.Reward.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class RewardController : BaseController
    {
        private readonly ILogger<RewardController> _logger;
        public RewardController(IMediator mediator, ILogger<RewardController> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("CreateReward")]
        public async Task<ActionResult> CreateReward([FromBody] RewardModel model)
        {
            if (!ModelState.IsValid) return ApiBadRequest(model);
            var result = await Mediator.Send(new CreateRewardCommand { RewardModel = model });
            return ApiOk(result);
        }

        [HttpPut("UpdateReward")]
        public async Task<ActionResult> UpdateReward([FromBody] RewardModel model)
        {
            if (!ModelState.IsValid) return ApiBadRequest(model);
            var result = await Mediator.Send(new UpdateRewardCommand { RewardModel = model });
            return ApiOk(result);
        }

        [HttpDelete("DeleteReward")]
        public async Task<ActionResult> DeleteReward([FromBody] Guid id)
        {
            var result = await Mediator.Send(new DeleteRewardCommand { Id = id });
            return ApiOk(result);
        }

        [HttpGet("GetPagedRewards")]
        public async Task<ActionResult> GetPagedRewards([FromQuery] RewardQuery query)
        {
            var result = await Mediator.Send(new GetPagedRewardsQuery { Query = query });
            return ApiOk(result);
        }

        [HttpGet("GetRewardById/{id}")]
        public async Task<ActionResult> GetRewardById(Guid id)
        {
            var result = await Mediator.Send(new GetRewardByIdQuery { Id = id });
            return ApiOk(result);
        }
    }
}
