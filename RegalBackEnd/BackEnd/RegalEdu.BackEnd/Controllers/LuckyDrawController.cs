using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.LuckyDraw.Commands;
using RegalEdu.Application.LuckyDraw.Queries;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class LuckyDrawController : BaseController
    {
        private readonly ILogger<LuckyDrawController> _logger;
        public LuckyDrawController(IMediator mediator, ILogger<LuckyDrawController> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("CreateLuckyDraw")]
        public async Task<ActionResult> CreateLuckyDraw([FromBody] LuckyDrawModel model)
        {
            if (!ModelState.IsValid) return ApiBadRequest(model);
            var result = await Mediator.Send(new CreateLuckyDrawCommand { LuckyDrawModel = model });
            return ApiOk(result);
        }

        [HttpPut("UpdateLuckyDraw")]
        public async Task<ActionResult> UpdateLuckyDraw([FromBody] LuckyDrawModel model)
        {
            if (!ModelState.IsValid) return ApiBadRequest(model);
            var result = await Mediator.Send(new UpdateLuckyDrawCommand { LuckyDrawModel = model });
            return ApiOk(result);
        }

        [HttpDelete("DeleteLuckyDraw")]
        public async Task<ActionResult> DeleteLuckyDraw([FromBody] Guid id)
        {
            var result = await Mediator.Send(new DeleteLuckyDrawCommand { Id = id });
            return ApiOk(result);
        }

        [AllowAnonymous]
        [HttpGet("GetPagedLuckyDraws")]
        public async Task<ActionResult> GetPagedLuckyDraws([FromQuery] LuckyDrawQuery query)
        {
            var result = await Mediator.Send(new GetPagedLuckyDrawsQuery { Query = query });
            return ApiOk(result);
        }

        [AllowAnonymous]
        [HttpGet("GetLuckyDrawById/{id}")]
        public async Task<ActionResult> GetLuckyDrawById(Guid id)
        {
            var result = await Mediator.Send(new GetLuckyDrawByIdQuery { Id = id });
            return ApiOk(result);
        }

        [AllowAnonymous]
        [HttpGet("GetAllActiveLuckyDraws")]
        public async Task<ActionResult> GetAllActiveLuckyDraws()
        {
            var result = await Mediator.Send(new GetAllActiveLuckyDrawsQuery());
            return ApiOk(result);
        }
    }
}
