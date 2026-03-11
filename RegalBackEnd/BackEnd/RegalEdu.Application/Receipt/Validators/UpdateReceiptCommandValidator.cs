using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Department.Commands;
using RegalEdu.Application.Gift.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Receipt.Validators
{
    public class UpdateReceiptCommandValidator : AbstractValidator<RegalEdu.Application.Receipt.Commands.UpdateReceiptCommand>
    {
        public UpdateReceiptCommandValidator(IRegalEducationDbContext db, ILocalizationService localizer)
        {
            RuleFor(x => x.ReceiptModel).NotNull();
            RuleFor(x => x.ReceiptModel.Id).NotEmpty();

            RuleFor(x => x.ReceiptModel)
                .SetValidator(new BaseReceiptModelValidator(localizer));

            RuleFor(x => x.ReceiptModel)
                .MustAsync(async (m, ct) =>
                {
                    // ReceiptCode duy nhất, bỏ qua bản ghi hiện tại
                    if (string.IsNullOrWhiteSpace(m.ReceiptCode)) return false;
                    return !await db.Receipts.AnyAsync(s => !s.IsDeleted && s.ReceiptCode == m.ReceiptCode && s.Id != m.Id, ct);
                })
                .WithMessage(localizer["DuplicateCode"]);
        }
    }
}
