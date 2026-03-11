using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Company.Commands;

namespace RegalEdu.Application.Company.Validators
{
    public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
    {
        public UpdateCompanyCommandValidator(ILocalizationService localizer, IRegalEducationDbContext dbContext)
        {
            RuleFor (x => x.CompanyModel)
                .SetValidator (new BaseCompanyModelValidator (localizer));

            RuleFor (x => x.CompanyModel.CompanyCode)
                .MustAsync (async (command, code, cancellation) =>
                    !await dbContext.Companies.AnyAsync (c => c.CompanyCode == code && c.Id != command.CompanyModel.Id && !c.IsDeleted, cancellation))
                .WithMessage ((command, code) => localizer.Format ("ModelCodeAlreadyExists", localizer["Company"], code));

            RuleFor (x => x.CompanyModel.CompanyName)
                .MustAsync (async (command, name, cancellation) =>
                    !await dbContext.Companies.AnyAsync (c => c.CompanyName == name && c.Id != command.CompanyModel.Id && !c.IsDeleted, cancellation))
                .WithMessage ((command, name) => localizer.Format ("ModelNameAlreadyExists", localizer["Company"], name));
        }
    }
}
