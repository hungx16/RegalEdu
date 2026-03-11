using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.LearningRoadMap.Commands
{
    public class RestoreListLearningRoadMapsCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    public class RestoreListLearningRoadMapsCommandHandler : IRequestHandler<RestoreListLearningRoadMapsCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<RestoreListLearningRoadMapsCommandHandler> _logger;
        private readonly ILocalizationService _localizer;

        public RestoreListLearningRoadMapsCommandHandler(IRegalEducationDbContext context, ILogger<RestoreListLearningRoadMapsCommandHandler> logger, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(RestoreListLearningRoadMapsCommand request, CancellationToken cancellationToken)
        {
            // 1. Kiểm tra danh sách id có dữ liệu không
            if (request.ListIds == null || !request.ListIds.Any ( ))
                return Result.Failure (_localizer.Format (LocalizationKey.NoModelToRestore, EntityName.LearningRoadMap));

            int successCount = 0;
            int failCount = 0;
            var failMessages = new List<string> ( );

            // 2. Duyệt từng id để xử lý restore
            foreach (var id in request.ListIds)
            {
                var learningRoadMap = await _context.LearningRoadMaps
                    .IgnoreQueryFilters ( )
                    .FirstOrDefaultAsync (x => x.Id.ToString ( ) == id, cancellationToken);

                // 2.1 Không tìm thấy
                if (learningRoadMap == null)
                {
                    failCount++;
                    var notFoundMsg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, EntityName.LearningRoadMap, id);
                    failMessages.Add (notFoundMsg);
                    _logger.LogWarning (notFoundMsg);
                    continue;
                }

                // 2.2 Không ở trạng thái deleted
                if (!learningRoadMap.IsDeleted)
                {
                    failCount++;
                    var notDeletedMsg = _localizer.Format (LocalizationKey.EntityNotDeleted, EntityName.LearningRoadMap, learningRoadMap.Id);
                    failMessages.Add (notDeletedMsg);
                    continue;
                }

                // 2.3 Kiểm tra AgeGroupId có tồn tại trong Category không
                bool ageGroupExists = await _context.Categories
                    .AnyAsync (c => c.Id == learningRoadMap.AgeGrId && c.CategoryType == (byte)CategoryType.AgeGroup, cancellationToken);

                if (!ageGroupExists)
                {
                    failCount++;
                    var invalidAgeGroupMsg = _localizer.Format (LocalizationKey.InvalidCategoryType, EntityName.Category, CategoryType.AgeGroup);
                    failMessages.Add (invalidAgeGroupMsg);
                    continue;
                }

                // 2.4 Restore
                learningRoadMap.IsDeleted = false;
                // learningRoadMap.DeletedAt = null;
                // learningRoadMap.DeletedBy = null;
                successCount++;
                _context.LearningRoadMaps.Update (learningRoadMap);
            }

            // 3. Save DB
            var dbResult = await _context.SaveChangesAsync (cancellationToken) > 0;

            // 4. Tổng hợp kết quả
            string mainMsg = _localizer.Format (
                LocalizationKey.MSG_RESTORE_RESULT,
                EntityName.LearningRoadMap,
                successCount,
                failCount
            );

            if (failMessages.Any ( ))
                mainMsg += " " + string.Join (" ", failMessages);

            return (dbResult && successCount > 0)
                ? Result.Success (mainMsg)
                : Result.Failure (mainMsg);
        }

    }
}