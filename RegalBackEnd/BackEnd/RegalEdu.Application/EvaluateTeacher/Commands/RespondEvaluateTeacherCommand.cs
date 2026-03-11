using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.EvaluateTeacher.Commands
{
    public class RespondEvaluateTeacherCommand : IRequest<Result>
    {
        public required Guid Id { get; set; }
        public required string ResponseContent { get; set; }

        public class RespondEvaluateTeacherCommandHandler : IRequestHandler<RespondEvaluateTeacherCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly ILocalizationService _localizer;

            public RespondEvaluateTeacherCommandHandler(
                IRegalEducationDbContext context,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            }

            public async Task<Result> Handle(RespondEvaluateTeacherCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.EvaluateTeachers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                if (entity == null)
                {
                    return Result.Failure(_localizer.Format(LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.EvaluateTeacher], request.Id));
                }

                entity.ResponseContent = request.ResponseContent;
                entity.EvaluateDate = DateTime.UtcNow;

                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (success)
                    return Result.Success(_localizer.Format("EvaluateTeacherResponseSuccess", _localizer[EntityName.EvaluateTeacher]));
                else
                    return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer[EntityName.EvaluateTeacher]));
            }
        }
    }
}
