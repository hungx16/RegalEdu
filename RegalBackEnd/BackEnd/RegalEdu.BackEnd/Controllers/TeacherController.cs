using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Teacher.Commands;
using RegalEdu.Application.Teacher.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class TeacherController : BaseController
    {
        private readonly ILogger<TeacherController> _logger;
        public TeacherController(ILogger<TeacherController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddTeacher")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddTeacher([FromBody] TeacherModel teacherModel)
        {
            var result = await Mediator.Send (new AddTeacherCommand { TeacherModel = teacherModel });
            return result;
        }

        [HttpPut ("UpdateTeacher")]
        public async Task<ActionResult> UpdateTeacher([FromBody] TeacherModel model)
            => await Mediator.Send (new UpdateTeacherCommand { TeacherModel = model });

        [HttpDelete ("DeleteListTeacher")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListTeacher([FromBody] List<string> arrTeacherId)
        {
            var result = await Mediator.Send (new DeleteListTeacherCommand { ListIds = arrTeacherId });
            return result;
        }

        [HttpDelete ("RestoreListTeacher")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RestoreListTeacher([FromBody] List<string> arrTeacherId)
        {
            var result = await Mediator.Send (new RestoreListTeacherCommand { ListIds = arrTeacherId });
            return result;
        }

        [HttpGet ("GetTeacherById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetTeacherById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetTeacherByIdQuery { Id = id });
            return result;
        }

        [HttpGet ("GetPagedTeachers")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedTeachers([FromQuery] TeacherQuery query)
        {
            var teachers = await Mediator.Send (new GetPagedTeachersQuery { TeacherQuery = query });
            return teachers;
        }

        [HttpGet ("GetAllTeachers")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllTeachers( )
        {
            var teachers = await Mediator.Send (new GetAllTeachersQuery { });
            return teachers;
        }

        [HttpGet ("GetDeletedTeachers")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetDeletedTeachers( )
        {
            var result = await Mediator.Send (new GetDeletedTeachersQuery { });
            return result;
        }

        [HttpGet ("IsCurrentUserTeacher")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> IsCurrentUserTeacher( )
        {
            var result = await Mediator.Send (new IsCurrentUserTeacherQuery { });
            return result;
        }

    }
}
