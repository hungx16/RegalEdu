using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Tuition.Commands;
using RegalEdu.Application.Tuition.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class TuitionController : BaseController
    {
        private readonly ILogger<TuitionController> _logger;
        public TuitionController(ILogger<TuitionController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddTuition")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddTuition([FromBody] TuitionModel tuitionModel)
        {
            var result = await Mediator.Send (new AddTuitionCommand { TuitionModel = tuitionModel });
            return result;
        }

        [HttpPut ("UpdateTuition")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateTuition([FromBody] TuitionModel tuitionModel)
        {
            var result = await Mediator.Send (new UpdateTuitionCommand { TuitionModel = tuitionModel });
            return result;
        }

        [HttpDelete ("DeleteListTuition")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListTuition([FromBody] List<string> arrTuitionId)
        {
            var result = await Mediator.Send (new DeleteListTuitionCommand { ListIds = arrTuitionId });
            return result;
        }

        [HttpDelete ("RestoreListTuition")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RestoreListTuition([FromBody] List<string> arrTuitionId)
        {
            var result = await Mediator.Send (new RestoreListTuitionCommand { ListIds = arrTuitionId });
            return result;
        }

        [HttpGet ("GetTuitionById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetTuitionById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetTuitionByIdQuery { Id = id });
            return result;
        }

        [HttpGet ("GetTuitionByCourseIdAndClassTypeId")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetTuitionByCourseIdAndClassTypeId([FromQuery] string courseId, string classTypeId)
        {
            var result = await Mediator.Send (new GetTuitionByCourseIdAndClassTypeIdQuery { CourseId = courseId, ClassTypeId = classTypeId });
            return result;
        }
        [HttpGet ("GetPagedTuitions")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedTuitions([FromQuery] TuitionQuery query)
        {
            var tuitions = await Mediator.Send (new GetPagedTuitionsQuery { TuitionQuery = query });
            return tuitions;
        }

        [HttpGet ("GetAllTuitions")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllTuitions( )
        {
            var tuitions = await Mediator.Send (new GetAllTuitionQuery { });
            return tuitions;
        }

        [HttpGet ("GetDeletedTuitions")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetDeletedTuitions( )
        {
            var result = await Mediator.Send (new GetDeletedTuitionQuery { });
            return result;
        }
    }
}
