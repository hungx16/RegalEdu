using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.AllocationEvent.Commands;
using RegalEdu.Application.AllocationEvent.Queries;

using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.Request;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class AllocationEventController : BaseController
    {
        private readonly ILogger<AllocationEventController> _logger;
        public AllocationEventController(ILogger<AllocationEventController> logger, IConfiguration configuration, IMediator mediator)
            : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("GetAllocationEventById")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllocationEventById([FromQuery] string id)
        {
            var result = await Mediator.Send(new GetAllocationEventByIdQuery { Id = id });
            return result;
        }

        [HttpGet("GetPagedAllocationEvents")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedAllocationEvents([FromQuery] AllocationEventQuery query)
        {
            var result = await Mediator.Send(new GetPagedAllocationEventsQuery { AllocationEventQuery = query });
            return result;
        }

        [HttpGet("GetAllAllocationEvents")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllAllocationEvents()
        {
            var result = await Mediator.Send(new GetAllAllocationEventsQuery { });
            return result;
        }

        //Bổ sung
        [HttpPost("AddAllocationEventWithDetails")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddAllocationEventWithDetails([FromBody] AllocationEventModel allocationEventModel)
        {
            // Gọi đến Command mới để thêm đồng thời AllocationEvent + AllocationDetailEvent
            var result = await Mediator.Send(new AddAllocationEventWithDetailsCommand
            {
                AllocationEventModel = allocationEventModel
            });

            return result;
        }

        [HttpPut("UpdateAllocationEventWithDetails")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateAllocationEventWithDetails([FromBody] AllocationEventModel allocationEventModel)
        {
            // Gọi đến Command mới để cập nhật đồng thời AllocationEvent + AllocationDetailEvent
            var result = await Mediator.Send(new UpdateAllocationEventWithDetailsCommand
            {
                AllocationEventModel = allocationEventModel
            });

            return result;
        }

        [HttpDelete("DeleteListAllocationEventWithDetails")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListAllocationEventWithDetails([FromBody] List<string> arrAllocationEventId)
        {
            var result = await Mediator.Send(new DeleteListAllocationEventWithDetailsCommand
            {
                ListIds = arrAllocationEventId
            });

            if (result.Succeeded)
                return Ok(result);
            else
                return BadRequest(result);
        }
        //Hiển thị tổng hợp theo giao diện 7.1.2
        [HttpGet("GetAllocationEventSummaries")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllocationEventSummaries()
        {
            var result = await Mediator.Send(new GetAllocationEventSummariesQuery());
            return result;
        }


        [HttpGet("GetAllAllocationEventsForRegion")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllAllocationEventsForRegion()
        {
            var result = await Mediator.Send(new GetAllAllocationEventsForRegionQuery { });
            return result;
        }
        [HttpGet("GetAllAllocationEventsForCompany")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllAllocationEventsForCompany()
        {
            var result = await Mediator.Send(new GetAllAllocationEventsForCompanyQuery { });
            return result;
        }


        [HttpPost("CreateCompanyEventProposal")]
        public async Task<ActionResult> CreateCompanyEventProposal([FromBody] CompanyEventProposalRequest request)
        {
            var result = await Mediator.Send(new CreateProposalCommand
            {
                CompanyEventProposalRequest = request
            });

            return result;
        }
        [HttpPut("UpdateCompanyEventProposal")]
        public async Task<ActionResult> UpdateCompanyEventProposal([FromBody] CompanyEventProposalRequest request)
        {
            var result = await Mediator.Send(new UpdateProposalCommand
            {
                CompanyEventProposalRequest = request
            });
            return result;
        }

        [HttpGet("GetAllCompanyEventProposal")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllCompanyEventProposal()
        {
            var result = await Mediator.Send(new GetAllCompanyEventProposalQuery { });
            return result;
        }

        [HttpGet("GetAllCompanyEventReports")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllCompanyEventReports()
        {
            var result = await Mediator.Send(new GetAllCompanyEventReportsQuery { });
            return result;
        }

        [HttpGet("GetCompanyEventReportsByCompanyEventId")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetCompanyEventReportsByCompanyEventId([FromQuery] string companyEventId)
        {
            var result = await Mediator.Send(new GetCompanyEventReportsByCompanyEventIdQuery
            {
                CompanyEventId = companyEventId
            });
            return result;
        }

        [HttpPost("CreateCompanyEventReport")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> CreateCompanyEventReport([FromBody] CompanyEventReportModel request)
        {
            var result = await Mediator.Send(new CreateCompanyEventReportCommand
            {
                CompanyEventReportModel = request
            });

            return result;
        }

        [HttpPut("UpdateCompanyEventReport")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateCompanyEventReport([FromBody] CompanyEventReportModel request)
        {
            var result = await Mediator.Send(new UpdateCompanyEventReportCommand
            {
                CompanyEventReportModel = request
            });

            return result;
        }

        [HttpPost("ApproveCompanyEventProposal")]
        public async Task<ActionResult> ApproveCompanyEventProposal([FromBody] ApproveCompanyEventModel request)
        {
            var result = await Mediator.Send(new ApproveCompanyEventProposalCommand
            {
                ApproveCompanyEventModel = request
            });

            return result;
        }

        [HttpPost("UpdateStatusOfCompanyEventProposal")]
        public async Task<ActionResult> UpdateStatusOfCompanyEventProposal([FromBody] ApproveCompanyEventModel request)
        {
            var result = await Mediator.Send(new UpdateStatusOfCompanyEventProposalCommand
            {
                ApproveCompanyEventModel = request
            });

            return result;
        }

        [HttpPost("ApproveCompanyEventReport")]
        public async Task<ActionResult> ApproveCompanyEventReport([FromBody] ApproveCompanyEventReportModel request)
        {
            var result = await Mediator.Send(new ApproveCompanyEventReportCommand
            {
                ApproveCompanyEventReportModel = request
            });

            return result;
        }

        [HttpPost("UpdateStatusOfCompanyEventReport")]
        public async Task<ActionResult> UpdateStatusOfCompanyEventReport([FromBody] ApproveCompanyEventReportModel request)
        {
            var result = await Mediator.Send(new UpdateStatusOfCompanyEventReportCommand
            {
                ApproveCompanyEventReportModel = request
            });

            return result;
        }
    }
}
