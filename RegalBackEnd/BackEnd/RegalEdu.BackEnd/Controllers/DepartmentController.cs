using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.Department.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class DepartmentController : BaseController
    {
        private readonly ILogger<DepartmentController> _logger;
        public DepartmentController(ILogger<DepartmentController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddDepartment")]
        public async Task<ActionResult> AddDepartment([FromBody] DepartmentModel departmentModel)
        {
            var result = await Mediator.Send (new AddDepartmentCommand { DepartmentModel = departmentModel });
            return result;
        }

        [HttpPut ("UpdateDepartment")]
        public async Task<ActionResult> UpdateDepartment([FromBody] DepartmentModel departmentModel)
        {
            var result = await Mediator.Send (new UpdateDepartmentCommand { DepartmentModel = departmentModel });
            return result;
        }

        [HttpDelete ("DeleteListDepartment")]
        public async Task<ActionResult> DeleteListDepartment([FromBody] List<string> arrDepartmentId)
        {
            var result = await Mediator.Send (new DeleteListDepartmentCommand { ListIds = arrDepartmentId });
            return result;
        }

        [HttpDelete ("RestoreListDepartment")]
        public async Task<ActionResult> RestoreListDepartment([FromBody] List<string> arrDepartmentId)
        {
            var result = await Mediator.Send (new RestoreListDepartmentCommand { ListIds = arrDepartmentId });
            return result;
        }

        [HttpGet ("GetPagedDepartments")]
        public async Task<ActionResult> GetPagedDepartments([FromQuery] DepartmentQuery query)
        {
            var departments = await Mediator.Send (new GetPagedDepartmentsQuery { DepartmentQuery = query });
            return departments;
        }

        [HttpGet ("GetAllDepartments")]
        public async Task<ActionResult> GetAllDepartments( )
        {
            var departments = await Mediator.Send (new GetAllDepartmentsQuery { });
            return departments;
        }

        [HttpGet ("GetDeletedDepartments")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetDeletedDepartments( )
        {
            var result = await Mediator.Send (new GetDeletedDepartmentsQuery { });
            return result;
        }

        [HttpGet ("GetDepartmentById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> GetDepartmentById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetDepartmentByIdQuery { Id = id });
            return result;
        }
    }
}
