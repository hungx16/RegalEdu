using FluentValidation;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models; // Giả sử TransferCompanyModel nằm ở đây
using System;

namespace RegalEdu.Application.TransferCompany.Validators
{
    public class BaseTransferCompanyModelValidator : AbstractValidator<TransferCompanyModel>
    {
        public BaseTransferCompanyModelValidator(ILocalizationService localizer)
        {
            // --- 1. Validate cho TransferCompanyCode (Mã phiếu) ---
            // Lấy cấu hình tự động sinh mã cho Phiếu Chuyển Chi nhánh
            string start = AutoCodeConfig.Get(AutoCodeType.TransferCompany).Prefix;
            int length = AutoCodeConfig.Get(AutoCodeType.TransferCompany).Length;

            RuleFor(x => x.TransferCompanyCode)
                .NotEmpty().WithMessage(localizer["TransferCompanyCodeRequired"])
                .MaximumLength(10).WithMessage(localizer.Format("TransferCompanyCodeMaxLength", 10))                
                .Matches($"^{start}\\d{{{length}}}$")
                .WithMessage(localizer.Format("TransferCompanyCodeInvalidFormat", start, length));

            // --- 2. Validate cho SourceStudentCode (Mã học viên) ---
            RuleFor(x => x.SourceStudentCode)
                .NotEmpty().WithMessage(localizer["SourceStudentCodeRequired"]);
               // .MaximumLength(10).WithMessage(localizer.Format("SourceStudentCodeMaxLength", 10));          

            // --- 5. Validate cho DestinationCompanyId (Chi nhánh chuyển đến) ---
            // Cần là Guid hợp lệ và không rỗng (Guid.Empty)
            RuleFor(x => x.DestinationCompanyId)
                .NotEqual(Guid.Empty).WithMessage(localizer["DestinationCompanyIdRequired"]);

            // Đảm bảo Chi nhánh chuyển đến khác Chi nhánh chuyển đi
            RuleFor(x => x.DestinationCompanyId)
                .NotEqual(x => x.SourceCompanyId)
                .When(x => x.SourceCompanyId != Guid.Empty && x.DestinationCompanyId != Guid.Empty)
                .WithMessage(localizer["DestinationCompanyMustBeDifferentFromSourceCompany"]);

            // --- 6. Validate cho TransferDate (Ngày chuyển) ---
            RuleFor(x => x.TransferDate)
                .NotEmpty().WithMessage(localizer["TransferDateRequired"])
                // Có thể thêm điều kiện ngày phải là trong tương lai hoặc gần đây, tùy quy tắc nghiệp vụ
                .LessThanOrEqualTo(DateTime.Today).WithMessage(localizer["TransferDateCannotBeInTheFuture"]);

            // --- 7. Validate cho Reason (Lý do) ---
            RuleFor(x => x.Reason)
                .MaximumLength(255).WithMessage(localizer.Format("TransferCompanyReasonMaxLength", 255));

            // --- 8. Validate cho TransferCompanyStatus ---
            // Nếu bạn muốn đảm bảo trạng thái hợp lệ trong Enum
            RuleFor(x => x.TransferCompanyStatus)
                .IsInEnum().WithMessage(localizer["TransferCompanyStatusInvalid"]);
        }
    }
}