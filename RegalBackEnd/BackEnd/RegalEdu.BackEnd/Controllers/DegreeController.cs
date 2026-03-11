using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Degree.Commands;
using RegalEdu.Application.Degree.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class DegreeController : BaseController
    {
        private readonly ILogger<DegreeController> _logger;
        public DegreeController(ILogger<DegreeController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddDegree")]
        public async Task<ActionResult> AddDegree([FromBody] DegreeModel model)
            => await Mediator.Send (new AddDegreeCommand { DegreeModel = model });

        [HttpPut ("UpdateDegree")]
        public async Task<ActionResult> UpdateDegree([FromBody] DegreeModel model)
            => await Mediator.Send (new UpdateDegreeCommand { DegreeModel = model });

        [HttpDelete ("DeleteListDegree")]
        public async Task<ActionResult> DeleteListDegree([FromBody] List<string> ids)
            => await Mediator.Send (new DeleteListDegreeCommand { ListIds = ids });

        [HttpDelete ("RestoreListDegree")]
        public async Task<ActionResult> RestoreListDegree([FromBody] List<string> ids)
            => await Mediator.Send (new RestoreListDegreeCommand { ListIds = ids });

        [HttpGet ("GetDegreeById")]
        public async Task<ActionResult> GetDegreeById([FromQuery] string id)
            => await Mediator.Send (new GetDegreeByIdQuery { Id = id });

        [HttpGet ("GetPagedDegrees")]
        public async Task<ActionResult> GetPagedDegrees([FromQuery] DegreeQuery query)
            => await Mediator.Send (new GetPagedDegreesQuery { DegreeQuery = query });

        [HttpGet ("GetAllDegrees")]
        public async Task<ActionResult> GetAllDegrees( )
            => await Mediator.Send (new GetAllDegreesQuery { });

        [HttpGet ("GetDeletedDegrees")]
        public async Task<ActionResult> GetDeletedDegrees( )
            => await Mediator.Send (new GetDeletedDegreesQuery { });
    }
}
