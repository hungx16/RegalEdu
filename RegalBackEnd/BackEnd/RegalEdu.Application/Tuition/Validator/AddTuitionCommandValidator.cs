using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Tuition.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Tuition.Validators
{
    public class AddTuitionCommandValidator : AbstractValidator<AddTuitionCommand>
    {
        public AddTuitionCommandValidator(
            ILocalizationService localizer,
            IRegalEducationDbContext dbContext)
        {
            RuleFor (x => x.TuitionModel)
                .SetValidator (new BaseTuitionModelValidator (localizer));

            RuleFor (x => x.TuitionModel.TuitionName)
                .MustAsync (async (name, cancellation) =>
                {
                    return !await dbContext.Tuition.AnyAsync (d => d.TuitionName == name && !d.IsDeleted, cancellation);
                })
                .WithMessage ((command, name) => localizer.Format (LocalizationKey.ModelNameAlreadyExists, localizer[EntityName.Tuition], name));
        }
    }
}
