using MediatR;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.AccountGroupPermission.Commands;
using RegalEdu.Application.AccountGroupPermission.Queries;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models.Request;


namespace RegalEdu.Api.Controllers
{
    //[Authorize]
    public class AccountGroupPermissionController : BaseController
    {
        readonly ILogger<AccountGroupPermissionController> _logger;
        protected readonly ICurrentUserService _userService;
        private readonly IUserPermissionInfoService _userPermissionInfoService;
        public AccountGroupPermissionController(IMediator mediator, ILogger<AccountGroupPermissionController> logger, ICurrentUserService userService,
            IUserPermissionInfoService userPermissionInfoService) : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _userService = userService ?? throw new ArgumentNullException (nameof (userService));
            _userPermissionInfoService = userPermissionInfoService ?? throw new ArgumentNullException (nameof (userPermissionInfoService));
        }
        [HttpGet ("GetMenuAccept")]
        [ProducesResponseType (typeof (List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<string>>> GetMenuAccept( )
        {
            return ApiOk (await _userPermissionInfoService.GetMenuAcceptByEmployee (_userService.UserCode));

        }
        [HttpGet ("GetMenuAndPermissionAccept")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetMenuAndPermissionAccept( )
        {
            var result = await _userPermissionInfoService.GetMenuAndPermissionByEmployee (_userService.UserCode);
            return result;
        }
        [HttpPost ("SaveAccountGroupPermission")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType (typeof (AccountGroupPermissionRequestModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> SaveAccountGroupPermission(AccountGroupPermissionRequestModel groupPermission)
        {
            var result = await Mediator.Send (new SaveAccountGroupPermissionCommand { RequestModel = groupPermission });
            return result;
        }
        [HttpGet ("GetAccountGroupPermissionByAccountGroupId")]
        [ProducesResponseType (typeof (Result), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAccountGroupPermissionByAccountGroupId(string accountGroupId)
        {
            var result = await Mediator.Send (new GetAccountGroupPermissionByGroupIdQuery ( ) { AccountGroupId = accountGroupId });
            return result;
        }


    }
}
