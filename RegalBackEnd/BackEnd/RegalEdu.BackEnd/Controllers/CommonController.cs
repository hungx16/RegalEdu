using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Queries;
using RegalEdu.Application.Common.Request;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class CommonController : BaseController
    {
        private readonly ILogger<CommonController> _logger;
        public CommonController(ILogger<CommonController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }
        [HttpGet ("GenerateCode")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GenerateCode([FromQuery] GenerateCodeRequest request)
        {
            var result = await Mediator.Send (new GenerateCodeQuery { GenerateCodeRequest = request });
            return result;
        }
        [AllowAnonymous]
        [HttpGet ("Provinces")]
        [ProducesResponseType (typeof (List<Province>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetProvinces( )
        {
            var result = await Mediator.Send (new GetProvincesQuery ( ));
            return result;
        }
        [AllowAnonymous]
        [HttpGet ("Wards")]
        [ProducesResponseType (typeof (List<Ward>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetWards([FromQuery] string? provinceCode)
        {
            var result = await Mediator.Send (new GetWardsQuery { ProvinceCode = provinceCode });
            return result;
        }

        [HttpGet ("DocumentTypes")]
        [ProducesResponseType (typeof (List<DocumentType>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetDocumentTypes( )
        {
            var result = await Mediator.Send (new GetDocumentTypesQuery ( ));
            return result;
        }
        [HttpGet ("GetWebsiteKeys")]
        [ProducesResponseType (typeof (List<WebsiteKey>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTagWebsiteKeys( )
        {
            var result = await Mediator.Send (new GetWebsiteKeysQuery ( ));
            return result;
        }

        [HttpGet ("GetEnWebsiteKeys")]
        [ProducesResponseType (typeof (List<WebsiteKey>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetEnTagWebsiteKeys( )
        {
            var result = await Mediator.Send (new GetEnWebsiteKeysQuery ( ));
            return result;
        }
    }
}
