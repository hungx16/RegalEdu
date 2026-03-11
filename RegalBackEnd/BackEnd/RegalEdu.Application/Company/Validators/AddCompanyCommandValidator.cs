using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Company.Commands;

namespace RegalEdu.Application.Company.Validators
{
    public class AddCompanyCommandValidator : AbstractValidator<AddCompanyCommand>
    {
        public AddCompanyCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor (x => x.CompanyModel)
                .SetValidator (new BaseCompanyModelValidator (localizer));

            RuleFor (x => x.CompanyModel.CompanyName)
                .MustAsync (async (name, cancellation) =>
                    !await dbContext.Companies.AnyAsync (c => c.CompanyName == name && !c.IsDeleted, cancellation))
                .WithMessage ((cmd, name) => localizer.Format ("ModelNameAlreadyExists", localizer["Company"], name));
        }
    }
}
