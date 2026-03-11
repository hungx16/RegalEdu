using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.TransferCompany.Commands;
using RegalEdu.Application.TransferCompany.Validators;
using RegalEdu.Domain.Enums;

namespace RegalEdu.Application.TransferCompany.Validators
{
    /// <summary>
    /// Validator cho Command CẬP NHẬT Phiếu Chuyển Chi Nhánh
    /// - Validate dữ liệu đầu vào
    /// - Kiểm tra nghiệp vụ liên quan đến DB
    /// </summary>
    public class UpdateTransferCompanyCommandValidator
        : AbstractValidator<UpdateTransferCompanyCommand>
    {
        public UpdateTransferCompanyCommandValidator(
            ILocalizationService localizer,
            IRegalEducationDbContext dbContext)
        {
            // ============================================================
            // 1. KIỂM TRA MODEL ĐẦU VÀO
            // ============================================================
            // - Model không được null
            // - Áp dụng toàn bộ validate cơ bản cho TransferCompanyModel
            RuleFor(x => x.TransferCompanyModel)
                .NotNull()
                .WithMessage(localizer["TransferCompanyModelRequired"])
                .SetValidator(new BaseTransferCompanyModelValidator(localizer));

            // ============================================================
            // 2. KHÔNG CHO PHÉP THAY ĐỔI HỌC VIÊN NGUỒN
            // ============================================================
            // - SourceStudentId là dữ liệu gắn với lịch sử & workflow
            // - Khi UPDATE, KHÔNG được phép đổi học viên
            RuleFor(x => x.TransferCompanyModel.SourceStudentId)
                .MustAsync(async (command, studentId, cancellation) =>
                {
                    // Lấy SourceStudentId hiện tại trong DB
                    var currentStudentId = await dbContext.TransferCompanies
                        .Where(tc => tc.Id == command.TransferCompanyModel.Id)
                        .Select(tc => tc.SourceStudentId)
                        .FirstOrDefaultAsync(cancellation);

                    // Không cho phép thay đổi
                    return currentStudentId == studentId;
                })
                .WithMessage(localizer["SourceStudentCannotBeChanged"]);

            // ============================================================
            // 3. KIỂM TRA PHIẾU CHUYỂN CÓ TỒN TẠI & CHƯA BỊ XÓA
            // ============================================================
            RuleFor(x => x.TransferCompanyModel.Id)
                .MustAsync(async (id, cancellation) =>
                {
                    return await dbContext.TransferCompanies.AnyAsync(
                        tc => tc.Id == id && !tc.IsDeleted,
                        cancellation);
                })
                .WithMessage(localizer["TransferCompanyNotFound"]);

            // ============================================================
            // 4. NGHIỆP VỤ:
            //    MỖI HỌC VIÊN CHỈ ĐƯỢC CÓ 1 PHIẾU ĐANG XỬ LÝ
            //    (TRỪ CHÍNH PHIẾU ĐANG SỬA)
            // ============================================================
            RuleFor(x => x.TransferCompanyModel.SourceStudentId)
                .MustAsync(async (command, studentId, cancellation) =>
                {
                    if (!studentId.HasValue)
                        return true;

                    return !await dbContext.TransferCompanies.AnyAsync(
                        tc =>
                            tc.SourceStudentId == studentId &&
                            tc.Id != command.TransferCompanyModel.Id &&
                            tc.TransferCompanyStatus != TransferCompanyStatus.Completed &&
                            tc.TransferCompanyStatus != TransferCompanyStatus.Rejected &&
                            tc.TransferCompanyStatus != TransferCompanyStatus.ParentRejected &&
                            !tc.IsDeleted,
                        cancellation);
                })
                .WithMessage(
                    localizer["StudentHasActiveTransferCompanyRequest"]);

            // ============================================================
            // 5. KHÔNG CHO SỬA PHIẾU ĐÃ KẾT THÚC
            // ============================================================
            RuleFor(x => x.TransferCompanyModel.TransferCompanyStatus)
                .MustAsync(async (command, status, cancellation) =>
                {
                    var currentStatus = await dbContext.TransferCompanies
                        .Where(tc => tc.Id == command.TransferCompanyModel.Id)
                        .Select(tc => tc.TransferCompanyStatus)
                        .FirstOrDefaultAsync(cancellation);

                    // Nếu đã Completed / Rejected → cấm sửa
                    return currentStatus != TransferCompanyStatus.Completed &&
                           currentStatus != TransferCompanyStatus.Rejected &&
                           currentStatus != TransferCompanyStatus.ParentRejected;
                })
                .WithMessage(
                    localizer["TransferCompanyCannotBeUpdatedInFinalState"]);
        }
    }
}
