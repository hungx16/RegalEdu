using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.LuckyDraw.Commands;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LuckyDraw.Validators
{
    public class CreateLuckyDrawCommandValidator : AbstractValidator<CreateLuckyDrawCommand>
    {
        public CreateLuckyDrawCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.LuckyDrawModel).NotNull();
            RuleFor(x => x.LuckyDrawModel.Name).NotEmpty().WithMessage(localizer["NameRequired"]).MaximumLength(200);
            RuleFor(x => x.LuckyDrawModel.StartDate).NotEmpty();
            RuleFor(x => x.LuckyDrawModel.EndDate).NotEmpty();
            RuleFor(x => x.LuckyDrawModel).Must(m => m.StartDate <= m.EndDate).WithMessage(localizer["StartBeforeEnd"]);
        }
    }
}
