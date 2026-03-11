using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.PayrollTeacher.Commands;
using RegalEdu.Application.PayrollTeacher.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class PayrollTeacherController : BaseController
    {
        private readonly ILogger<PayrollTeacherController> _logger;

        public PayrollTeacherController(
            ILogger<PayrollTeacherController> logger,
            IMediator mediator) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("AddPayrollTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddPayrollTeacher([FromBody] PayrollTeacherModel model)
        {
            var result = await Mediator.Send(new AddPayrollTeacherCommand { PayrollTeacherModel = model });
            return result;
        }

        [HttpPut("UpdatePayrollTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdatePayrollTeacher([FromBody] PayrollTeacherModel model)
        {
            var result = await Mediator.Send(new UpdatePayrollTeacherCommand { PayrollTeacherModel = model });
            return result;
        }

        [HttpDelete("DeletePayrollTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeletePayrollTeacher([FromQuery] Guid id)
        {
            var result = await Mediator.Send(new DeletePayrollTeacherCommand { Id = id });
            return result;
        }

        [HttpPut("MarkAsPaidPayrollTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> MarkAsPaidPayrollTeacher([FromBody] MarkAsPaidPayrollTeacherCommand command)
        {
            var result = await Mediator.Send(command);
            return result;
        }

        [HttpGet("GetPayrollTeacherById")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPayrollTeacherById([FromQuery] Guid id)
        {
            var result = await Mediator.Send(new GetPayrollTeacherByIdQuery { Id = id });
            return result;
        }

        [HttpGet("GetPagedPayrollTeachers")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedPayrollTeachers([FromQuery] PayrollTeacherQuery query)
        {
            var result = await Mediator.Send(new GetPagedPayrollTeachersQuery { PayrollTeacherQuery = query });
            return result;
        }

        [HttpGet("GetAllPayrollTeachers")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllPayrollTeachers()
        {
            var result = await Mediator.Send(new GetAllPayrollTeachersQuery());
            return result;
        }

        [HttpGet("GetTeacherPayrolls")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetTeacherPayrolls([FromQuery] Guid teacherId, [FromQuery] int? year)
        {
            var result = await Mediator.Send(new GetTeacherPayrollsQuery
            {
                TeacherId = teacherId,
                Year = year
            });
            return result;
        }

        [HttpGet("GetPayrollSummary")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPayrollSummary([FromQuery] int? year, [FromQuery] int? month)
        {
            var result = await Mediator.Send(new GetPayrollSummaryQuery
            {
                Year = year,
                Month = month
            });
            return result;
        }

        // Các endpoint cho xử lý danh sách (nếu cần)
        [HttpDelete("DeleteListPayrollTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListPayrollTeacher([FromBody] List<Guid> ids)
        {
            // Cần tạo DeleteListPayrollTeacherCommand nếu cần xóa nhiều
            // var result = await Mediator.Send(new DeleteListPayrollTeacherCommand { ListIds = ids });
            // return result;
            return BadRequest("Endpoint chưa được triển khai");
        }

        [HttpPost("RestorePayrollTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RestorePayrollTeacher([FromQuery] Guid id)
        {
            // Cần tạo RestorePayrollTeacherCommand nếu cần khôi phục
            // var result = await Mediator.Send(new RestorePayrollTeacherCommand { Id = id });
            // return result;
            return BadRequest("Endpoint chưa được triển khai");
        }

        [HttpGet("GetDeletedPayrollTeachers")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetDeletedPayrollTeachers()
        {
            // Cần tạo GetDeletedPayrollTeachersQuery nếu cần lấy danh sách đã xóa
            // var result = await Mediator.Send(new GetDeletedPayrollTeachersQuery());
            // return result;
            return BadRequest("Endpoint chưa được triển khai");
        }

        [HttpGet("GeneratePayrollReport")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GeneratePayrollReport([FromQuery] int year, [FromQuery] int month)
        {
            // Có thể tạo thêm Query để tạo báo cáo lương
            // var result = await Mediator.Send(new GeneratePayrollReportQuery { Year = year, Month = month });
            // return result;
            return BadRequest("Endpoint chưa được triển khai");
        }
    }
}