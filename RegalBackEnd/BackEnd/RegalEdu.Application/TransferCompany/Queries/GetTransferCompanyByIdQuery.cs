using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RegalEdu.Application.TransferCompany.Queries
{
    // ======================================================
    // QUERY: Lấy chi tiết Phiếu Chuyển theo Id
    // ======================================================
    public class GetTransferCompanyByIdQuery
        : IRequest<Result<TransferCompanyModel>>
    {
        public Guid Id { get; set; }
    }

    // ======================================================
    // QUERY HANDLER
    // ======================================================
    public class GetTransferCompanyByIdQueryHandler
        : IRequestHandler<GetTransferCompanyByIdQuery, Result<TransferCompanyModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetTransferCompanyByIdQueryHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result<TransferCompanyModel>> Handle(
            GetTransferCompanyByIdQuery request,
            CancellationToken cancellationToken)
        {
            // ==================================================
            // 1. Tìm Phiếu Chuyển theo Id
            // ==================================================
            var transferCompany = await _context.TransferCompanies
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    tc => tc.Id == request.Id && !tc.IsDeleted,
                    cancellationToken);

            // Không tìm thấy → lỗi nghiệp vụ
            if (transferCompany == null)
            {
                return Result<TransferCompanyModel>.Failure(
                    _localizer.Format(
                        LocalizationKey.EntityNotFound,
                        EntityName.TransferCompany));
            }

            // ==================================================
            // 2. Map Entity → Model
            // ==================================================
            var result = _mapper.Map<TransferCompanyModel>(transferCompany);

            // ==================================================
            // 3. Trả kết quả
            // ==================================================
            return Result<TransferCompanyModel>.Success(result);
        }
    }
}
