using FluentValidation;
using RegalEdu.Application.Common.Interfaces;

namespace RegalEdu.Application.OutputCommitment.Validators
{
    public class AddOutputCommitmentCommandValidator : AbstractValidator<Commands.AddOutputCommitmentCommand>
    {
        public AddOutputCommitmentCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.OutputCommitmentModel)
                .SetValidator (new BaseOutputCommitmentModelValidator (localizer, db));
        }
    }
}
