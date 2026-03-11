using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.WorkBoardTeacher.Commands
{
    public class DeleteWorkBoardTeacherCommand : IRequest<Result>
    {
        public required Guid Id { get; set; }

        public class DeleteWorkBoardTeacherCommandHandler : IRequestHandler<DeleteWorkBoardTeacherCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly ILogger<DeleteWorkBoardTeacherCommandHandler> _logger;
            private readonly ILocalizationService _localizer;
            private readonly ISoftDeleteService _softDeleteService;

            public DeleteWorkBoardTeacherCommandHandler(
                IRegalEducationDbContext context,
                ILogger<DeleteWorkBoardTeacherCommandHandler> logger,
                ILocalizationService localizer,
                ISoftDeleteService softDeleteService)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
                _softDeleteService = softDeleteService ?? throw new ArgumentNullException(nameof(softDeleteService));
            }

            public async Task<Result> Handle(DeleteWorkBoardTeacherCommand request, CancellationToken cancellationToken)
            {
                var entity = _context.WorkBoardTeachers.FirstOrDefault(x => x.Id == request.Id);
                if (entity == null)
                {
                    return Result.Failure(_localizer.Format(LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.WorkBoardTeacher], request.Id));
                }

                var result = await _softDeleteService.RecursiveSoftDelete(entity.Id, typeof(Domain.Entities.WorkBoardTeacher));
                if (result.Succeeded)
                {
                    return Result.Success(_localizer.Format(LocalizationKey.MSG_DELETE_SUCCESS, _localizer[EntityName.WorkBoardTeacher]));
                }
                else
                {
                    return Result.Failure(_localizer.Format(LocalizationKey.EntityDeleteFailed, _localizer[EntityName.WorkBoardTeacher], entity.Id, result.Errors));
                }
            }
        }
    }
}