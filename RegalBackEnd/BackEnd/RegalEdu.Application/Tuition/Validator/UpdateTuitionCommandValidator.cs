using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Tuition.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Tuition.Validators
{
    public class UpdateTuitionCommandValidator : AbstractValidator<UpdateTuitionCommand>
    {
        public UpdateTuitionCommandValidator(
            ILocalizationService localizer,
            IRegalEducationDbContext dbContext)
        {
            RuleFor (x => x.TuitionModel).SetValidator (new BaseTuitionModelValidator (localizer));

            RuleFor (x => x.TuitionModel.TuitionName)
                .MustAsync (async (command, tuitionName, cancellation) =>
                {
                    return !await dbContext.Tuition.AnyAsync (
                        d => d.TuitionName == tuitionName
                          && d.Id != command.TuitionModel.Id
                          && !d.IsDeleted, cancellation);
                })
                .WithMessage ((command, tuitionName) => localizer.Format (LocalizationKey.ModelNameAlreadyExists, localizer[EntityName.Tuition], tuitionName));
        }
    }
}
