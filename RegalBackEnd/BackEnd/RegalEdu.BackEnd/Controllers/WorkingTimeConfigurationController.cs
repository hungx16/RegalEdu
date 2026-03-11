using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.WorkingTimeConfiguration.Commands;
using RegalEdu.Application.WorkingTimeConfiguration.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class WorkingTimeConfigurationController : BaseController
    {
        private readonly ILogger<WorkingTimeConfigurationController> _logger;
        public WorkingTimeConfigurationController(ILogger<WorkingTimeConfigurationController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddWorkingTimeConfiguration")]
        public async Task<ActionResult> AddWorkingTimeConfiguration([FromBody] WorkingTimeConfigurationModel model)
            => await Mediator.Send (new AddWorkingTimeConfigurationCommand { WorkingTimeConfigurationModel = model });

        [HttpPut ("UpdateWorkingTimeConfiguration")]
        public async Task<ActionResult> UpdateWorkingTimeConfiguration([FromBody] WorkingTimeConfigurationModel model)
            => await Mediator.Send (new UpdateWorkingTimeConfigurationCommand { WorkingTimeConfigurationModel = model });

        [HttpDelete ("DeleteListWorkingTimeConfiguration")]
        public async Task<ActionResult> DeleteListWorkingTimeConfiguration([FromBody] List<string> ids)
            => await Mediator.Send (new DeleteListWorkingTimeConfigurationCommand { ListIds = ids });

        [HttpDelete ("RestoreListWorkingTimeConfiguration")]
        public async Task<ActionResult> RestoreListWorkingTimeConfiguration([FromBody] List<string> ids)
            => await Mediator.Send (new RestoreListWorkingTimeConfigurationCommand { ListIds = ids });

        [HttpGet ("GetWorkingTimeConfigurationById")]
        public async Task<ActionResult> GetWorkingTimeConfigurationById([FromQuery] string id)
            => await Mediator.Send (new GetWorkingTimeConfigurationByIdQuery { Id = id });

        [HttpGet ("GetPagedWorkingTimeConfigurations")]
        public async Task<ActionResult> GetPagedWorkingTimeConfigurations([FromQuery] WorkingTimeConfigurationQuery query)
            => await Mediator.Send (new GetPagedWorkingTimeConfigurationsQuery { WorkingTimeConfigurationQuery = query });

        [HttpGet ("GetAllWorkingTimeConfigurations")]
        public async Task<ActionResult> GetAllWorkingTimeConfigurations( )
            => await Mediator.Send (new GetAllWorkingTimeConfigurationsQuery { });

        [HttpGet ("GetDeletedWorkingTimeConfigurations")]
        public async Task<ActionResult> GetDeletedWorkingTimeConfigurations( )
            => await Mediator.Send (new GetDeletedWorkingTimeConfigurationsQuery { });
    }
}
