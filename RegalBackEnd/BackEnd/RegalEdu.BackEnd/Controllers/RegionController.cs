using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Region.Commands;
using RegalEdu.Application.Region.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class RegionController : BaseController
    {
        private readonly ILogger<RegionController> _logger;
        public RegionController(ILogger<RegionController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddRegion")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddRegion([FromBody] RegionModel regionModel)
        {
            var result = await Mediator.Send (new AddRegionCommand { RegionModel = regionModel });
            return result;
        }

        //[HttpPut ("UpdateRegion")]
        //[ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        //[ProducesResponseType (StatusCodes.Status400BadRequest)]
        //[ProducesResponseType (StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult> UpdateRegion([FromBody] RegionModel regionModel)
        //{
        //    var result = await Mediator.Send (new UpdateRegionCommand { RegionModel = regionModel });
        //    return result;
        //}

        [HttpPut ("UpdateRegion")]
        public async Task<ActionResult> UpdateRegion([FromBody] RegionModel model)
            => await Mediator.Send (new UpdateRegionCommand { RegionModel = model });

        [HttpDelete ("DeleteListRegion")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListRegion([FromBody] List<string> arrRegionId)
        {
            var result = await Mediator.Send (new DeleteListRegionCommand { ListIds = arrRegionId });
            return result;
        }

        [HttpDelete ("RestoreListRegion")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RestoreListRegion([FromBody] List<string> arrRegionId)
        {
            var result = await Mediator.Send (new RestoreListRegionCommand { ListIds = arrRegionId });
            return result;
        }

        [HttpGet ("GetRegionById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetRegionById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetRegionByIdQuery { Id = id });
            return result;
        }

        [HttpGet ("GetPagedRegions")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedRegions([FromQuery] RegionQuery query)
        {
            var regions = await Mediator.Send (new GetPagedRegionsQuery { RegionQuery = query });
            return regions;
        }

        [HttpGet ("GetAllRegions")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllRegions( )
        {
            var regions = await Mediator.Send (new GetAllRegionsQuery { });
            return regions;
        }

        [HttpGet ("GetDeletedRegions")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetDeletedRegions( )
        {
            var result = await Mediator.Send (new GetDeletedRegionsQuery { });
            return result;
        }
    }
}
