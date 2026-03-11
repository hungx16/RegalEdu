using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.PartnerType.Commands;

namespace RegalEdu.Application.PartnerType.Validators
{
    public class AddPartnerTypeCommandValidator : AbstractValidator<AddPartnerTypeCommand>
    {
        public AddPartnerTypeCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor(x => x.Model)
                .SetValidator(new BasePartnerTypeModelValidator(localizer));

            // Unique code
            RuleFor(x => x.Model.PartnerTypeCode)
                .MustAsync(async (code, ct) => !await db.PartnerTypes.AnyAsync(pt => pt.PartnerTypeCode == code && !pt.IsDeleted, ct))
                .WithMessage((_, code) => localizer.Format("PartnerTypeCodeAlreadyExists", code));

            // Optional: unique name
            RuleFor(x => x.Model.PartnerTypeName)
                .MustAsync(async (name, ct) => !await db.PartnerTypes.AnyAsync(pt => pt.PartnerTypeName == name && !pt.IsDeleted, ct))
                .WithMessage((_, name) => localizer.Format("PartnerTypeNameAlreadyExists", name));
        }
    }
}