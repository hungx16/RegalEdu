using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.WorkingTimeConfiguration.Commands;
using RegalEdu.Application.Common.Interfaces;

namespace RegalEdu.Application.WorkingTimeConfiguration.Validators
{
    public class AddWorkingTimeConfigurationCommandValidator : AbstractValidator<AddWorkingTimeConfigurationCommand>
    {
        public AddWorkingTimeConfigurationCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.WorkingTimeConfigurationModel)
                .SetValidator (new BaseWorkingTimeConfigurationModelValidator (localizer));

            RuleFor (x => x.WorkingTimeConfigurationModel.NameConfiguration)
                .MustAsync (async (name, cancel) =>
                    !await db.WorkingTimeConfigurations.AnyAsync (c => c.NameConfiguration == name && !c.IsDeleted, cancel))
                .WithMessage ((cmd, name) => localizer.Format ("WTCNameAlreadyExists", name));
        }
    }
}
