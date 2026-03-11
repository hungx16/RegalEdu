using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Student.Commands;
using RegalEdu.Application.Student.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class StudentController : BaseController
    {
        private readonly ILogger<StudentController> _logger;
        public StudentController(ILogger<StudentController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddStudent")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddStudent([FromBody] StudentModel model)
        {
            var result = await Mediator.Send (new AddStudentCommand { StudentModel = model });
            return result;
        }

        //[HttpPut ("UpdateStudent")]
        //[ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        //[ProducesResponseType (StatusCodes.Status400BadRequest)]
        //[ProducesResponseType (StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult> UpdateStudent([FromBody] StudentModel teacherModel)
        //{
        //    var result = await Mediator.Send (new UpdateStudentCommand { StudentModel = teacherModel });
        //    return result;
        //}

        [HttpPut ("UpdateStudent")]
        public async Task<ActionResult> UpdateStudent([FromBody] StudentModel model)
            => await Mediator.Send (new UpdateStudentCommand { StudentModel = model });

        [HttpDelete ("DeleteListStudent")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListStudent([FromBody] List<string> arrStudentId)
        {
            var result = await Mediator.Send (new DeleteListStudentCommand { ListIds = arrStudentId });
            return result;
        }

        [HttpDelete ("RestoreListStudent")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RestoreListStudent([FromBody] List<string> arrStudentId)
        {
            var result = await Mediator.Send (new RestoreListStudentCommand { ListIds = arrStudentId });
            return result;
        }

        [HttpGet ("GetStudentById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetStudentById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetStudentByIdQuery { Id = id });
            return result;
        }
        [HttpGet ("GetStudentByStudentCode")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetStudentByStudentCode([FromQuery] string code)
        {
            var result = await Mediator.Send (new GetStudentByStudentCodeQuery { Code = code });
            return result;
        }
        [HttpGet ("GetCoursesWithClassByStudentId")]
        [ProducesResponseType (typeof (ActionResult<List<StudentCourseProgressModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetCoursesWithClassByStudentId([FromQuery] string studentId)
        {
            var result = await Mediator.Send (new GetCoursesWithClassByStudentIdQuery { StudentId = studentId });
            return result;
        }
        [HttpGet ("GetStudentClassDetail")]
        [ProducesResponseType (typeof (ActionResult<StudentClassDetailModel>), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetStudentClassDetail([FromQuery] string studentId, [FromQuery] string classId)
        {
            var result = await Mediator.Send (new GetStudentClassDetailQuery { StudentId = studentId, ClassId = classId });
            return result;
        }
        [HttpPost ("SubmitTeacherFeedback")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> SubmitTeacherFeedback([FromBody] SubmitStudentTeacherFeedbackCommand command)
        {
            var result = await Mediator.Send (command);
            return result;
        }
        [HttpGet ("GetStudentClasses")]
        [ProducesResponseType (typeof (ActionResult<List<StudentClassItemModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetStudentClasses([FromQuery] string studentId)
        {
            var result = await Mediator.Send (new GetStudentClassesQuery { StudentId = studentId });
            return result;
        }
        [HttpGet ("GetStudentTimetable")]
        [ProducesResponseType (typeof (ActionResult<List<StudentTimetableItemModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetStudentTimetable([FromQuery] string studentId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var result = await Mediator.Send (new GetStudentTimetableQuery { StudentId = studentId, FromDate = fromDate, ToDate = toDate });
            return result;
        }
        [HttpGet ("GetPagedStudents")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedStudents([FromQuery] StudentQuery query)
        {
            var teachers = await Mediator.Send (new GetPagedStudentsQuery { StudentQuery = query });
            return teachers;
        }
        [HttpGet ("GetPagedCustoms")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedCustoms([FromQuery] CustomQuery query)
        {
            var teachers = await Mediator.Send (new GetPagedCustomQuery { StudentQuery = query });
            return teachers;
        }
        [HttpGet ("GetAllStudents")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllStudents( )
        {
            var teachers = await Mediator.Send (new GetAllStudentsQuery { });
            return teachers;
        }
        [HttpGet ("GetAllCustoms")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllCustoms( )
        {
            var teachers = await Mediator.Send (new GetAllCustomQuery { });
            return teachers;
        }
        [HttpGet ("GetDeletedStudents")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetDeletedStudents( )
        {
            var result = await Mediator.Send (new GetDeletedStudentsQuery { });
            return result;
        }
        [AllowAnonymous]
        [HttpPost ("AddPotentialCustomers")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddPotentialCustomers([FromBody] StudentModel model)
        {
            var result = await Mediator.Send (new AddPotentialCustomersCommand { StudentModel = model });
            return result;
        }
    }
}
