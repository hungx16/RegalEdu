using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.EvaluateTeacher.Commands
{
    public class DeleteEvaluateTeacherCommand : IRequest<Result>
    {
        public required Guid Id { get; set; }

        public class DeleteEvaluateTeacherCommandHandler : IRequestHandler<DeleteEvaluateTeacherCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly ILogger<DeleteEvaluateTeacherCommandHandler> _logger;
            private readonly ILocalizationService _localizer;
            private readonly ISoftDeleteService _softDeleteService;

            public DeleteEvaluateTeacherCommandHandler(
                IRegalEducationDbContext context,
                ILogger<DeleteEvaluateTeacherCommandHandler> logger,
                ILocalizationService localizer,
                ISoftDeleteService softDeleteService)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
                _softDeleteService = softDeleteService ?? throw new ArgumentNullException(nameof(softDeleteService));
            }

            public async Task<Result> Handle(DeleteEvaluateTeacherCommand request, CancellationToken cancellationToken)
            {
                var entity = _context.EvaluateTeachers.FirstOrDefault(x => x.Id == request.Id);
                if (entity == null)
                {
                    return Result.Failure(_localizer.Format(LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.EvaluateTeacher], request.Id));
                }

                var result = await _softDeleteService.RecursiveSoftDelete(entity.Id, typeof(Domain.Entities.EvaluateTeacher));
                if (result.Succeeded)
                {
                    return Result.Success(_localizer.Format(LocalizationKey.MSG_DELETE_SUCCESS, _localizer[EntityName.EvaluateTeacher]));
                }
                else
                {
                    return Result.Failure(_localizer.Format(LocalizationKey.EntityDeleteFailed, _localizer[EntityName.EvaluateTeacher], entity.Id, result.Errors));
                }
            }
        }
    }
}