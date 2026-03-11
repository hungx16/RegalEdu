using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.LectureType.Commands;
using RegalEdu.Application.LectureType.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class LectureTypeController : BaseController
    {
        private readonly ILogger<LectureTypeController> _logger;
        public LectureTypeController(ILogger<LectureTypeController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddLectureType")]
        public async Task<ActionResult> AddLectureType([FromBody] LectureTypeModel model)
            => await Mediator.Send (new AddLectureTypeCommand { LectureTypeModel = model });

        [HttpPut ("UpdateLectureType")]
        public async Task<ActionResult> UpdateLectureType([FromBody] LectureTypeModel model)
            => await Mediator.Send (new UpdateLectureTypeCommand { LectureTypeModel = model });

        [HttpDelete ("DeleteListLectureType")]
        public async Task<ActionResult> DeleteListLectureType([FromBody] List<string> ids)
            => await Mediator.Send (new DeleteListLectureTypeCommand { ListIds = ids });

        [HttpDelete ("RestoreListLectureType")]
        public async Task<ActionResult> RestoreListLectureType([FromBody] List<string> ids)
            => await Mediator.Send (new RestoreListLectureTypeCommand { ListIds = ids });

        [HttpGet ("GetLectureTypeById")]
        public async Task<ActionResult> GetLectureTypeById([FromQuery] string id)
            => await Mediator.Send (new GetLectureTypeByIdQuery { Id = id });

        [HttpGet ("GetPagedLectureTypes")]
        public async Task<ActionResult> GetPagedLectureTypes([FromQuery] LectureTypeQuery query)
            => await Mediator.Send (new GetPagedLectureTypesQuery { LectureTypeQuery = query });

        [HttpGet ("GetAllLectureTypes")]
        public async Task<ActionResult> GetAllLectureTypes( )
            => await Mediator.Send (new GetAllLectureTypesQuery { });

        [HttpGet ("GetDeletedLectureTypes")]
        public async Task<ActionResult> GetDeletedLectureTypes( )
            => await Mediator.Send (new GetDeletedLectureTypesQuery { });
    }
}
