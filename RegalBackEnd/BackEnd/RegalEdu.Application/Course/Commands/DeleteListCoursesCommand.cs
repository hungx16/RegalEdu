using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Course.Commands
{
    public class DeleteListCoursesCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    public class DeleteListCoursesCommandHandler : IRequestHandler<DeleteListCoursesCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<DeleteListCoursesCommandHandler> _logger;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDeleteService;

        public DeleteListCoursesCommandHandler(IRegalEducationDbContext context, ILogger<DeleteListCoursesCommandHandler> logger, ILocalizationService localizer, ISoftDeleteService softDeleteService)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            _softDeleteService = softDeleteService ?? throw new ArgumentNullException (nameof (softDeleteService));
        }

        public async Task<Result> Handle(DeleteListCoursesCommand request, CancellationToken cancellationToken)
        {
            if (request.ListIds == null || !request.ListIds.Any ( ))
                return Result.Failure (_localizer.Format (LocalizationKey.NoModelToDelete, _localizer[EntityName.Course]));

            int successCount = 0;
            int failCount = 0;
            var failMessages = new List<string> ( );

            foreach (var id in request.ListIds)
            {
                var course = _context.Courses.FirstOrDefault (x => x.Id.ToString ( ) == id);
                if (course != null)
                {
                    var result = await _softDeleteService.RecursiveSoftDelete (course.Id, typeof (Domain.Entities.Course));
                    if (result.Succeeded)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        var deleteFailMsg = _localizer.Format (
                            LocalizationKey.EntityDeleteFailed,
                            _localizer[EntityName.Course], course.CourseName, result.Errors
                        );
                        failMessages.Add (deleteFailMsg);
                        _logger.LogWarning (deleteFailMsg);
                    }
                }
                else
                {
                    failCount++;
                    var notFoundMsg = _localizer.Format (
                        LocalizationKey.EntityWithIdNotFound,
                        _localizer[EntityName.Course], id
                    );
                    failMessages.Add (notFoundMsg);
                    _logger.LogWarning (notFoundMsg);
                }
            }

            // Thông điệp tổng hợp đã localize hoàn toàn
            var msg = _localizer.Format (
                LocalizationKey.MSG_DELETE_RESULT,
                _localizer[EntityName.Course], successCount, failCount
            ); // "Xóa LearningRoadmap: 3 thành công, 2 thất bại."
            if (failMessages.Any ( ))
                msg += "\n" + string.Join ("\n", failMessages);

            if (successCount > 0)
                return Result.Success (msg);
            else
                return Result.Failure (msg);
        }
    }
}