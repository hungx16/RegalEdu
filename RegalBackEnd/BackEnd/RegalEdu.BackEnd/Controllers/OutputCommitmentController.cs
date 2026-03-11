using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.OutputCommitment.Commands;
using RegalEdu.Application.OutputCommitment.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class OutputCommitmentController : BaseController
    {
        private readonly ILogger<OutputCommitmentController> _logger;
        private readonly IOutputCommitmentPdfService _pdfService;
        public OutputCommitmentController(
            ILogger<OutputCommitmentController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IOutputCommitmentPdfService pdfService)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _pdfService = pdfService ?? throw new ArgumentNullException (nameof (pdfService)); ;
        }

        [HttpPost ("AddOutputCommitment")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddOutputCommitment([FromBody] OutputCommitmentModel model)
        {
            var result = await Mediator.Send (new AddOutputCommitmentCommand { OutputCommitmentModel = model });
            return result;
        }

        [HttpPut ("UpdateOutputCommitment")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateOutputCommitment([FromBody] OutputCommitmentModel model)
        {
            var result = await Mediator.Send (new UpdateOutputCommitmentCommand { OutputCommitmentModel = model });
            return result;
        }

        [HttpDelete ("DeleteListOutputCommitment")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListOutputCommitment([FromBody] List<string> ids)
        {
            var result = await Mediator.Send (new DeleteListOutputCommitmentCommand { ListIds = ids });
            return result;
        }



        [HttpGet ("GetOutputCommitmentById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetOutputCommitmentById([FromBody] string id)
        {
            var result = await Mediator.Send (new GetOutputCommitmentByIdQuery { Id = id });
            return result;
        }


        [HttpGet ("GetAllOutputCommitments")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllOutputCommitments( )
        {
            var result = await Mediator.Send (new GetAllOutputCommitmentsQuery { });
            return result;
        }

        [HttpGet ("GetPdfForOutputCommitment")]
        public async Task<IActionResult> GetPdfForOutputCommitment([FromQuery] OutputCommitmentModel model, CancellationToken ct)
        {
            var pdfBytes = await _pdfService.GeneratePdfAsync (model, ct);

            var fileName = $"CamKetDauRa_{model.StudentCode:N}.pdf";
            return File (pdfBytes, "application/pdf", fileName);
        }
    }
}
