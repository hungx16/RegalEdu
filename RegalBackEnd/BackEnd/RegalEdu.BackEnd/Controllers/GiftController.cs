using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.Department.Queries;
using RegalEdu.Application.Gift.Commands;
using RegalEdu.Application.Gift.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class GiftController : BaseController
    {
        private readonly ILogger<GiftController> _logger;
        public GiftController(ILogger<GiftController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddGift")]
        public async Task<ActionResult> AddGift([FromBody] GiftModel giftModel)
        {
            var result = await Mediator.Send (new AddGiftCommand { GiftModel = giftModel });
            return result;
        }

        [HttpPut ("UpdateGift")]
        public async Task<ActionResult> UpdateGift([FromBody] GiftModel giftModel)
        {
            var result = await Mediator.Send (new UpdateGiftCommand { GiftModel = giftModel });
            return result;
        }

        [HttpDelete ("DeleteListGift")]
        public async Task<ActionResult> DeleteListGift([FromBody] List<string> arrGiftId)
        {
            var result = await Mediator.Send (new DeleteListGiftCommand { ListIds = arrGiftId });
            return result;
        }

        //[HttpDelete ("RestoreListDepartment")]
        //public async Task<ActionResult> RestoreListDepartment([FromBody] List<string> arrDepartmentId)
        //{
        //    var result = await Mediator.Send (new RestoreListDepartmentCommand { ListIds = arrDepartmentId });
        //    return result;
        //}

        [HttpGet ("GetPagedGifts")]
        public async Task<ActionResult> GetPagedGifts([FromQuery] GiftQuery query)
        {
            var result = await Mediator.Send (new GetPagedGiftsQuery { GiftQuery = query });
            return result;
        }

        [HttpGet ("GetAllGifts")]
        public async Task<ActionResult> GetAllGifts( )
        {
            var result = await Mediator.Send (new GetAllGiftsQuery { });
            return result;
        }

        //[HttpGet ("GetDeletedDepartments")]
        //[ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        //[ProducesResponseType (StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult> GetDeletedDepartments( )
        //{
        //    var result = await Mediator.Send (new GetDeletedDepartmentsQuery { });
        //    return result;
        //}

        [HttpGet ("GetGiftById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> GetGiftById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetGiftByIdQuery { Id = id });
            return result;
        }
    }
}
