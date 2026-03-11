using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Region.Commands;

namespace RegalEdu.Application.Region.Validators
{
    public class UpdateRegionCommandValidator : AbstractValidator<UpdateRegionCommand>
    {
        public UpdateRegionCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor (x => x.RegionModel)
                .SetValidator (new BaseRegionModelValidator (localizer));

            RuleFor (x => x.RegionModel.RegionCode)
                .MustAsync (async (command, code, cancellation) =>
                    !await dbContext.Regions.AnyAsync (r => r.RegionCode == code && r.Id != command.RegionModel.Id && !r.IsDeleted, cancellation))
                .WithMessage ((command, code) => localizer.Format ("ModelCodeAlreadyExists", localizer["Region"], code));

            RuleFor (x => x.RegionModel.RegionName)
                .MustAsync (async (command, name, cancellation) =>
                    !await dbContext.Regions.AnyAsync (r => r.RegionName == name && r.Id != command.RegionModel.Id && !r.IsDeleted, cancellation))
                .WithMessage ((command, name) => localizer.Format ("ModelNameAlreadyExists", localizer["Region"], name));
        }
    }
}
