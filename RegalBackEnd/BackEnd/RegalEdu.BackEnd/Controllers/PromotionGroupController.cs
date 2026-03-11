using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.Department.Queries;
using RegalEdu.Application.Gift.Commands;
using RegalEdu.Application.Gift.Queries;
using RegalEdu.Application.PromotionGroup.Commands;
using RegalEdu.Application.PromotionGroup.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class PromotionGroupController : BaseController
    {
        private readonly ILogger<PromotionGroupController> _logger;
        public PromotionGroupController(ILogger<PromotionGroupController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddPromotionGroup")]
        public async Task<ActionResult> AddPromotionGroup([FromBody] PromotionGroupModel model)
        {
            var result = await Mediator.Send (new AddPromotionGroupCommand { PromotionGroupModel = model });
            return result;
        }

        [HttpPut ("UpdatePromotionGroup")]
        public async Task<ActionResult> UpdatePromotionGroup([FromBody] PromotionGroupModel model)
        {
            var result = await Mediator.Send (new UpdatePromotionGroupCommand { PromotionGroupModel = model });
            return result;
        }

        [HttpDelete ("DeleteListPromotionGroup")]
        public async Task<ActionResult> DeleteListPromotionGroup([FromBody] List<string> arrId)
        {
            var result = await Mediator.Send (new DeleteListPromotionGroupCommand { ListIds = arrId });
            return result;
        }

        //[HttpDelete ("RestoreListDepartment")]
        //public async Task<ActionResult> RestoreListDepartment([FromBody] List<string> arrDepartmentId)
        //{
        //    var result = await Mediator.Send (new RestoreListDepartmentCommand { ListIds = arrDepartmentId });
        //    return result;
        //}

        [HttpGet ("GetPagedPromotionGroups")]
        public async Task<ActionResult> GetPagedPromotionGroups([FromQuery] PromotionGroupQuery query)
        {
            var result = await Mediator.Send (new GetPagedPromotionGroupsQuery { PromotionGroupQuery = query });
            return result;
        }

        [HttpGet ("GetAllPromotionGroups")]
        public async Task<ActionResult> GetAllPromotionGroups( )
        {
            var result = await Mediator.Send(new GetAllPromotionGroupsQuery { });
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

        [HttpGet ("GetPromotionGroupById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> GetPromotionGroupById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetPromotionGroupByIdQuery { Id = id });
            return result;
        }
    }
}
