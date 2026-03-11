using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Company.Commands;
using RegalEdu.Application.Company.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class CompanyController : BaseController
    {
        private readonly ILogger<CompanyController> _logger;
        public CompanyController(ILogger<CompanyController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddCompany")]
        public async Task<ActionResult> AddCompany([FromBody] CompanyModel model)
            => await Mediator.Send (new AddCompanyCommand { CompanyModel = model, });

        [HttpPost ("CreateLogRegionCom")]
        public async Task<ActionResult> CreateLogRegionCom([FromBody] LogRegionComModel model)
           => await Mediator.Send (new CreateLogRegionComCommand { LogRegionComModel = model });

        [HttpPut ("UpdateCompany")]
        public async Task<ActionResult> UpdateCompany([FromBody] CompanyModel model)
            => await Mediator.Send (new UpdateCompanyCommand { CompanyModel = model });

        [HttpDelete ("DeleteListCompany")]
        public async Task<ActionResult> DeleteListCompany([FromBody] List<string> ids)
            => await Mediator.Send (new DeleteListCompanyCommand { ListIds = ids });

        [HttpDelete ("RestoreListCompany")]
        public async Task<ActionResult> RestoreListCompany([FromBody] List<string> ids)
            => await Mediator.Send (new RestoreListCompanyCommand { ListIds = ids });

        [HttpGet ("GetCompanyById")]
        public async Task<ActionResult> GetCompanyById([FromQuery] string id)
            => await Mediator.Send (new GetCompanyByIdQuery { Id = id });

        [HttpGet ("GetPagedCompanies")]
        public async Task<ActionResult> GetPagedCompanies([FromQuery] CompanyQuery query)
            => await Mediator.Send (new GetPagedCompaniesQuery { CompanyQuery = query });

        [HttpGet ("GetAllCompanies")]
        public async Task<ActionResult> GetAllCompanies( )
            => await Mediator.Send (new GetAllCompaniesQuery { });

        [HttpGet ("GetDeletedCompanies")]
        public async Task<ActionResult> GetDeletedCompanies( )
            => await Mediator.Send (new GetDeletedCompaniesQuery { });

        [HttpGet ("GetAllCompanyRegions")]
        public async Task<ActionResult> GetAllCompanyRegions( )
            => await Mediator.Send (new GetAllCompanyRegionsQuery { });

        [AllowAnonymous]
        [HttpGet ("GetAllPublishedCompanies")]
        public async Task<ActionResult> GetAllPublishedCompanies( )
        => await Mediator.Send (new GetAllPublishCompaniesQuery { });
    }
}
