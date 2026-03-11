using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.TransferCompany.Commands;
using RegalEdu.Application.TransferCompany.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    /// <summary>
    /// Controller quản lý Phiếu Chuyển Chi Nhánh của Học viên
    /// </summary>
    [Authorize]
    public class TransferCompanyController : BaseController
    {
        private readonly ILogger<TransferCompanyController> _logger;

        public TransferCompanyController(
            ILogger<TransferCompanyController> logger,
            IConfiguration configuration,
            IMediator mediator)
            : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ============================================================
        // 1. TẠO MỚI PHIẾU CHUYỂN CHI NHÁNH
        // ============================================================
        [HttpPost("AddTransferCompany")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddTransferCompany(
            [FromBody] TransferCompanyModel transferCompanyModel)
        {
            var result = await Mediator.Send(
                new AddTransferCompanyCommand
                {
                    TransferCompanyModel = transferCompanyModel
                });

            return result;
        }

        // ============================================================
        // 2. CẬP NHẬT PHIẾU CHUYỂN (SỬA THÔNG TIN NHÁP)
        // ============================================================
        [HttpPut("UpdateTransferCompany")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateTransferCompany(
            [FromBody] TransferCompanyModel transferCompanyModel)
        {
            var result = await Mediator.Send(
                new UpdateTransferCompanyCommand
                {
                    TransferCompanyModel = transferCompanyModel
                });

            return result;
        }


        // ============================================================
        // 3. CHUYỂN TRẠNG THÁI PHIẾU CHUYỂN CHI NHÁNH
        // ============================================================
        [HttpPut("ChangeTransferCompanyStatus")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> ChangeTransferCompanyStatus(
            [FromBody] ChangeTransferCompanyStatusCommand command)
        {
            var result = await Mediator.Send(command);
            return result;
        }

         //============================================================
         //4. XÓA DANH SÁCH PHIẾU CHUYỂN(SOFT DELETE)
         //============================================================
        [HttpDelete("DeleteListTransferCompany")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListTransferCompanies(
            [FromBody] List<string> arrTransferCompanyId)
        {
            var result = await Mediator.Send(
                new DeleteListTransferCompanyCommand
                {
                    ListIds = arrTransferCompanyId
                });

            return result;
        }

       
         //============================================================
         //5. LẤY THÔNG TIN PHIẾU CHUYỂN THEO ID
         //============================================================
        [HttpGet("GetTransferCompanyById")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetTransferCompanyById(
            [FromQuery] Guid id)
        {
            var result = await Mediator.Send(
                new GetTransferCompanyByIdQuery
                {
                    Id = id
                });

            return result;
        }
        

         //============================================================
         //6. LẤY TOÀN BỘ PHIẾU CHUYỂN(CHƯA BỊ XÓA)
         //============================================================
        [HttpGet("GetAllTransferCompanies")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllTransferCompanies()
        {
            var result = await Mediator.Send(
                new GetAllTransferCompaniesQuery());

            return result;
        }

        //============================================================
        // 7. LẤY DANH SÁCH PHIẾU CHUYỂN THEO CHI NHÁNH
        //    (Chi nhánh gốc hoặc chi nhánh đích)
        //============================================================
        [HttpGet("GetAllTransferCompaniesByCompanyId/{companyId}")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllTransferCompaniesByCompanyId(Guid companyId)
        {
            var result = await Mediator.Send(
                new GetAllTransferCompaniesByCompanyIdQuery
                {
                    CompanyId = companyId
                });

            return result;
        }


    }
}
