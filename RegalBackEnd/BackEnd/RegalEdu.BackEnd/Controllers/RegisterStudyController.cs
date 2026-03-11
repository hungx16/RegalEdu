using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.RegisterStudy.Commands;
using RegalEdu.Application.RegisterStudy.Queries;

using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class RegisterStudyController : BaseController
    {
        private readonly ILogger<RegisterStudyController> _logger;
        public RegisterStudyController(ILogger<RegisterStudyController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddRegisterStudy")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddRegisterStudy([FromBody] RegisterStudyModel model)
        {
            var result = await Mediator.Send (new AddRegisterStudyCommand { RegisterStudyModel = model });
            return result;
        }

        [HttpPut ("UpdateRegisterStudy")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateRegisterStudy([FromBody] RegisterStudyModel model)
        {
            var result = await Mediator.Send (new UpdateRegisterStudyCommand { RegisterStudyModel = model });
            return result;
        }

        [HttpDelete ("DeleteListRegisterStudy")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListRegisterStudy([FromBody] List<string> arrId)
        {
            var result = await Mediator.Send (new DeleteListRegisterStudyCommand { ListIds = arrId });
            return result;
        }

        //[HttpDelete ("RestoreListRegisterStudy")]
        //[ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        //[ProducesResponseType (StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult> RestoreListRegion([FromBody] List<string> arrRegionId)
        //{
        //    var result = await Mediator.Send (new RestoreListRegionCommand { ListIds = arrRegionId });
        //    return result;
        //}

        [HttpGet ("GetRegisterStudyById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetRegisterStudyById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetRegisterStudyByIdQuery { Id = id });
            return result;
        }

        [HttpGet ("GetPagedRegisterStudy")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedRegisterStudy([FromQuery] RegisterStudyQuery query)
        {
            var regions = await Mediator.Send (new GetPagedRegisterStudysQuery { RegisterStudyQuery = query });
            return regions;
        }

        [HttpGet ("GetAllRegisterStudy")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllRegisterStudy( )
        {
            var regions = await Mediator.Send (new GetAllRegisterStudysQuery { });
            return regions;
        }

        //[HttpGet ("GetDeletedRegisterStudy")]
        //[ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        //[ProducesResponseType (StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult> GetDeletedRegisterStudy( )
        //{
        //    var result = await Mediator.Send (new GetDeletedRegionsQuery { });
        //    return result;
        //}
    }
}
