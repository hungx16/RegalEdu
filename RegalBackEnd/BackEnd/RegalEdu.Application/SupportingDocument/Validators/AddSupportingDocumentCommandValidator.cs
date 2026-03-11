using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.SupportingDocument.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.SupportingDocument.Validators
{
    public class AddSupportingDocumentCommandValidator : AbstractValidator<AddSupportingDocumentCommand>
    {
        public AddSupportingDocumentCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.SupportingDocumentModel)
                .SetValidator (new BaseSupportingDocumentModelValidator (localizer));

            RuleFor (x => x.SupportingDocumentModel.DocumentName)
              .MustAsync (async (name, cancellation) =>
                  !await db.SupportingDocuments.AnyAsync (d => d.DocumentName == name && !d.IsDeleted, cancellation))
              .WithMessage ((command, name) => localizer.Format (LocalizationKey.ModelNameAlreadyExists, EntityName.SupportingDocument, name));
        }
    }
}
