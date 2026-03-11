using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Region.Commands;
using RegalEdu.Application.RegisterStudy.Commands;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RegisterStudy.Validators
{
    public class AddRegisterStudyCommandValidator : AbstractValidator<AddRegisterStudyCommand>
    {
        public AddRegisterStudyCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor(x => x.RegisterStudyModel).SetValidator(new BaseRegisterStudyModelValidator(localizer));

            // Code duy nhất
            RuleFor(x => x.RegisterStudyModel.Code!)
                .MustAsync(async (code, ct) => !await db.RegisterStudys.AnyAsync(r => r.Code == code && !r.IsDeleted, ct))
                .WithMessage(localizer["RegisterStudyCodeExisted"]);
        }
    }
}
