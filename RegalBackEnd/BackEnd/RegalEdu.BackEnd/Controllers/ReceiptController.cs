using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.Department.Queries;
using RegalEdu.Application.Receipt.Commands;
using RegalEdu.Application.Receipt.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class ReceiptController : BaseController
    {
        private readonly ILogger<ReceiptController> _logger;
        public ReceiptController(ILogger<ReceiptController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddReceipt")]
        public async Task<ActionResult> AddReceipt([FromBody] ReceiptsModel ReceiptModel)
        {
            var result = await Mediator.Send (new AddReceiptCommand { ReceiptModel = ReceiptModel });
            return result;
        }

        [HttpPut ("UpdateReceipt")]
        public async Task<ActionResult> UpdateReceipt([FromBody] ReceiptsModel ReceiptModel)
        {
            var result = await Mediator.Send (new UpdateReceiptCommand { ReceiptModel = ReceiptModel });
            return result;
        }

        [HttpDelete ("DeleteListReceipt")]
        public async Task<ActionResult> DeleteListReceipt([FromBody] List<string> arrReceiptId)
        {
            var result = await Mediator.Send (new DeleteListReceiptCommand { ListIds = arrReceiptId });
            return result;
        }

        //[HttpDelete ("RestoreListDepartment")]
        //public async Task<ActionResult> RestoreListDepartment([FromBody] List<string> arrDepartmentId)
        //{
        //    var result = await Mediator.Send (new RestoreListDepartmentCommand { ListIds = arrDepartmentId });
        //    return result;
        //}

        [HttpGet ("GetPagedReceipts")]
        public async Task<ActionResult> GetPagedReceipts([FromQuery] ReceiptQuery query)
        {
            var result = await Mediator.Send (new GetPagedReceiptsQuery { ReceiptQuery = query });
            return result;
        }

        [HttpGet ("GetReceiptsByStudentId")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetReceiptsByStudentId([FromQuery] string studentId)
        {
            var result = await Mediator.Send (new GetReceiptByStudentIdQuery { StudentId = studentId });
            return result;
        }

        [HttpGet ("GetAllReceipts")]
        public async Task<ActionResult> GetAllReceipts( )
        {
            var result = await Mediator.Send (new GetAllReceiptsQuery { });
            return result;
        }

        //[HttpGet ("GetDeletedDepartments")]
        //[ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        //[ProducesResponseType (StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult> GetDeletedDepartments( )
        //{
        //    var result = await Mediator.Send (new GetDeletedDepartmentsQuery { });
        //    return result;
        //}

        [HttpGet ("GetReceiptById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> GetReceiptById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetReceiptByIdQuery { Id = id });
            return result;
        }
    }
}
