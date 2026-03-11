using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Event.Commands
{
    // Command: Yêu cầu xóa nhiều sự kiện theo danh sách Id
    public class DeleteListEventCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; } // Danh sách Id cần xóa (chuỗi Guid dạng string)
    }

    // Handler: Xử lý việc xóa danh sách sự kiện
    public class DeleteListEventCommandHandler : IRequestHandler<DeleteListEventCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<DeleteListEventCommandHandler> _logger;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDeleteService;

        public DeleteListEventCommandHandler(
            IRegalEducationDbContext context,
            ILogger<DeleteListEventCommandHandler> logger,
            ILocalizationService localizer,
            ISoftDeleteService softDeleteService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _softDeleteService = softDeleteService ?? throw new ArgumentNullException(nameof(softDeleteService));
        }

        public async Task<Result> Handle(DeleteListEventCommand request, CancellationToken cancellationToken)
        {
            // Trường hợp danh sách Id rỗng → không có gì để xóa
            if (request.ListIds == null || !request.ListIds.Any())
                return Result.Failure(_localizer.Format(LocalizationKey.NoModelToDelete, EntityName.Event));

            int successCount = 0; // Đếm số bản ghi xóa thành công
            int failCount = 0;    // Đếm số bản ghi xóa thất bại
            var failMessages = new List<string>(); // Lưu thông báo lỗi chi tiết từng bản ghi

            foreach (var id in request.ListIds)
            {
                // Tìm sự kiện theo Id
                var eventEntity = _context.Events.FirstOrDefault(x => x.Id.ToString() == id);

                if (eventEntity != null)
                {
                    // Xóa mềm (có hỗ trợ đệ quy: xóa cả các entity con liên quan)
                    var result = await _softDeleteService.RecursiveSoftDelete(eventEntity.Id, typeof(Domain.Entities.Event));

                    if (result.Succeeded)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        var deleteFailMsg = _localizer.Format(
                            LocalizationKey.EntityDeleteFailed,
                            _localizer[EntityName.Event], // Tên entity: "Sự kiện"
                            eventEntity.EventName,        // Tên sự kiện cụ thể
                            string.Join(", ", result.Errors) // Ghép chuỗi các lỗi trả về
                        );
                        failMessages.Add(deleteFailMsg);
                        _logger.LogWarning(deleteFailMsg);
                    }
                }
                else
                {
                    // Không tìm thấy sự kiện với Id này
                    failCount++;
                    var notFoundMsg = _localizer.Format(
                        LocalizationKey.EntityWithIdNotFound,
                        _localizer[EntityName.Event], // "Sự kiện"
                        id                            // Id không tồn tại
                    );
                    failMessages.Add(notFoundMsg);
                    _logger.LogWarning(notFoundMsg);
                }
            }

            // Tạo thông điệp tổng kết (vd: "Xóa Sự kiện: 3 thành công, 2 thất bại")
            var msg = _localizer.Format(
                LocalizationKey.MSG_DELETE_RESULT,
                _localizer[EntityName.Event], // Chú ý: phải là Event, không phải Division
                successCount,
                failCount
            );

            // Nếu có chi tiết lỗi thì nối thêm vào
            if (failMessages.Any())
                msg += "\n" + string.Join("\n", failMessages);

            // Trả kết quả: nếu có ít nhất 1 thành công thì coi như Success
            if (successCount > 0)
                return Result.Success(msg);
            else
                return Result.Failure(msg);
        }
    }
}
