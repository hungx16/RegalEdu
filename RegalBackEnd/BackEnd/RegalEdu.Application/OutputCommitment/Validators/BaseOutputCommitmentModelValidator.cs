using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.OutputCommitment.Validators
{
    public class BaseOutputCommitmentModelValidator : AbstractValidator<OutputCommitmentModel>
    {
        public BaseOutputCommitmentModelValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.StudentCode)
                .NotEmpty ( ).WithMessage (localizer["StudentCodeRequired"])
                .MaximumLength (50).WithMessage (localizer.Format ("StudentCodeMaxLength", 50));

            // Optional text fields
            RuleFor (x => x.BeginningLevel)
                .MaximumLength (50).WithMessage (localizer.Format ("BeginningLevelMaxLength", 50));
            RuleFor (x => x.FinalLevel)
                .MaximumLength (50).WithMessage (localizer.Format ("FinalLevelMaxLength", 50));
            RuleFor (x => x.OutputCommitmentInfo)
                .MaximumLength (2000).WithMessage (localizer.Format ("OutputCommitmentInfoMaxLength", 2000));

            // Enum status
            RuleFor (x => x.OutputCommitmentStatus)
                .IsInEnum ( ).WithMessage (localizer["OutputCommitmentStatusInvalid"]);

            // If StudentId provided -> must exist
            When (x => x.StudentId.HasValue, ( ) =>
            {
                RuleFor (x => x.StudentId)
                    .MustAsync (async (id, ct) =>
                        await db.Students.AnyAsync (s => s.Id == id, ct))
                    .WithMessage (localizer["StudentNotFound"]);
            });
        }
    }
}
