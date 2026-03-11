using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.PayrollTeacher.Commands
{
    public class MarkAsPaidPayrollTeacherCommand : IRequest<Result>
    {
        public required Guid Id { get; set; }
        public DateTime? PaidDate { get; set; }

        public class MarkAsPaidPayrollTeacherCommandHandler : IRequestHandler<MarkAsPaidPayrollTeacherCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly ILocalizationService _localizer;

            public MarkAsPaidPayrollTeacherCommandHandler(
                IRegalEducationDbContext context,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            }

            public async Task<Result> Handle(MarkAsPaidPayrollTeacherCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.PayrollTeachers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                if (entity == null)
                {
                    return Result.Failure(_localizer.Format(LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.PayrollTeacher], request.Id));
                }

                entity.IsPaid = true;
                entity.PaidDate = request.PaidDate ?? DateTime.UtcNow;

                var success = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (success)
                    return Result.Success(_localizer.Format("PayrollTeacherMarkAsPaidSuccess", _localizer[EntityName.PayrollTeacher]));
                else
                    return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer[EntityName.PayrollTeacher]));
            }
        }
    }
}