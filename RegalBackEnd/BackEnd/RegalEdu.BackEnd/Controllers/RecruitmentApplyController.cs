using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.RecruitmentApply.Commands;
using RegalEdu.Application.RecruitmentApply.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class RecruitmentApplyController : BaseController
    {
        private readonly ILogger<RecruitmentApplyController> _logger;
        public RecruitmentApplyController(ILogger<RecruitmentApplyController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }
        [AllowAnonymous]
        [HttpPost ("AddRecruitmentApply")]
        public async Task<ActionResult> Add([FromBody] RecruitmentApplyModel model)
            => await Mediator.Send (new AddRecruitmentApplyCommand { RecruitmentApplyModel = model });

        [HttpPut ("UpdateRecruitmentApply")]
        public async Task<ActionResult> Update([FromBody] RecruitmentApplyModel model)
            => await Mediator.Send (new UpdateRecruitmentApplyCommand { RecruitmentApplyModel = model });

        [HttpDelete ("DeleteListRecruitmentApply")]
        public async Task<ActionResult> Delete([FromBody] List<string> ids)
            => await Mediator.Send (new DeleteListRecruitmentApplyCommand { ListIds = ids });

        [HttpDelete ("RestoreListRecruitmentApply")]
        public async Task<ActionResult> Restore([FromBody] List<string> ids)
            => await Mediator.Send (new RestoreListRecruitmentApplyCommand { ListIds = ids });

        [HttpGet ("GetRecruitmentApplyById")]
        public async Task<ActionResult> GetById([FromBody] string id)
            => await Mediator.Send (new GetRecruitmentApplyByIdQuery { Id = id });

        [HttpGet ("GetAllRecruitmentApplies")]
        public async Task<ActionResult> GetAll( )
            => await Mediator.Send (new GetAllRecruitmentAppliesQuery ( ));

        [HttpGet ("GetDeletedRecruitmentApplies")]
        public async Task<ActionResult> GetDeleted( )
            => await Mediator.Send (new GetDeletedRecruitmentAppliesQuery ( ));

        [HttpGet ("GetPagedRecruitmentApplies")]
        public async Task<ActionResult> GetPaged([FromQuery] RecruitmentApplyQuery query)
            => await Mediator.Send (new GetPagedRecruitmentAppliesQuery { Query = query });
    }
}
