using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Holiday.Commands;
using RegalEdu.Application.Holiday.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class HolidayController : BaseController
    {
        private readonly ILogger<HolidayController> _logger;
        public HolidayController(ILogger<HolidayController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddHoliday")]
        public async Task<ActionResult> AddHoliday([FromBody] HolidayModel model)
            => await Mediator.Send (new AddHolidayCommand { HolidayModel = model });

        [HttpPut ("UpdateHoliday")]
        public async Task<ActionResult> UpdateHoliday([FromBody] HolidayModel model)
            => await Mediator.Send (new UpdateHolidayCommand { HolidayModel = model });

        [HttpDelete ("DeleteListHoliday")]
        public async Task<ActionResult> DeleteListHoliday([FromBody] List<string> ids)
            => await Mediator.Send (new DeleteListHolidayCommand { ListIds = ids });

        [HttpDelete ("RestoreListHoliday")]
        public async Task<ActionResult> RestoreListHoliday([FromBody] List<string> ids)
            => await Mediator.Send (new RestoreListHolidayCommand { ListIds = ids });

        [HttpGet ("GetHolidayById")]
        public async Task<ActionResult> GetHolidayById([FromQuery] string id)
            => await Mediator.Send (new GetHolidayByIdQuery { Id = id });

        [HttpGet ("GetPagedHolidays")]
        public async Task<ActionResult> GetPagedHolidays([FromQuery] HolidayQuery query)
            => await Mediator.Send (new GetPagedHolidaysQuery { HolidayQuery = query });

        [HttpGet ("GetAllHolidays")]
        public async Task<ActionResult> GetAllHolidays( )
            => await Mediator.Send (new GetAllHolidaysQuery { });

        [HttpGet ("GetDeletedHolidays")]
        public async Task<ActionResult> GetDeletedHolidays( )
            => await Mediator.Send (new GetDeletedHolidaysQuery { });
    }
}
