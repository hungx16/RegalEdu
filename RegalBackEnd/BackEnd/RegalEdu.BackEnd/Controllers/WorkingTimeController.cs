using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.WorkingTime.Commands;
using RegalEdu.Application.WorkingTime.Queries;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class WorkingTimeController : BaseController
    {
        private readonly ILogger<WorkingTimeController> _logger;
        public WorkingTimeController(ILogger<WorkingTimeController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddWorkingTime")]
        public async Task<ActionResult> AddWorkingTime([FromBody] WorkingTimeModel model)
            => await Mediator.Send (new AddWorkingTimeCommand { WorkingTimeModel = model });

        [HttpPut ("UpdateWorkingTime")]
        public async Task<ActionResult> UpdateWorkingTime([FromBody] WorkingTimeModel model)
            => await Mediator.Send (new UpdateWorkingTimeCommand { WorkingTimeModel = model });

        [HttpDelete ("DeleteListWorkingTime")]
        public async Task<ActionResult> DeleteListWorkingTime([FromBody] List<string> ids)
            => await Mediator.Send (new DeleteListWorkingTimeCommand { ListIds = ids });

        [HttpDelete ("RestoreListWorkingTime")]
        public async Task<ActionResult> RestoreListWorkingTime([FromBody] List<string> ids)
            => await Mediator.Send (new RestoreListWorkingTimeCommand { ListIds = ids });

        [HttpGet ("GetWorkingTimeById")]
        public async Task<ActionResult> GetWorkingTimeById([FromBody] string id)
            => await Mediator.Send (new GetWorkingTimeByIdQuery { Id = id });

        [HttpGet ("GetPagedWorkingTimes")]
        public async Task<ActionResult> GetPagedWorkingTimes([FromQuery] WorkingTimeQuery query)
            => await Mediator.Send (new GetPagedWorkingTimesQuery { WorkingTimeQuery = query });

        [HttpGet ("GetAllWorkingTimes")]
        public async Task<ActionResult> GetAllWorkingTimes( )
            => await Mediator.Send (new GetAllWorkingTimesQuery { });

        [HttpGet ("GetDeletedWorkingTimes")]
        public async Task<ActionResult> GetDeletedWorkingTimes( )
            => await Mediator.Send (new GetDeletedWorkingTimesQuery { });

        [HttpGet ("GetAllWorkingTimesByConfigId")]
        public async Task<ActionResult> GetAllWorkingTimesByConfigId([FromQuery] string configurationId)
           => await Mediator.Send (new GetAllWorkingTimesByConfigIdQuery { ConfigurationId = configurationId });
    }
}
