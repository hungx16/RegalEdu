using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Item.Commands;

namespace RegalEdu.Application.Item.Validators
{
    public class AddItemCommandValidator : AbstractValidator<AddItemCommand>
    {
        public AddItemCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.ItemModel)
                .SetValidator (new BaseItemModelValidator (localizer));

            RuleFor (x => x.ItemModel.ItemCode)
                .MustAsync (async (code, ct) =>
                    !await db.Items.AnyAsync (i => i.ItemCode == code && !i.IsDeleted, ct))
                .WithMessage ((_, code) => localizer.Format ("ItemCodeExists", code));
        }
    }
}
