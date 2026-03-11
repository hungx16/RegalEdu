using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LectureType.Validators
{
    public class BaseLectureTypeModelValidator : AbstractValidator<LectureTypeModel>
    {
        public BaseLectureTypeModelValidator(ILocalizationService localizer)
        {
            RuleFor (x => x.LectureName)
                .NotEmpty ( ).WithMessage (localizer["LectureNameRequired"])
                .MaximumLength (200).WithMessage (localizer.Format ("LectureNameMaxLength", 200));

            RuleFor (x => x.FileUrl)
                .MaximumLength (255).WithMessage (localizer.Format ("LectureTypeFileUrlMaxLength", 255));
        }
    }
}
