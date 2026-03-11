using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.CouponIssue.Commands;

using RegalEdu.Application.CouponIssue.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class CouponIssueController : BaseController
    {
        private readonly ILogger<CouponIssueController> _logger;
        public CouponIssueController(ILogger<CouponIssueController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddCouponIssue")]
        public async Task<ActionResult> AddCouponIssue([FromBody] CouponIssueModel model)
        {
            var result = await Mediator.Send (new AddCouponIssueCommand { CouponIssueModel = model });
            return result;
        }

        [HttpPut ("UpdateCouponIssue")]
        public async Task<ActionResult> UpdateCouponIssue([FromBody] CouponIssueModel model)
        {
            var result = await Mediator.Send (new UpdateCouponIssueCommand { CouponIssueModel = model });
            return result;
        }

        [HttpDelete ("DeleteListCouponIssue")]
        public async Task<ActionResult> DeleteListCouponIssue([FromBody] List<string> arrId)
        {
            var result = await Mediator.Send (new DeleteListCouponIssueCommand { ListIds = arrId });
            return result;
        }
        [HttpDelete("DeleteListCoupon")]
        public async Task<ActionResult> DeleteListCoupon([FromBody] List<string> arrId)
        {
            var result = await Mediator.Send(new DeleteListCouponCommand { ListIds = arrId });
            return result;
        }
        //[HttpDelete ("RestoreListDepartment")]
        //public async Task<ActionResult> RestoreListDepartment([FromBody] List<string> arrDepartmentId)
        //{
        //    var result = await Mediator.Send (new RestoreListDepartmentCommand { ListIds = arrDepartmentId });
        //    return result;
        //}

        [HttpGet ("GetPagedCouponIssues")]
        public async Task<ActionResult> GetPagedCouponIssues([FromQuery] CouponIssueQuery query)
        {
            var result = await Mediator.Send (new GetPagedCouponIssuesQuery { CouponIssueQuery = query });
            return result;
        }

        [HttpGet ("GetAllCouponIssues")]
        public async Task<ActionResult> GetAllCouponIssues( )
        {
            var result = await Mediator.Send(new GetAllCouponIssuesQuery { });
            return result;
        }

        [HttpGet("GetAllCoupons")]
        public async Task<ActionResult> GetAllCoupons()
        {
            var result = await Mediator.Send(new GetAllCouponQuery { });
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

        [HttpGet ("GetCouponIssueById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> GetCouponIssueById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetCouponIssueByIdQuery { Id = id });
            return result;
        }
    }
}
