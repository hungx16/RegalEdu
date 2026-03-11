using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.PartnerType.Commands;

namespace RegalEdu.Application.PartnerType.Validators
{
    public class UpdatePartnerTypeCommandValidator : AbstractValidator<UpdatePartnerTypeCommand>
    {
        public UpdatePartnerTypeCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.Model)
                .SetValidator (new BasePartnerTypeModelValidator (localizer));

            RuleFor (x => x.Model)
                .MustAsync (async (m, ct) =>
                    !await db.PartnerTypes.AnyAsync (pt => pt.PartnerTypeCode == m.PartnerTypeCode && pt.Id != m.Id && !pt.IsDeleted, ct))
                .WithMessage (m => localizer.Format ("PartnerTypeCodeAlreadyExists", m.Model.PartnerTypeCode));

            RuleFor (x => x.Model)
                .MustAsync (async (m, ct) =>
                    !await db.PartnerTypes.AnyAsync (pt => pt.PartnerTypeName == m.PartnerTypeName && pt.Id != m.Id && !pt.IsDeleted, ct))
                .WithMessage (m => localizer.Format ("PartnerTypeNameAlreadyExists", m.Model.PartnerTypeName));
        }
    }
}