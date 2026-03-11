using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.SupportingDocument.Validators
{
    public class BaseSupportingDocumentModelValidator : AbstractValidator<SupportingDocumentModel>
    {
        public BaseSupportingDocumentModelValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.DocumentName)
                .NotEmpty ( ).WithMessage (localizer["SD_DocumentNameRequired"])
                .MaximumLength (500).WithMessage (localizer.Format ("SD_DocumentNameMaxLength", 500));

            RuleFor (x => x.Description)
                .MaximumLength (2000).WithMessage (localizer.Format ("SD_DescriptionMaxLength", 2000));

            RuleFor (x => x.DocumentTypeId)
                .GreaterThan (0).WithMessage (localizer["SD_DocumentTypeIdRequired"]);

            RuleFor (x => x.WebsiteKeys)
                .MaximumLength (500).WithMessage (localizer.Format ("SD_WebsiteKeysMaxLength", 500));

            RuleFor (x => x.AuthorName)
                .MaximumLength (255).WithMessage (localizer.Format ("SD_AuthorNameMaxLength", 255));

            // PublishDate <= EndDate (n?u c? hai có giá tr?)
            RuleFor (x => x)
                .Must (x =>
                {
                    if (x.StartDate.HasValue && x.EndDate.HasValue)
                        return x.StartDate.Value.Date <= x.EndDate.Value.Date;
                    return true;
                })
                .WithMessage (localizer["SD_DateRangeInvalid"]);
        }
    }
}
