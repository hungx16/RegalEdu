using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.WorkingTimeConfiguration.Commands;
using RegalEdu.Application.Common.Interfaces;

namespace RegalEdu.Application.WorkingTimeConfiguration.Validators
{
    public class UpdateWorkingTimeConfigurationCommandValidator : AbstractValidator<UpdateWorkingTimeConfigurationCommand>
    {
        public UpdateWorkingTimeConfigurationCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.WorkingTimeConfigurationModel)
                .SetValidator (new BaseWorkingTimeConfigurationModelValidator (localizer));

            RuleFor (x => x.WorkingTimeConfigurationModel.NameConfiguration)
                .MustAsync (async (cmd, name, cancel) =>
                    !await db.WorkingTimeConfigurations.AnyAsync (c =>
                        c.NameConfiguration == name &&
                        c.Id != cmd.WorkingTimeConfigurationModel.Id &&
                        !c.IsDeleted, cancel))
                .WithMessage ((cmd, name) => localizer.Format ("WTCNameAlreadyExists", name));
        }
    }
}
