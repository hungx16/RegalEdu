using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Region.Commands;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Region.Validators
{
    public class AddRegionCommandValidator : AbstractValidator<AddRegionCommand>
    {
        public AddRegionCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor (x => x.RegionModel)
                .SetValidator (new BaseRegionModelValidator (localizer));

            RuleFor (x => x.RegionModel.RegionName)
                .MustAsync (async (name, cancellation) =>
                    !await dbContext.Regions.AnyAsync (r => r.RegionName == name && !r.IsDeleted, cancellation))
                .WithMessage ((cmd, name) => localizer.Format ("ModelNameAlreadyExists", localizer["Region"], name));
        }
    }
}
