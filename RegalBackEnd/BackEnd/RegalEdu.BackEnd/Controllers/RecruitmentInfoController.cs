using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.RecruitmentInfo.Commands;
using RegalEdu.Application.RecruitmentInfo.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class RecruitmentInfoController : BaseController
    {
        private readonly ILogger<RecruitmentInfoController> _logger;

        public RecruitmentInfoController(ILogger<RecruitmentInfoController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddRecruitmentInfo")]
        public async Task<ActionResult> AddRecruitmentInfo([FromBody] RecruitmentInfoModel model)
            => await Mediator.Send (new AddRecruitmentInfoCommand { RecruitmentInfoModel = model });

        [HttpPut ("UpdateRecruitmentInfo")]
        public async Task<ActionResult> UpdateRecruitmentInfo([FromBody] RecruitmentInfoModel model)
            => await Mediator.Send (new UpdateRecruitmentInfoCommand { RecruitmentInfoModel = model });

        [HttpDelete ("DeleteListRecruitmentInfo")]
        public async Task<ActionResult> DeleteListRecruitmentInfo([FromBody] List<string> ids)
            => await Mediator.Send (new DeleteListRecruitmentInfoCommand { ListIds = ids });

        [HttpDelete ("RestoreListRecruitmentInfo")]
        public async Task<ActionResult> RestoreListRecruitmentInfo([FromBody] List<string> ids)
            => await Mediator.Send (new RestoreListRecruitmentInfoCommand { ListIds = ids });

        [HttpGet ("GetRecruitmentInfoById")]
        public async Task<ActionResult> GetRecruitmentInfoById([FromBody] string id)
            => await Mediator.Send (new GetRecruitmentInfoByIdQuery { Id = id });

        [HttpGet ("GetAllRecruitmentInfo")]
        public async Task<ActionResult> GetAllRecruitmentInfo( )
            => await Mediator.Send (new GetAllRecruitmentInfoQuery ( ));

        [HttpGet ("GetDeletedRecruitmentInfo")]
        public async Task<ActionResult> GetDeletedRecruitmentInfo( )
            => await Mediator.Send (new GetDeletedRecruitmentInfoQuery ( ));

        [HttpGet ("GetPagedRecruitmentInfo")]
        public async Task<ActionResult> GetPagedRecruitmentInfo([FromQuery] RecruitmentInfoQuery query)
            => await Mediator.Send (new GetPagedRecruitmentInfoQuery { Query = query });
        [AllowAnonymous]
        [HttpGet ("GetAllPublishedRecruitmentInfo")]
        public async Task<ActionResult> GetAllPublishedRecruitmentInfo( )
          => await Mediator.Send (new GetAllPublishedRecruitmentInfoQuery ( ));
    }
}
