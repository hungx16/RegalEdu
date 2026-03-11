using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.TransferCompany.Commands;
using RegalEdu.Application.TransferCompany.Validators;
using RegalEdu.Domain.Enums;

namespace RegalEdu.Application.TransferCompany.Validators
{
    /// <summary>
    /// Validator cho Command tạo mới Phiếu Chuyển Chi Nhánh
    /// - Validate dữ liệu đầu vào
    /// - Kiểm tra nghiệp vụ liên quan đến DB
    /// </summary>
    public class AddTransferCompanyCommandValidator
        : AbstractValidator<AddTransferCompanyCommand>
    {
        public AddTransferCompanyCommandValidator(
            ILocalizationService localizer,
            IRegalEducationDbContext dbContext)
        {
            // ============================================================
            // 1. KIỂM TRA MODEL ĐẦU VÀO
            // ============================================================
            // - Không cho model null
            // - Áp dụng toàn bộ rule validate cơ bản cho TransferCompanyModel
            RuleFor(x => x.TransferCompanyModel)
                .NotNull()
                .WithMessage(localizer["TransferCompanyModelRequired"])
                .SetValidator(new BaseTransferCompanyModelValidator(localizer));

            // ============================================================
            // 2. KIỂM TRA NGHIỆP VỤ QUAN TRỌNG NHẤT
            //    MỖI HỌC VIÊN CHỈ ĐƯỢC CÓ 1 PHIẾU CHUYỂN ĐANG XỬ LÝ
            // ============================================================
            // - Khi tạo mới phiếu chuyển, hệ thống cần đảm bảo:
            //   Học viên KHÔNG có bất kỳ phiếu nào đang ở trạng thái xử lý
            // - Các trạng thái xử lý gồm:
            //   Draft, PendingApproval, Approved, ParentConfirmed
            // - Các trạng thái kết thúc (được phép tạo mới):
            //   Completed, Rejected, ParentRejected
            RuleFor(x => x.TransferCompanyModel.SourceStudentId)
                .MustAsync(async (studentId, cancellation) =>
                {
                    // Nếu chưa xác định được học viên → bỏ qua rule này
                    if (!studentId.HasValue)
                        return true;

                    // Kiểm tra DB:
                    // Nếu tồn tại phiếu chuyển của học viên
                    // mà KHÔNG nằm trong các trạng thái kết thúc
                    // thì KHÔNG cho phép tạo mới
                    return !await dbContext.TransferCompanies.AnyAsync(
                        tc =>
                            tc.SourceStudentId == studentId &&
                            tc.TransferCompanyStatus != TransferCompanyStatus.Completed &&
                            tc.TransferCompanyStatus != TransferCompanyStatus.Rejected &&
                            tc.TransferCompanyStatus != TransferCompanyStatus.ParentRejected &&
                            !tc.IsDeleted,
                        cancellation);
                })
                .WithMessage(
                    localizer["StudentHasActiveTransferCompanyRequest"]);
        }
    }
}
