using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Event.Commands;
using RegalEdu.Application.Event.Queries;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class EventController : BaseController
    {
        private readonly ILogger<EventController> _logger;
        public EventController(ILogger<EventController> logger, IConfiguration configuration, IMediator mediator)
            : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("AddEvent")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddEvent([FromBody] EventModel eventModel)
        {
            var result = await Mediator.Send(new AddEventCommand { EventModel = eventModel });
            return result;
        }

        [HttpPut("UpdateEvent")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateEvent([FromBody] EventModel eventModel)
        {
            var result = await Mediator.Send(new UpdateEventCommand { EventModel = eventModel });
            return result;
        }


        [HttpDelete("DeleteListEvents")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListEvents([FromBody] List<string> arrEventId)
        {
            var result = await Mediator.Send(new DeleteListEventCommand{ ListIds = arrEventId });
            return result;
        }


        [HttpGet("GetEventById")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> GetEventById([FromQuery] string id)
        {
            var result = await Mediator.Send(new GetEventByIdQuery { Id = id });
            return result;
        }
        [HttpGet("GetPagedEvents")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedEvents([FromQuery] EventQuery query)
        {
            var events = await Mediator.Send(new GetPagedEventsQuery { EventQuery = query });
            return events;
        }

        [HttpGet("GetAllEvents")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllEvents()
        {
            var events = await Mediator.Send(new GetAllEventsQuery {                
            });
            return events;
        }
    }
}
