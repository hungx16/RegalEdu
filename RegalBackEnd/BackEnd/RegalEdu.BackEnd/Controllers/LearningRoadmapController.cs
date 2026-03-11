using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.LearningRoadmap.Queries;
using RegalEdu.Application.LearningRoadMap.Commands;
using RegalEdu.Application.LearningRoadMap.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class LearningRoadMapController : BaseController
    {
        private readonly ILogger<LearningRoadMapController> _logger;
        public LearningRoadMapController(ILogger<LearningRoadMapController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddLearningRoadMap")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddLearningRoadMap([FromBody] LearningRoadMapModel learningRoadMapModel)
        {
            var result = await Mediator.Send (new AddLearningRoadMapCommand { LearningRoadMapModel = learningRoadMapModel });
            return result;
        }

        [HttpPut ("UpdateLearningRoadMap")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateLearningRoadMap([FromBody] LearningRoadMapModel learningRoadMapModel)
        {
            var result = await Mediator.Send (new UpdateLearningRoadMapCommand { LearningRoadMapModel = learningRoadMapModel });
            return result;
        }

        //[HttpPut ("RestoreListLearningRoadMaps")]
        //[ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        //[ProducesResponseType (StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult> RestoreListLearningRoadMaps([FromBody] List<string> arrLearningRoadMapId)
        //{
        //    var result = await Mediator.Send (new RestoreListLearningRoadMapsCommand { ListIds = arrLearningRoadMapId });
        //    return result;
        //}

        [HttpDelete ("DeleteListLearningRoadMaps")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListLearningRoadMaps([FromBody] List<string> arrLearningRoadMapId)
        {
            var result = await Mediator.Send (new DeleteListLearningRoadMapsCommand { ListIds = arrLearningRoadMapId });
            return result;
        }


        [HttpGet ("GetLearningRoadMapById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> GetLearningRoadMapById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetLearningRoadMapByIdQuery { Id = id });
            return result;
        }
        [HttpGet ("GetPagedLearningRoadMaps")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedLearningRoadMaps([FromQuery] LearningRoadMapQuery query)
        {
            var learningRoadMaps = await Mediator.Send (new GetPagedLearningRoadMapsQuery { LearningRoadMapQuery = query });
            return learningRoadMaps;
        }

        [HttpGet ("GetAllLearningRoadMaps")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllLearningRoadMaps( )
        {
            var learningRoadMaps = await Mediator.Send (new GetAllLearningRoadMapsQuery { });
            return learningRoadMaps;
        }

        //[HttpGet ("GetDeletedLearningRoadMaps")]
        //[ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        //[ProducesResponseType (StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult> GetDeletedLearningRoadMaps( )
        //{
        //    var result = await Mediator.Send (new GetDeletedLearningRoadMapsQuery { });
        //    return result;
        //}
        [AllowAnonymous]
        [HttpGet ("GetAllPublishedLearningRoadMaps")]
        public async Task<ActionResult> GetAllPublishedLearningRoadMaps( )
         => await Mediator.Send (new GetAllPublishedLearningRoadMapsQuery { });

        [AllowAnonymous]
        [HttpGet ("GetPublishedLearningRoadMapById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> GetPublishedLearningRoadMapById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetPublishedLearningRoadMapByIdQuery { Id = id });
            return result;
        }
    }
}
