using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Enumerations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RegalEdu.Application.TransferCompany.Commands
{
    // =====================================================================
    // COMMAND: Chuyển trạng thái Phiếu Chuyển Chi Nhánh
    // - Chỉ dùng cho workflow
    // - KHÔNG dùng để update dữ liệu khác
    // =====================================================================
    public class ChangeTransferCompanyStatusCommand : IRequest<Result>
    {
        /// <summary>
        /// Id Phiếu chuyển chi nhánh
        /// </summary>
        public Guid TransferCompanyId { get; set; }

        /// <summary>
        /// Trạng thái mới cần chuyển sang
        /// </summary>
        public TransferCompanyStatus NewStatus { get; set; }
    }

    // =====================================================================
    // HANDLER
    // =====================================================================
    public class ChangeTransferCompanyStatusCommandHandler
        : IRequestHandler<ChangeTransferCompanyStatusCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILocalizationService _localizer;

        public ChangeTransferCompanyStatusCommandHandler(
            IRegalEducationDbContext context,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result> Handle(
            ChangeTransferCompanyStatusCommand request,
            CancellationToken cancellationToken)
        {
            // ======================================================
            // 1. Lấy phiếu chuyển chi nhánh
            // ======================================================
            var transferCompany = await _context.TransferCompanies
                .FirstOrDefaultAsync(
                    x => x.Id == request.TransferCompanyId && !x.IsDeleted,
                    cancellationToken);

            if (transferCompany == null)
            {
                return Result.Failure(
                    _localizer.Format(
                        LocalizationKey.EntityNotFound,
                        EntityName.TransferCompany));
            }

            // ======================================================
            // 2. Không cho chuyển trạng thái nếu phiếu đã kết thúc
            // ======================================================
            if (transferCompany.TransferCompanyStatus == TransferCompanyStatus.Completed ||
                transferCompany.TransferCompanyStatus == TransferCompanyStatus.Rejected ||
                transferCompany.TransferCompanyStatus == TransferCompanyStatus.ParentRejected)
            {
                return Result.Failure(
                    _localizer["TransferCompanyCannotBeUpdatedInFinalState"]);
            }

            // ======================================================
            // 3. Kiểm tra workflow chuyển trạng thái hợp lệ
            // ======================================================
            if (!IsValidTransition(
                    transferCompany.TransferCompanyStatus,
                    request.NewStatus))
            {
                return Result.Failure(
                    _localizer["InvalidTransferCompanyStatusTransition"]);
            }

            // ======================================================
            // 4. Cập nhật trạng thái
            // ======================================================
            transferCompany.TransferCompanyStatus = request.NewStatus;

            // ======================================================
            // 5. Lưu DB
            // ======================================================
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success)
            {
                return Result.Success(
                    _localizer.Format(
                        LocalizationKey.MSG_UPDATE_SUCCESS,
                        EntityName.TransferCompany));
            }

            return Result.Failure(
                _localizer.Format(
                    LocalizationKey.ERR_SAVE_NO_EFFECT,
                    EntityName.TransferCompany));
        }

        // ======================================================
        // WORKFLOW CHUYỂN TRẠNG THÁI
        // ======================================================
        private static bool IsValidTransition(
            TransferCompanyStatus currentStatus,
            TransferCompanyStatus newStatus)
        {
            return currentStatus switch
            {
                TransferCompanyStatus.Draft =>
                    newStatus == TransferCompanyStatus.PendingApproval,

                TransferCompanyStatus.PendingApproval =>
                    newStatus == TransferCompanyStatus.Approved ||
                    newStatus == TransferCompanyStatus.Rejected,

                TransferCompanyStatus.Approved =>
                    newStatus == TransferCompanyStatus.ParentConfirmed,

                TransferCompanyStatus.ParentConfirmed =>
                    newStatus == TransferCompanyStatus.Completed,

                _ => false // Completed / Rejected / ParentRejected
            };
        }
    }
}
