using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Course.Validators
{
    public class BaseCourseModelValidator : AbstractValidator<CourseModel>
    {
        public BaseCourseModelValidator(ILocalizationService localizer)
        {
            // 1?. Ki?m tra Code
            RuleFor(x => x.CourseCode)
                .NotEmpty()
                .WithMessage(localizer["CourseCodeRequired"])
                .MaximumLength(50)
                .WithMessage(localizer.Format("CourseCodeMaxLength", 50))
                .Must(code => !code.Contains(" "))
                .WithMessage(localizer["CourseCodeNoSpaces"]);

            // 2?. Ki?m tra CourseName
            RuleFor(x => x.CourseName)
                .NotEmpty()
                .WithMessage(localizer["CourseNameRequired"])
                .MaximumLength(255)
                .WithMessage(localizer.Format("CourseNameMaxLength", 255));

            // 3?. Description (n?u c¢ th max length 1000)
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage(localizer.Format("CourseDescriptionMaxLength", 1000));

            // 3b. CommitmentOutputType valid
            RuleFor(x => x.CommitmentOutputType)
                .IsInEnum();

            // 4. CommitmentLevel rules
            RuleFor(x => x.CommitmentLevel)
                .MaximumLength(100)
                .WithMessage(localizer.Format("CourseCommitmentLevelMaxLength", 100));

            // 4a. Self-commit requires commitment level
            RuleFor(x => x.CommitmentLevel)
                .NotEmpty()
                .WithMessage(localizer["CommitmentLevelRequired"])
                .When(x => x.CommitmentOutputType == CommitmentOutputType.SelfCommitment);

            // 4b. None cannot have commitment level
            RuleFor(x => x.CommitmentLevel)
                .Must((model, level) => model.CommitmentOutputType != CommitmentOutputType.None || string.IsNullOrWhiteSpace(level))
                .WithMessage(localizer["CommitmentLevelCannotBeSetWhenCommitmentOutputFalse"]);

            // 5?. Reference (n?u c¢ th max length 255)
            RuleFor(x => x.Reference)
                .MaximumLength(255)
                .WithMessage(localizer.Format("CourseReferenceMaxLength", 255));

            // 6?. MidExamIds (n?u c¢ th max length 500 v… ch? ch?a GUID c ch nhau b?ng d?u ph?y)
            RuleFor(x => x.MidExamIds)
                .MaximumLength(500)
                .WithMessage(localizer.Format("CourseMidExamIdsMaxLength", 500))
                .Must(ids => string.IsNullOrWhiteSpace(ids) || ids.Split(',')
                    .All(s => Guid.TryParse(s.Trim(), out _)))
                .WithMessage(localizer["MidExamIdsMustBeGuids"]);

            // 7?. FinalExamIds (n?u c¢ th max length 500 v… ch? ch?a GUID c ch nhau b?ng d?u ph?y)
            RuleFor(x => x.FinalExamIds)
                .MaximumLength(500)
                .WithMessage(localizer.Format("CourseFinalExamIdsMaxLength", 500))
                .Must(ids => string.IsNullOrWhiteSpace(ids) || ids.Split(',')
                    .All(s => Guid.TryParse(s.Trim(), out _)))
                .WithMessage(localizer["FinalExamIdsMustBeGuids"]);

            // 8?. Sequence (th? t? kh¢a h?c trong chuong trnh) h?p l? t? 1 -> 100
            RuleFor(x => x.Sequence)
                .InclusiveBetween(1, 100)
                .WithMessage(localizer.Format("CourseSequenceRange", 1, 100));

            // 9?. MinAvgScore > 0
            RuleFor(x => x.MinAvgScore)
                .GreaterThan(0)
                .WithMessage(localizer["MinAvgScoreMustBeGreaterThanZero"]);

            // 10. LearningRoadmapId (n?u b?t bu?c ph?i c¢)
            RuleFor(x => x.LearningRoadMapId)
                .NotEmpty()
                .WithMessage(localizer["LearningRoadmapRequired"]);

        }
    }
}
