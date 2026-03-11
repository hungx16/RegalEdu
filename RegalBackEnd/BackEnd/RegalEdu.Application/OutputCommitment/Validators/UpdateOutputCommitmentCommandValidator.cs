using FluentValidation;
using RegalEdu.Application.Common.Interfaces;

namespace RegalEdu.Application.OutputCommitment.Validators
{
    public class UpdateOutputCommitmentCommandValidator : AbstractValidator<Commands.UpdateOutputCommitmentCommand>
    {
        public UpdateOutputCommitmentCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.OutputCommitmentModel)
                .SetValidator (new BaseOutputCommitmentModelValidator (localizer, db));
        }
    }
}
