using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.WorkBoardTeacher.Commands;
using RegalEdu.Application.WorkBoardTeacher.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class WorkBoardTeacherController : BaseController
    {
        private readonly ILogger<WorkBoardTeacherController> _logger;

        public WorkBoardTeacherController(
            ILogger<WorkBoardTeacherController> logger,
            IMediator mediator) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("AddWorkBoardTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddWorkBoardTeacher([FromBody] WorkBoardTeacherModel model)
        {
            var result = await Mediator.Send(new AddWorkBoardTeacherCommand { WorkBoardTeacherModel = model });
            return result;
        }

        [HttpPut("UpdateWorkBoardTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateWorkBoardTeacher([FromBody] WorkBoardTeacherModel model)
        {
            var result = await Mediator.Send(new UpdateWorkBoardTeacherCommand { WorkBoardTeacherModel = model });
            return result;
        }

        [HttpDelete("DeleteWorkBoardTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteWorkBoardTeacher([FromQuery] Guid id)
        {
            var result = await Mediator.Send(new DeleteWorkBoardTeacherCommand { Id = id });
            return result;
        }

        [HttpPost("ConfirmWorkBoardTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> ConfirmWorkBoardTeacher([FromBody] ConfirmWorkBoardTeacherCommand command)
        {
            var result = await Mediator.Send(command);
            return result;
        }

        [HttpGet("GetWorkBoardTeacherById")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetWorkBoardTeacherById([FromQuery] Guid id)
        {
            var result = await Mediator.Send(new GetWorkBoardTeacherByIdQuery { Id = id });
            return result;
        }

        [HttpGet("GetPagedWorkBoardTeachers")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedWorkBoardTeachers([FromQuery] WorkBoardTeacherQuery query)
        {
            var result = await Mediator.Send(new GetPagedWorkBoardTeachersQuery { WorkBoardTeacherQuery = query });
            return result;
        }

        [HttpGet("GetAllWorkBoardTeachers")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllWorkBoardTeachers()
        {
            var result = await Mediator.Send(new GetAllWorkBoardTeachersQuery());
            return result;
        }

        [HttpGet("GetTeacherWorkBoard")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetTeacherWorkBoard([FromQuery] Guid teacherId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var result = await Mediator.Send(new GetTeacherWorkBoardQuery
            {
                TeacherId = teacherId,
                FromDate = fromDate,
                ToDate = toDate
            });
            return result;
        }

        [HttpGet("GetWorkBoardSummary")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetWorkBoardSummary([FromQuery] Guid? teacherId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var result = await Mediator.Send(new GetWorkBoardSummaryQuery
            {
                TeacherId = teacherId,
                FromDate = fromDate,
                ToDate = toDate
            });
            return result;
        }

        // Các endpoint cho xử lý danh sách (nếu cần)
        [HttpDelete("DeleteListWorkBoardTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListWorkBoardTeacher([FromBody] List<Guid> ids)
        {
            // Cần tạo DeleteListWorkBoardTeacherCommand nếu cần xóa nhiều
            // var result = await Mediator.Send(new DeleteListWorkBoardTeacherCommand { ListIds = ids });
            // return result;
            return BadRequest("Endpoint chưa được triển khai");
        }

        [HttpPost("RestoreWorkBoardTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RestoreWorkBoardTeacher([FromQuery] Guid id)
        {
            // Cần tạo RestoreWorkBoardTeacherCommand nếu cần khôi phục
            // var result = await Mediator.Send(new RestoreWorkBoardTeacherCommand { Id = id });
            // return result;
            return BadRequest("Endpoint chưa được triển khai");
        }

        [HttpGet("GetDeletedWorkBoardTeachers")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetDeletedWorkBoardTeachers()
        {
            // Cần tạo GetDeletedWorkBoardTeachersQuery nếu cần lấy danh sách đã xóa
            // var result = await Mediator.Send(new GetDeletedWorkBoardTeachersQuery());
            // return result;
            return BadRequest("Endpoint chưa được triển khai");
        }
    }
}