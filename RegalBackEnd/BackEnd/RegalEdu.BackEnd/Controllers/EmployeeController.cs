using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Employee.Commands;
using RegalEdu.Application.Employee.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class EmployeeController : BaseController
    {
        private readonly ILogger<EmployeeController> _logger;
        public EmployeeController(ILogger<EmployeeController> logger, IConfiguration configuration, IMediator mediator)
            : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("AddEmployee")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddEmployee([FromBody] EmployeeModel employeeModel)
        {
            var result = await Mediator.Send(new AddEmployeeCommand { EmployeeModel = employeeModel });
            return result;
        }

        [HttpPut("UpdateEmployee")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateEmployee([FromBody] EmployeeModel employeeModel)
        {
            var result = await Mediator.Send(new UpdateEmployeeCommand { EmployeeModel = employeeModel });
            return result;
        }
        [HttpPut("UpdateProfile")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateProfile([FromBody] EmployeeModel employeeModel)
        {
            var result = await Mediator.Send(new UpdateProfileCommand { ProfileModel = employeeModel });
            return result;
        }
        [HttpDelete("DeleteListEmployee")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListEmployee([FromBody] List<string> arrEmployeeId)
        {
            var result = await Mediator.Send(new DeleteListEmployeeCommand { ListIds = arrEmployeeId });
            return result;
        }

        [HttpDelete("RestoreListEmployee")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RestoreListEmployee([FromBody] List<string> arrEmployeeId)
        {
            var result = await Mediator.Send(new RestoreListEmployeeCommand { ListIds = arrEmployeeId });
            return result;
        }

        [HttpGet("GetEmployeeByIdOrEmail")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetEmployeeByIdOrEmail([FromQuery] string? id = null, string? email = null)
        {
            var result = await Mediator.Send(new GetEmployeeByIdOrEmailQuery { Id = id, CompanyEmail = email });
            return result;
        }

        [HttpGet("GetPagedEmployees")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedEmployees([FromQuery] EmployeeQuery query)
        {
            var employees = await Mediator.Send(new GetPagedEmployeesQuery { EmployeeQuery = query });
            return employees;
        }

        [HttpGet("GetAllEmployees")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllEmployees()
        {
            var employees = await Mediator.Send(new GetAllEmployeesQuery { });
            return employees;
        }

        [HttpGet("GetDeletedEmployees")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetDeletedEmployees()
        {
            var result = await Mediator.Send(new GetDeletedEmployeesQuery { });
            return result;
        }

        [HttpGet("IsRegionManager")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> IsRegionManager()
        {
            var result = await Mediator.Send(new IsRegionManagerQuery { });
            return result;
        }

        [HttpGet("IsCompanyManager")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> IsCompanyManager()
        {
            var result = await Mediator.Send(new IsCompanyManagerQuery { });
            return result;
        }

        [HttpGet("IsAdmissionEmployee")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> IsAdmissionEmployee()
        {
            var result = await Mediator.Send(new IsAdmissionEmployeeQuery { });
            return result;
        }

        [HttpGet("IsMarketingEmployee")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> IsMarketingEmployee()
        {
            var result = await Mediator.Send(new IsMarketingEmployeeQuery { });
            return result;
        }
        [HttpGet("IsAcademicAffairsEmployee")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> IsAcademicAffairsEmployee()
        {
            var result = await Mediator.Send(new IsAcademicAffairsEmployeeQuery { });
            return result;
        }
    }
}
