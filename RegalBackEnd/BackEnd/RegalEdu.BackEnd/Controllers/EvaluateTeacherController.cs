using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.EvaluateTeacher.Commands;
using RegalEdu.Application.EvaluateTeacher.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class EvaluateTeacherController : BaseController
    {
        private readonly ILogger<EvaluateTeacherController> _logger;

        public EvaluateTeacherController(
            ILogger<EvaluateTeacherController> logger,
            IMediator mediator) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("AddEvaluateTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddEvaluateTeacher([FromBody] EvaluateTeacherModel model)
        {
            var result = await Mediator.Send(new AddEvaluateTeacherCommand { EvaluateTeacherModel = model });
            return result;
        }

        [HttpPut("UpdateEvaluateTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateEvaluateTeacher([FromBody] EvaluateTeacherModel model)
        {
            var result = await Mediator.Send(new UpdateEvaluateTeacherCommand { EvaluateTeacherModel = model });
            return result;
        }

        [HttpDelete("DeleteEvaluateTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteEvaluateTeacher([FromQuery] Guid id)
        {
            var result = await Mediator.Send(new DeleteEvaluateTeacherCommand { Id = id });
            return result;
        }

        [HttpPost("RespondEvaluateTeacher")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RespondEvaluateTeacher([FromBody] RespondEvaluateTeacherCommand command)
        {
            var result = await Mediator.Send(command);
            return result;
        }

        [HttpGet("GetEvaluateTeacherById")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetEvaluateTeacherById([FromQuery] Guid id)
        {
            var result = await Mediator.Send(new GetEvaluateTeacherByIdQuery { Id = id });
            return result;
        }

        [HttpGet("GetPagedEvaluateTeachers")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedEvaluateTeachers([FromQuery] EvaluateTeacherQuery query)
        {
            var result = await Mediator.Send(new GetPagedEvaluateTeachersQuery { EvaluateTeacherQuery = query });
            return result;
        }

        [HttpGet("GetAllEvaluateTeachers")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllEvaluateTeachers()
        {
            var result = await Mediator.Send(new GetAllEvaluateTeachersQuery());
            return result;
        }

        [HttpGet("GetTeacherEvaluations")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetTeacherEvaluations([FromQuery] Guid teacherId)
        {
            var result = await Mediator.Send(new GetTeacherEvaluationsQuery { TeacherId = teacherId });
            return result;
        }


        [HttpGet("GetEvaluateTeacherSummary")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetEvaluateTeacherSummary(
            [FromQuery] Guid? teacherId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var result = await Mediator.Send(new GetEvaluateTeacherSummaryQuery
            {
                TeacherId = teacherId,
                FromDate = fromDate,
                ToDate = toDate
            });
            return result;
        }
    }
}