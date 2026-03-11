using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Region.Commands;
using RegalEdu.Application.RegisterStudy.Commands;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RegisterStudy.Validators
{
    public class UpdateRegisterStudyCommandValidator : AbstractValidator<UpdateRegisterStudyCommand>
    {
        public UpdateRegisterStudyCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor(x => x.RegisterStudyModel).SetValidator(new BaseRegisterStudyModelValidator(localizer));

            RuleFor(x => x.RegisterStudyModel)
                .MustAsync(async (m, ct) =>
                    !await db.RegisterStudys.AnyAsync(r => r.Code == m.Code && r.Id != m.Id && !r.IsDeleted, ct))
                .WithMessage(localizer["RegisterStudyCodeExisted"]);
        }
    }
}
