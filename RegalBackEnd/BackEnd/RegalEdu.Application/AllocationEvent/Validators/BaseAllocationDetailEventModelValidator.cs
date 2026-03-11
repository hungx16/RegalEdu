using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AllocationEvent.Validators
{
    // 🔹 Validator kiểm tra tính hợp lệ cho dữ liệu AllocationDetailEventModel
    public class BaseAllocationDetailEventModelValidator : AbstractValidator<AllocationDetailEventModel>
    {
        public BaseAllocationDetailEventModelValidator(ILocalizationService localizer)
        {
            // 🔸 Kiểm tra EventId: bắt buộc có
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage(localizer["EventIdRequired"]);

            // 🔸 Kiểm tra CompanyId: bắt buộc có
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage(localizer["CompanyIdRequired"]);

            // 🔸 Kiểm tra RegionId: bắt buộc có
            RuleFor(x => x.RegionId)
                .NotEmpty()
                .WithMessage(localizer["RegionIdRequired"]);

            // 🔸 Kiểm tra Quantity: phải >= 0
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage(localizer["QuantityMustBePositive"]);

            // 🔸 Kiểm tra Enum NoAllocation: phải hợp lệ
            RuleFor(x => x.NoAllocation)
                .IsInEnum()
                .WithMessage(localizer["InvalidNoAllocationStatus"]);
        }
    }
}
