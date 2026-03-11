using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Promotion.Commands;
using RegalEdu.Application.Promotion.Queries;
using RegalEdu.Application.Region.Commands;
using RegalEdu.Application.Region.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class PromotionController : BaseController
    {
        private readonly ILogger<PromotionController> _logger;
        public PromotionController(ILogger<PromotionController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddPromotion")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddPromotion([FromBody] PromotionModel promotionModel)
        {
            var result = await Mediator.Send (new AddPromotionCommand { PromotionModel = promotionModel });
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

        [HttpPut("UpdatePromotion")]
        public async Task<ActionResult> UpdatePromotion([FromBody] PromotionModel model)
        {
            return await Mediator.Send(new UpdatePromotionCommand { PromotionModel = model });
        }

        [HttpDelete ("DeleteListPromotion")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListPromotion([FromBody] List<string> arrId)
        {
            var result = await Mediator.Send (new DeleteListPromotionCommand { ListIds = arrId });
            return result;
        }

        //[HttpDelete ("RestoreListRegion")]
        //[ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        //[ProducesResponseType (StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult> RestoreListRegion([FromBody] List<string> arrRegionId)
        //{
        //    var result = await Mediator.Send (new RestoreListRegionCommand { ListIds = arrRegionId });
        //    return result;
        //}

        [HttpGet ("GetPromotionById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPromotionById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetPromotionByIdQuery { Id = id });
            return result;
        }

        [HttpGet ("GetPagedPromotions")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedPromotions([FromQuery] PromotionQuery query)
        {
            var result = await Mediator.Send (new GetPagedPromotionsQuery { PromotionQuery = query });
            return result;
        }

        [HttpGet ("GetAllPromotions")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllPromotions( )
        {
            var result = await Mediator.Send (new GetAllPromotionsQuery { });
            return result;
        }
        [HttpGet("GetGlobalPromotions")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetGlobalPromotions()
        {
            var result = await Mediator.Send(new GetGlobalPromotionQuery { });
            return result;
        }
        [HttpGet("GetPromotionValues")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPromotionValues()
        {
            var result = await Mediator.Send(new GetPromotionValueQuery { });
            return result;
        }
        //[HttpGet ("GetDeletedRegions")]
        //[ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        //[ProducesResponseType (StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult> GetDeletedRegions( )
        //{
        //    var result = await Mediator.Send (new GetDeletedRegionsQuery { });
        //    return result;
        //}
    }
}
