using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.CouponType.Commands;

using RegalEdu.Application.CouponType.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class CouponTypeController : BaseController
    {
        private readonly ILogger<CouponTypeController> _logger;
        public CouponTypeController(ILogger<CouponTypeController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddCouponType")]
        public async Task<ActionResult> AddCouponType([FromBody] CouponTypeModel model)
        {
            var result = await Mediator.Send (new AddCouponTypeCommand { CouponTypeModel = model });
            return result;
        }

        [HttpPut ("UpdateCouponType")]
        public async Task<ActionResult> UpdateCouponType([FromBody] CouponTypeModel model)
        {
            var result = await Mediator.Send (new UpdateCouponTypeCommand { CouponTypeModel = model });
            return result;
        }

        [HttpDelete ("DeleteListCouponType")]
        public async Task<ActionResult> DeleteListCouponType([FromBody] List<string> arrId)
        {
            var result = await Mediator.Send (new DeleteListCouponTypeCommand { ListIds = arrId });
            return result;
        }

        //[HttpDelete ("RestoreListDepartment")]
        //public async Task<ActionResult> RestoreListDepartment([FromBody] List<string> arrDepartmentId)
        //{
        //    var result = await Mediator.Send (new RestoreListDepartmentCommand { ListIds = arrDepartmentId });
        //    return result;
        //}

        [HttpGet ("GetPagedCouponTypes")]
        public async Task<ActionResult> GetPagedCouponTypes([FromQuery] CouponTypeQuery query)
        {
            var result = await Mediator.Send (new GetPagedCouponTypesQuery { CouponTypeQuery = query });
            return result;
        }

        [HttpGet ("GetAllCouponTypes")]
        public async Task<ActionResult> GetAllCouponTypes( )
        {
            var result = await Mediator.Send(new GetAllCouponTypesQuery { });
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

        [HttpGet ("GetCouponTypeById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> GetCouponTypeById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetCouponTypeByIdQuery { Id = id });
            return result;
        }
    }
}
