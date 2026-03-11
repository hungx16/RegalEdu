using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Common.Request;
using RegalEdu.Application.SupportingDocument.Commands;
using RegalEdu.Application.SupportingDocument.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class SupportingDocumentController : BaseController
    {
        private readonly ILogger<SupportingDocumentController> _logger;
        public SupportingDocumentController(ILogger<SupportingDocumentController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddSupportingDocument")]
        public async Task<ActionResult> AddSupportingDocument([FromBody] SupportingDocumentModel model)
            => await Mediator.Send (new AddSupportingDocumentCommand { SupportingDocumentModel = model });

        [HttpPut ("UpdateSupportingDocument")]
        public async Task<ActionResult> UpdateSupportingDocument([FromBody] SupportingDocumentModel model)
            => await Mediator.Send (new UpdateSupportingDocumentCommand { SupportingDocumentModel = model });

        [HttpDelete ("DeleteListSupportingDocument")]
        public async Task<ActionResult> DeleteListSupportingDocument([FromBody] DeleteSupportingDocumentRequest req)
            => await Mediator.Send (new DeleteListSupportingDocumentCommand { ListIds = req.Ids, WebsiteKeys = req.WebsiteKeys });

        [HttpDelete ("RestoreListSupportingDocument")]
        public async Task<ActionResult> RestoreListSupportingDocument([FromBody] DeleteSupportingDocumentRequest req)
            => await Mediator.Send (new RestoreListSupportingDocumentCommand { ListIds = req.Ids, WebsiteKeys = req.WebsiteKeys });

        [HttpGet ("GetSupportingDocumentById")]
        public async Task<ActionResult> GetSupportingDocumentById([FromQuery] string id)
            => await Mediator.Send (new GetSupportingDocumentByIdQuery { Id = id });

        [HttpGet ("GetPagedSupportingDocuments")]
        public async Task<ActionResult> GetPagedSupportingDocuments([FromQuery] SupportingDocumentQuery query)
            => await Mediator.Send (new GetPagedSupportingDocumentsQuery { SupportingDocumentQuery = query });

        [HttpGet ("GetAllSupportingDocuments")]
        public async Task<ActionResult> GetAllSupportingDocuments( )
            => await Mediator.Send (new GetAllSupportingDocumentsQuery { });

        [HttpGet ("GetDeletedSupportingDocuments")]
        public async Task<ActionResult> GetDeletedSupportingDocuments( )
            => await Mediator.Send (new GetDeletedSupportingDocumentsQuery { });
        [AllowAnonymous]
        [HttpGet ("GetAllPublishedSupportingDocuments")]
        public async Task<ActionResult> GetAllPublishedSupportingDocuments( )
         => await Mediator.Send (new GetAllPublishedSupportingDocumentsQuery { });
    }
}
