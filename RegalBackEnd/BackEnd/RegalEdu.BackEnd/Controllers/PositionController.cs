using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Position.Commands;
using RegalEdu.Application.Position.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class PositionController : BaseController
    {
        private readonly ILogger<PositionController> _logger;
        public PositionController(ILogger<PositionController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddPosition")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddPosition([FromBody] PositionModel positionModel)
        {
            var result = await Mediator.Send (new AddPositionCommand { PositionModel = positionModel });
            return result;
        }

        [HttpPut ("UpdatePosition")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdatePosition([FromBody] PositionModel positionModel)
        {
            var result = await Mediator.Send (new UpdatePositionCommand { PositionModel = positionModel });
            return result;
        }

        [HttpDelete ("DeleteListPosition")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListPosition([FromBody] List<string> arrPositionId)
        {
            var result = await Mediator.Send (new DeleteListPositionCommand { ListIds = arrPositionId });
            return result;
        }

        [HttpDelete ("RestoreListPosition")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RestoreListPosition([FromBody] List<string> arrPositionId)
        {
            var result = await Mediator.Send (new RestoreListPositionCommand { ListIds = arrPositionId });
            return result;
        }

        [HttpGet ("GetPositionById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPositionById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetPositionByIdQuery { Id = id });
            return result;
        }

        [HttpGet ("GetPagedPositions")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedPositions([FromQuery] PositionQuery query)
        {
            var positions = await Mediator.Send (new GetPagedPositionsQuery { PositionQuery = query });
            return positions;
        }

        [HttpGet ("GetAllPositions")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllPositions( )
        {
            var positions = await Mediator.Send (new GetAllPositionsQuery { });
            return positions;
        }

        [HttpGet ("GetDeletedPositions")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetDeletedPositions( )
        {
            var result = await Mediator.Send (new GetDeletedPositionsQuery { });
            return result;
        }
    }
}
