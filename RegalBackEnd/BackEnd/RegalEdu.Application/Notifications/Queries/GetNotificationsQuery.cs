using System;
using MediatR;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.Notifications.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Notifications.Queries;

public class GetNotificationsQuery : IRequest<Result<NotificationListModel>>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public class Handler : IRequestHandler<GetNotificationsQuery, Result<NotificationListModel>>
    {
        private readonly INotificationRepository _notificationRepository;

        public Handler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException (nameof (notificationRepository));
        }

        public async Task<Result<NotificationListModel>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var validPage = Math.Max (1, request.Page);
            var validPageSize = Math.Max (1, request.PageSize);
            var skip = (validPage - 1) * validPageSize;

            var notifications = await _notificationRepository.GetNotificationsForRecipientAsync (
                request.UserId, skip, validPageSize, cancellationToken);

            var total = await _notificationRepository.CountNotificationsForRecipientAsync (request.UserId, cancellationToken);

            var models = notifications
                .Select (n => new NotificationModel
                {
                    Id = n.Id,
                    RecipientId = n.RecipientId,
                    Title = n.Title,
                    TitleVi = n.TitleVi,
                    TitleEn = n.TitleEn,
                    Message = n.Message,
                    MessageVi = n.MessageVi,
                    MessageEn = n.MessageEn,
                    Payload = n.Payload,
                    Type = n.Type,
                    Channel = n.Channel,
                    Status = n.Status,
                    IsRead = n.IsRead,
                    SentAt = n.SentAt,
                    DeliveredAt = n.DeliveredAt,
                    ReadAt = n.ReadAt,
                    CreatedAt = n.CreatedAt
                })
                .ToList ( );

            return Result<NotificationListModel>.Success (new NotificationListModel
            {
                Items = models,
                TotalCount = total
            });
        }
    }
}
