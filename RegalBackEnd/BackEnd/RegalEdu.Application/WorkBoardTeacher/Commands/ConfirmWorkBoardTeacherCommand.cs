using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.WorkBoardTeacher.Commands
{
    /// <summary>
    /// Xác nhận công
    /// </summary>
    public class ConfirmWorkBoardTeacherCommand : IRequest<Result>
    {
        public required Guid Id { get; set; }
        public required string ConfirmedBy { get; set; }

        public class ConfirmWorkBoardTeacherCommandHandler : IRequestHandler<ConfirmWorkBoardTeacherCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly ILocalizationService _localizer;

            public ConfirmWorkBoardTeacherCommandHandler(
                IRegalEducationDbContext context,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            }

            public async Task<Result> Handle(ConfirmWorkBoardTeacherCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.WorkBoardTeachers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                if (entity == null)
                {
                    return Result.Failure(_localizer.Format(LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.WorkBoardTeacher], request.Id));
                }

                entity.IsConfirmed = true;
                entity.ConfirmedBy = request.ConfirmedBy;

                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (success)
                    return Result.Success(_localizer.Format("WorkBoardTeacherConfirmSuccess", _localizer[EntityName.WorkBoardTeacher]));
                else
                    return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer[EntityName.WorkBoardTeacher]));
            }
        }
    }
}