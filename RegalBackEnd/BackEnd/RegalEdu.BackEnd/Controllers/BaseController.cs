// Ignore Spelling: Api Edu

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RegalEdu.Api.Controllers
{
    [Route("api/RegalEduManagement/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IMediator _mediator;

        protected BaseController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        protected ActionResult ApiOk<T>(T data)
        {
            if (data != null && data is Application.Common.Results.Result result)
            {
                if (result.GetStatus())
                {
                    return Ok(new { Succeeded = true, Error = "", Data = result.Data });
                }
                else
                {
                    return Ok(new { Succeeded = false, Error = result.GetError() });
                }
            }
            return Ok(new { Succeeded = true, Error = "", Data = data });
        }

        protected ActionResult ApiBadRequest<T>(T data)
        {
            return BadRequest(data);
        }

    protected string GetUserName()
    {
        Claim? claim = User.Claims.FirstOrDefault(x => x.Type == "preferred_username");
        string userName = "";
        if (claim != null)
        {
            userName = claim.Value;
        }

        return userName;
    }

    protected Guid? GetCurrentUserId()
    {
        string? value = User.Claims.FirstOrDefault (x => x.Type == ClaimTypes.NameIdentifier)?.Value
                         ?? User.Claims.FirstOrDefault (x => x.Type == "sub")?.Value
                         ?? User.Claims.FirstOrDefault (x => x.Type == "userId")?.Value;

        if (Guid.TryParse (value, out var id))
        {
            return id;
        }

        return null;
    }

        protected IMediator Mediator
        {
            get
            {
                if (_mediator == null)
                {
                    var mediator = HttpContext.RequestServices.GetService<IMediator>();
                    if (mediator == null)
                    {
                        throw new InvalidOperationException("IMediator service is not registered.");
                    }
                    _mediator = mediator;
                }
                return _mediator;
            }
        }
    }
}
