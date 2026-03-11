using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Item.Commands;

namespace RegalEdu.Application.Item.Validators
{
    public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.ItemModel)
                .SetValidator (new BaseItemModelValidator (localizer));

            RuleFor (x => x.ItemModel)
                .MustAsync (async (m, ct) =>
                    !await db.Items.AnyAsync (i =>
                        i.ItemCode == m.ItemCode &&
                        i.Id != m.Id &&
                        !i.IsDeleted, ct))
                .WithMessage (m => localizer.Format ("ItemCodeExists", m.ItemModel.ItemCode));
        }
    }
}
