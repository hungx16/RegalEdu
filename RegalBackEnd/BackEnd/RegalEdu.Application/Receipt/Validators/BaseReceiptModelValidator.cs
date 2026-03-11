using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
namespace RegalEdu.Application.Receipt.Validators
{
    public class BaseReceiptModelValidator : AbstractValidator<ReceiptsModel>
    {
        public BaseReceiptModelValidator(ILocalizationService localizer)
        {
            RuleFor(x => x.ReceiptCode)
                .NotEmpty().WithMessage("ReceiptCode is required.")
                .MaximumLength(50);

            //RuleFor(x => x.ReceiptType)
            //    .NotEmpty().WithMessage("ReceiptType is required.")
            //    .MaximumLength(100);

            //RuleFor(x => x.PaymentMethodType)
            //    .MaximumLength(100)
            //    .When(x => !string.IsNullOrWhiteSpace(x.PaymentMethodType));

            //RuleFor(x => x.PaymentTermType)
            //    .MaximumLength(100)
            //    .When(x => !string.IsNullOrWhiteSpace(x.PaymentTermType));

            //RuleFor(x => x.PaymentMethod)
            //    .MaximumLength(200)
            //    .When(x => !string.IsNullOrWhiteSpace(x.PaymentMethod));

            RuleFor(x => x.TotalAmount)
                .GreaterThanOrEqualTo(0).When(x => x.TotalAmount.HasValue);

            RuleFor(x => x.Note)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Note));
        }
    }
}
