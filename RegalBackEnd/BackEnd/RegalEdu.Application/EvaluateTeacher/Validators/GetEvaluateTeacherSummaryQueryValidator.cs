using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.EvaluateTeacher.Queries;

namespace RegalEdu.Application.EvaluateTeacher.Validators
{
    public class GetEvaluateTeacherSummaryQueryValidator : AbstractValidator<GetEvaluateTeacherSummaryQuery>
    {
        public GetEvaluateTeacherSummaryQueryValidator(ILocalizationService localizer)
        {
            // Kiểm tra nếu có cả FromDate và ToDate thì ToDate phải >= FromDate
            When(x => x.FromDate.HasValue && x.ToDate.HasValue, () =>
            {
                RuleFor(x => x.ToDate)
                    .GreaterThanOrEqualTo(x => x.FromDate)
                    .WithMessage(localizer["ToDateMustBeAfterFromDate"]);
            });

            // Kiểm tra ToDate không được ở tương lai
            RuleFor(x => x.ToDate)
                .LessThanOrEqualTo(DateTime.UtcNow.Date)
                .When(x => x.ToDate.HasValue)
                .WithMessage(localizer["ToDateCannotBeFuture"]);
        }
    }
}