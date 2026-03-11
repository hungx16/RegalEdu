using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.PartnerType.Commands;
using RegalEdu.Application.PartnerType.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class PartnerTypeController : BaseController
    {
        private readonly ILogger<PartnerTypeController> _logger;
        public PartnerTypeController(ILogger<PartnerTypeController> logger, IConfiguration configuration, IMediator mediator)
            : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("AddPartnerType")]
        public async Task<ActionResult> AddPartnerType([FromBody] PartnerTypeModel model)
            => await Mediator.Send(new AddPartnerTypeCommand { Model = model });

        [HttpPut("UpdatePartnerType")]
        public async Task<ActionResult> UpdatePartnerType([FromBody] PartnerTypeModel model)
            => await Mediator.Send(new UpdatePartnerTypeCommand { Model = model });

        [HttpDelete("DeleteListPartnerType")]
        public async Task<ActionResult> DeleteListPartnerType([FromBody] List<string> ids)
            => await Mediator.Send(new DeleteListPartnerTypeCommand { ListIds = ids });

        [HttpGet("GetPartnerTypeById")]
        public async Task<ActionResult> GetPartnerTypeById([FromBody] string id)
            => await Mediator.Send(new GetPartnerTypeByIdQuery { Id = id });

        [HttpGet("GetPagedPartnerTypes")]
        public async Task<ActionResult> GetPagedPartnerTypes([FromQuery] PartnerTypeQuery query)
            => await Mediator.Send(new GetPagedPartnerTypesQuery { Query = query });

        [HttpGet("GetAllPartnerTypes")]
        public async Task<ActionResult> GetAllPartnerTypes()
            => await Mediator.Send(new GetAllPartnerTypesQuery { });

        [HttpGet("GetDeletedPartnerTypes")]
        public async Task<ActionResult> GetDeletedPartnerTypes()
            => await Mediator.Send(new GetDeletedPartnerTypesQuery { });
    }
}