using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RegalEdu.Application.TransferCompany.Commands
{
    // ======================================================
    // COMMAND: Xóa danh sách Phiếu Chuyển
    // - Chỉ cho phép xóa khi ở trạng thái NHÁP (Draft)
    // - Thực hiện SOFT DELETE giống Division
    // ======================================================
    public class DeleteListTransferCompanyCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    // ======================================================
    // HANDLER
    // ======================================================
    public class DeleteListTransferCompanyCommandHandler
        : IRequestHandler<DeleteListTransferCompanyCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<DeleteListTransferCompanyCommandHandler> _logger;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDeleteService;

        public DeleteListTransferCompanyCommandHandler(
            IRegalEducationDbContext context,
            ILogger<DeleteListTransferCompanyCommandHandler> logger,
            ILocalizationService localizer,
            ISoftDeleteService softDeleteService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _softDeleteService = softDeleteService ?? throw new ArgumentNullException(nameof(softDeleteService));
        }

        public async Task<Result> Handle(
            DeleteListTransferCompanyCommand request,
            CancellationToken cancellationToken)
        {
            // ==================================================
            // 1. Kiểm tra dữ liệu đầu vào
            // ==================================================
            if (request.ListIds == null || !request.ListIds.Any())
            {
                return Result.Failure(
                    _localizer.Format(
                        LocalizationKey.NoModelToDelete,
                        EntityName.TransferCompany));
            }

            int successCount = 0;
            int failCount = 0;
            var failMessages = new List<string>();

            // ==================================================
            // 2. Duyệt từng ID Phiếu Chuyển
            // ==================================================
            foreach (var id in request.ListIds)
            {
                // ----------------------------------------------
                // 2.1. Parse string → Guid (TRÁNH LỖI Guid == string)
                // ----------------------------------------------
                if (!Guid.TryParse(id, out var transferCompanyId))
                {
                    failCount++;

                    var invalidIdMsg = _localizer.Format(
                        LocalizationKey.InvalidGuidFormat,
                        _localizer[EntityName.TransferCompany],
                        id);

                    failMessages.Add(invalidIdMsg);
                    _logger.LogWarning(invalidIdMsg);
                    continue;
                }

                // ----------------------------------------------
                // 2.2. Tìm Phiếu Chuyển
                // ----------------------------------------------
                var transferCompany = _context.TransferCompanies
                    .FirstOrDefault(tc =>
                        tc.Id == transferCompanyId &&
                        !tc.IsDeleted);

                if (transferCompany == null)
                {
                    failCount++;

                    var notFoundMsg = _localizer.Format(
                        LocalizationKey.EntityWithIdNotFound,
                        _localizer[EntityName.TransferCompany],
                        id);

                    failMessages.Add(notFoundMsg);
                    _logger.LogWarning(notFoundMsg);
                    continue;
                }

                // ==================================================
                // 3. CHỈ CHO XÓA KHI Ở TRẠNG THÁI NHÁP
                // ==================================================
                if (transferCompany.TransferCompanyStatus != TransferCompanyStatus.Draft)
                {
                    failCount++;

                    var invalidStatusMsg =
                        _localizer["TransferCompanyOnlyDraftCanBeDeleted"];

                    failMessages.Add(invalidStatusMsg);
                    _logger.LogWarning(invalidStatusMsg);
                    continue;
                }

                // ==================================================
                // 4. XÓA MỀM (GIỐNG Division)
                // ==================================================
                var deleteResult = await _softDeleteService.RecursiveSoftDelete(
                    transferCompany.Id,
                    typeof(Domain.Entities.TransferCompany));

                if (deleteResult.Succeeded)
                {
                    successCount++;
                }
                else
                {
                    failCount++;

                    var deleteFailMsg = _localizer.Format(
                        LocalizationKey.EntityDeleteFailed,
                        _localizer[EntityName.TransferCompany],
                        transferCompany.TransferCompanyCode,
                        deleteResult.Errors);

                    failMessages.Add(deleteFailMsg);
                    _logger.LogWarning(deleteFailMsg);
                }
            }

            // ==================================================
            // 5. Thông điệp tổng hợp kết quả
            // ==================================================
            var msg = _localizer.Format(
                LocalizationKey.MSG_DELETE_RESULT,
                _localizer[EntityName.TransferCompany],
                successCount,
                failCount);

            if (failMessages.Any())
            {
                msg += "\n" + string.Join("\n", failMessages);
            }

            return successCount > 0
                ? Result.Success(msg)
                : Result.Failure(msg);
        }
    }
}
