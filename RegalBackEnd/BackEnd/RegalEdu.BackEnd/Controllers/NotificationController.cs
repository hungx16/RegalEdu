using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Api.Hubs;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Notifications.Commands;
using RegalEdu.Application.Notifications.Queries;

namespace RegalEdu.Api.Controllers;

[ApiController]
[Route ("api/RegalEduManagement/[controller]")]
[Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class NotificationController : BaseController
{
    private readonly ILogger<NotificationController> _logger;
    private readonly ILocalizationService _localizer;

    public NotificationController(
        IMediator mediator,
        ILogger<NotificationController> logger,
        ILocalizationService localizer)
        : base (mediator)
    {
        _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
    }

    [HttpPost ("send")]
    public async Task<ActionResult> Send([FromBody] NotificationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest (ModelState);
        }

        _logger.LogInformation ("Creating notification \"{Title}\" (recipient: {Recipient})", request.Title, request.RecipientId);

        return await Mediator.Send (new CreateNotificationCommand
        {
            Payload = request.ToPayload ( ),
            Persist = request.Persist
        });
    }

    [HttpGet ("GetNotifications")]
    public async Task<ActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetCurrentUserId ( );
        if (!userId.HasValue)
        {
            return Unauthorized ( );
        }

        var result = await Mediator.Send (new GetNotificationsQuery
        {
            UserId = userId.Value,
            Page = page,
            PageSize = pageSize
        });

        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest (result.Errors);
        }

        var language = _localizer.GetCurrentLanguage ( );
        if (result.Data.Items.Count > 0)
        {
            foreach (var item in result.Data.Items)
            {
                var localizedTitle = language == "en"
                    ? item.TitleEn ?? item.Title
                    : item.TitleVi ?? item.Title;
                var localizedMessage = language == "en"
                    ? item.MessageEn ?? item.Message
                    : item.MessageVi ?? item.Message;

                item.Title = localizedTitle ?? item.Title;
                item.Message = localizedMessage ?? item.Message;
            }
        }

        return result;
    }

    [HttpPatch ("{id:guid}/read")]
    public async Task<ActionResult> MarkAsRead(Guid id)
    {
        var userId = GetCurrentUserId ( );
        if (!userId.HasValue)
        {
            return Unauthorized ( );
        }

        return await Mediator.Send (new MarkNotificationReadCommand
        {
            NotificationId = id,
            UserId = userId.Value
        });
    }

    [HttpPatch ("read-all")]
    public async Task<ActionResult> MarkAllAsRead( )
    {
        var userId = GetCurrentUserId ( );
        if (!userId.HasValue)
        {
            return Unauthorized ( );
        }

        return await Mediator.Send (new MarkAllNotificationsReadCommand
        {
            UserId = userId.Value
        });
    }
}
