using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RegalEdu.Application.TransferCompany.Commands
{
    // ============================================================
    // 1. COMMAND: Yêu cầu thêm mới Phiếu Chuyển Chi Nhánh
    // ============================================================
    public class AddTransferCompanyCommand : IRequest<Result>
    {
        // Model dữ liệu nhận từ client (KHÔNG chứa dữ liệu suy diễn)
        public required TransferCompanyModel TransferCompanyModel { get; set; }
    }

    // ============================================================
    // 2. COMMAND HANDLER: Xử lý nghiệp vụ tạo Phiếu Chuyển Chi Nhánh
    // ============================================================
    public class AddTransferCompanyCommandHandler
        : IRequestHandler<AddTransferCompanyCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        // ------------------------------------------------------------
        // Constructor – Inject các dependency cần thiết
        // ------------------------------------------------------------
        public AddTransferCompanyCommandHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        // ------------------------------------------------------------
        // 3. HANDLE: Thực thi nghiệp vụ
        // ------------------------------------------------------------
        public async Task<Result> Handle(
            AddTransferCompanyCommand request,
            CancellationToken cancellationToken)
        {
            // ========================================================
            // 3.1 Lấy cấu hình AutoCode cho Phiếu Chuyển Chi Nhánh
            // ========================================================
            var info = AutoCodeConfig.Get(AutoCodeType.TransferCompany);

            // ========================================================
            // 3.2 Đảm bảo DbContext hợp lệ (bắt buộc cho AutoCode retry)
            // ========================================================
            if (_context is not DbContext dbContext)
            {
                throw new InvalidOperationException(
                    _localizer[LocalizationKey.InvalidDbContextInstance]);
            }

            // ========================================================
            // 3.3 Tạo dữ liệu với AutoCode + Retry
            // ========================================================
            var result = await AutoCodeHelper.CreateWithAutoCodeRetryAsync(
                info,
                async (code) =>
                {
                    // ====================================================
                    // 1. LẤY THÔNG TIN HỌC VIÊN (NGUỒN)
                    // ====================================================
                    var student = await _context.Students
                        .FirstOrDefaultAsync(
                            s => s.Id == request.TransferCompanyModel.SourceStudentId
                                 && !s.IsDeleted,
                            cancellationToken);

                    // Không tìm thấy học viên → lỗi nghiệp vụ
                    if (student == null)
                    {
                        return Result.Failure(
                            _localizer["StudentNotFound"]);
                    }

                    // ====================================================
                    // 2. MAP MODEL → ENTITY
                    // ====================================================
                    var transferCompany =
                        _mapper.Map<Domain.Entities.TransferCompany>(
                            request.TransferCompanyModel);

                    // ====================================================
                    // 3. SINH DỮ LIỆU TỰ ĐỘNG (DERIVED DATA)
                    // ====================================================

                    // Sinh mã phiếu tự động
                    transferCompany.TransferCompanyCode = code;

                    // Tên học viên lấy từ bảng Student
                    transferCompany.SourceStudentName = student.FullName;

                    if (!student.CompanyId.HasValue)
                    {
                        return Result.Failure(
                            _localizer["StudentCompanyNotFound"]);
                    }
                    // Chi nhánh hiện tại của học viên
                    transferCompany.SourceCompanyId = student.CompanyId.Value;

                    // Đảm bảo khóa ngoại
                    transferCompany.SourceStudentId = student.Id;

                    // Trạng thái khởi tạo mặc định
                    transferCompany.TransferCompanyStatus =
                        Domain.Enums.TransferCompanyStatus.Draft;

                    // ====================================================
                    // 4. LƯU DATABASE
                    // ====================================================
                    await _context.TransferCompanies.AddAsync(
                        transferCompany, cancellationToken);

                    var success =
                        await _context.SaveChangesAsync(cancellationToken) > 0;

                    // ====================================================
                    // 5. TRẢ KẾT QUẢ
                    // ====================================================
                    if (success)
                    {
                        return Result.Success(
                            _localizer.Format(
                                LocalizationKey.MSG_CREATE_SUCCESS,
                                EntityName.TransferCompany));
                    }

                    return Result.Failure(
                        _localizer.Format(
                            LocalizationKey.ERR_SAVE_NO_EFFECT,
                            EntityName.TransferCompany));
                },
                dbContext
            );

            return result;
        }
    }
}
