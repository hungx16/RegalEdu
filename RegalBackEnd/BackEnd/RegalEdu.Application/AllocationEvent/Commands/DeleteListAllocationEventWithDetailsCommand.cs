using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;

namespace RegalEdu.Application.AllocationEvent.Commands
{
    /// <summary>
    /// Command dùng để xóa cứng danh sách AllocationEvent và toàn bộ AllocationDetailEvent liên quan.
    /// </summary>
    public class DeleteListAllocationEventWithDetailsCommand : IRequest<Result>
    {
        /// <summary>
        /// Danh sách ID của các AllocationEvent cần xóa.
        /// </summary>
        public required List<string> ListIds { get; set; }
    }

    /// <summary>
    /// Handler thực thi lệnh xóa danh sách AllocationEvent và các chi tiết liên quan.
    /// </summary>
    public class DeleteListAllocationEventWithDetailsCommandHandler
        : IRequestHandler<DeleteListAllocationEventWithDetailsCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<DeleteListAllocationEventWithDetailsCommandHandler> _logger;
        private readonly ILocalizationService _localizer;

        public DeleteListAllocationEventWithDetailsCommandHandler(
            IRegalEducationDbContext context,
            ILogger<DeleteListAllocationEventWithDetailsCommandHandler> logger,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(DeleteListAllocationEventWithDetailsCommand request, CancellationToken cancellationToken)
        {
            if (request.ListIds == null || !request.ListIds.Any ( ))
                return Result.Failure (_localizer.Format (LocalizationKey.NoModelToDelete, EntityName.AllocationEvent));

            int successCount = 0;
            int failCount = 0;
            var failMessages = new List<string> ( );

            // ✅ Transaction để đảm bảo toàn vẹn dữ liệu
            var dbContext = (DbContext)_context;
            using var transaction = await dbContext.Database.BeginTransactionAsync (cancellationToken);

            try
            {
                foreach (var id in request.ListIds)
                {
                    //Chỉ ở trạng thái Draft mới xóa
                    var allocationEvent = await _context.AllocationEvents
                        .FirstOrDefaultAsync (x => x.Id.ToString ( ) == id && x.AllocationEventStatus == AllocationEventStatus.Draft, cancellationToken);

                    if (allocationEvent == null)
                    {
                        failCount++;
                        var notFoundMsg = _localizer.Format (LocalizationKey.EntityWithIdNotFound,
                            _localizer[EntityName.AllocationEvent], id);
                        failMessages.Add (notFoundMsg);
                        _logger.LogWarning (notFoundMsg);
                        continue;
                    }

                    // 🔹 Xóa tất cả AllocationDetailEvent con
                    var detailEvents = await _context.AllocationDetailEvents
                        .Where (d => d.AllocationEventId == allocationEvent.Id)
                        .ToListAsync (cancellationToken);

                    if (detailEvents.Any ( ))
                    {
                        _context.AllocationDetailEvents.RemoveRange (detailEvents);
                        _logger.LogInformation ("Hard delete {Count} AllocationDetailEvent(s) for AllocationEvent ID {Id}",
                            detailEvents.Count, allocationEvent.Id);
                    }

                    // 🔹 Xóa luôn AllocationEvent cha
                    _context.AllocationEvents.Remove (allocationEvent);
                    _logger.LogInformation ("Hard delete AllocationEvent ID {Id}", allocationEvent.Id);

                    successCount++;
                }

                // ✅ Lưu thay đổi và commit transaction
                await dbContext.SaveChangesAsync (cancellationToken);
                await transaction.CommitAsync (cancellationToken);

                // ✅ Trả kết quả
                var msg = _localizer.Format (
                    LocalizationKey.MSG_DELETE_RESULT,
                    _localizer[EntityName.AllocationEvent],
                    successCount,
                    failCount
                );

                if (failMessages.Any ( ))
                    msg += "\n" + string.Join ("\n", failMessages);

                return Result.Success (msg);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync (cancellationToken);
                _logger.LogError (ex, "Error occurred while hard deleting AllocationEvent with details.");
                return Result.Failure (_localizer.Format (LocalizationKey.EntityDeleteFailed,
                    _localizer[EntityName.AllocationEvent], ex.Message));
            }
        }
    }
}
