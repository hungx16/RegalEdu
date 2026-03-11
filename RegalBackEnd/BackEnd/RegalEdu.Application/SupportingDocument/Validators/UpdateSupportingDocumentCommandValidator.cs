using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.SupportingDocument.Commands;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.SupportingDocument.Validators
{
    public class UpdateSupportingDocumentCommandValidator : AbstractValidator<UpdateSupportingDocumentCommand>
    {
        public UpdateSupportingDocumentCommandValidator(ILocalizationService localizer, IRegalEducationDbContext db)
        {
            RuleFor (x => x.SupportingDocumentModel)
                .SetValidator (new BaseSupportingDocumentModelValidator (localizer));

            RuleFor (x => x.SupportingDocumentModel.DocumentName)
                .MustAsync (async (command, name, cancellation) =>
                    !await db.SupportingDocuments.AnyAsync (d => d.DocumentName == name && d.Id != command.SupportingDocumentModel.Id && !d.IsDeleted, cancellation))
                .WithMessage ((command, name) => localizer.Format ("ModelNameAlreadyExists", localizer[EntityName.SupportingDocument], name));
        }
    }
}
