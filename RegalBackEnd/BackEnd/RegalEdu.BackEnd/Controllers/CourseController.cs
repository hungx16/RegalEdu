using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Course.Commands;
using RegalEdu.Application.Course.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class CourseController : BaseController
    {
        private readonly ILogger<CourseController> _logger;
        public CourseController(ILogger<CourseController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddCourse")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddCourse([FromBody] CourseModel courseModel)
        {
            var result = await Mediator.Send (new AddCourseCommand { CourseModel = courseModel });
            return result;
        }

        [HttpPut ("UpdateCourse")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateCourse([FromBody] CourseModel courseModel)
        {
            var result = await Mediator.Send (new UpdateCourseCommand { CourseModel = courseModel });
            return result;
        }


        [HttpDelete ("DeleteListCourses")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListCourses([FromBody] List<string> arrCourseId)
        {
            var result = await Mediator.Send (new DeleteListCoursesCommand { ListIds = arrCourseId });
            return result;
        }


        [HttpGet ("GetCourseById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> GetCourseById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetCourseByIdQuery { Id = id });
            return result;
        }
        [HttpGet ("GetPagedCourses")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedCourses([FromQuery] CourseQuery query)
        {
            var courses = await Mediator.Send (new GetPagedCoursesQuery { CourseQuery = query });
            return courses;
        }

        [HttpGet ("GetAllCourses")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllCourses( )
        {
            var courses = await Mediator.Send (new GetAllCoursesQuery { });
            return courses;
        }

        [AllowAnonymous]
        [HttpGet ("GetAllPublishedCourses")]
        public async Task<ActionResult> GetAllPublishedCourses([FromQuery] string learningRoadMapId)
        => await Mediator.Send (new GetAllPublishCoursesQuery { LearningRoadMapId = learningRoadMapId });

        [HttpGet ("GetPublishedCourseById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> GetPublishedCourseById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetPublishedCourseByIdQuery { Id = id });
            return result;
        }
    }
}
