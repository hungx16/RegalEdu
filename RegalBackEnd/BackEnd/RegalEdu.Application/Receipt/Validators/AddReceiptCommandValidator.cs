using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.Gift.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Receipt.Validators
{
    public class AddReceiptCommandValidator : AbstractValidator<RegalEdu.Application.Receipt.Commands.AddReceiptCommand>
    {
        public AddReceiptCommandValidator(IRegalEducationDbContext db, ILocalizationService localizer)
        {
            RuleFor(x => x.ReceiptModel).NotNull();

            RuleFor(x => x.ReceiptModel)
                .SetValidator(new BaseReceiptModelValidator(localizer));

            RuleFor(x => x.ReceiptModel.ReceiptCode)
                .MustAsync(async (code, ct) =>
                {
                    if (string.IsNullOrWhiteSpace(code)) return false;
                    return !await db.Receipts.AnyAsync(s => !s.IsDeleted && s.ReceiptCode == code, ct);
                })
                .WithMessage(localizer["DuplicateCode"]);
        }
    }
}