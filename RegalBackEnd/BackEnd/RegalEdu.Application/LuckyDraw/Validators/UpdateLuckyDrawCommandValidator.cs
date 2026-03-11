using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.LuckyDraw.Commands;

namespace RegalEdu.Application.LuckyDraw.Validators
{
    public class UpdateLuckyDrawCommandValidator : AbstractValidator<UpdateLuckyDrawCommand>
    {
        public UpdateLuckyDrawCommandValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.LuckyDrawModel).NotNull();
            RuleFor(x => x.LuckyDrawModel.Id).NotNull().WithMessage(localizer["IdRequired"]);
            RuleFor(x => x.LuckyDrawModel.Name).NotEmpty().WithMessage(localizer["NameRequired"]).MaximumLength(200);
            RuleFor(x => x.LuckyDrawModel).Must(m => m.StartDate <= m.EndDate).WithMessage(localizer["StartBeforeEnd"]);
        }
    }
}
