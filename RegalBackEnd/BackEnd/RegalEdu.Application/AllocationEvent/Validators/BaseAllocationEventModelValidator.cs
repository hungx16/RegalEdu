using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.AllocationEvent.Validators
{
    // 🔹 Validator kiểm tra tính hợp lệ cho dữ liệu AllocationEventModel
    public class BaseAllocationEventModelValidator : AbstractValidator<AllocationEventModel>
    {
        public BaseAllocationEventModelValidator(ILocalizationService localizer)
        {
            // 🔸 Mã phân bổ được sinh tự động
            // 🔸 Vẫn giữ kiểm tra độ dài & ký tự để đảm bảo dữ liệu hợp lệ khi lưu
            RuleFor(x => x.AllocationCode)
                .MaximumLength(50).WithMessage(localizer.Format("AllocationCodeMaxLength", 50))
                .Must(code => !code.Contains(" "))
                .WithMessage(localizer["AllocationCodeNoSpaces"]);

            // 🔸 Kiểm tra tháng phân bổ: giá trị hợp lệ từ 1 đến 12
            RuleFor(x => x.AllocationMonth)
                .InclusiveBetween(1, 12)
                .WithMessage(localizer.Format("AllocationMonthRange", 1, 12));

            //// 🔸 Kiểm tra năm phân bổ: nằm trong khoảng 2000–2100
            //RuleFor(x => x.AllocationYear)
            //    .InclusiveBetween(2000, 2100)
            //    .WithMessage(localizer.Format("AllocationYearRange", 2000, 2100));

            // 🔸 Kiểm tra ngân sách cho 1 chi nhánh: phải là số dương
            RuleFor(x => x.EventBudget)
                .GreaterThan(0)
                .WithMessage(localizer["EventBudgetMustBePositive"]);

            // 🔸 Kiểm tra trạng thái: phải là giá trị hợp lệ trong enum AllocationEventStatus
            RuleFor(x => x.AllocationEventStatus)
                .IsInEnum()
                .WithMessage(localizer["InvalidAllocationEventStatus"]);
        }
    }
}
