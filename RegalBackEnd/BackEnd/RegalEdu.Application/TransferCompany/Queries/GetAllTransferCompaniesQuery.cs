using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RegalEdu.Application.TransferCompany.Queries
{
    // ======================================================
    // QUERY: Lấy toàn bộ Phiếu Chuyển Chi Nhánh
    // ======================================================
    public class GetAllTransferCompaniesQuery
        : IRequest<Result<List<TransferCompanyModel>>>
    {
        // Query này KHÔNG cần tham số
    }

    // ======================================================
    // QUERY HANDLER
    // ======================================================
    public class GetAllTransferCompaniesQueryHandler
        : IRequestHandler<GetAllTransferCompaniesQuery, Result<List<TransferCompanyModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllTransferCompaniesQueryHandler(
            IRegalEducationDbContext context,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<List<TransferCompanyModel>>> Handle(
            GetAllTransferCompaniesQuery request,
            CancellationToken cancellationToken)
        {
            // ==================================================
            // 1. Lấy toàn bộ Phiếu Chuyển (chưa bị xóa)
            // ==================================================
            var transferCompanies = await _context.TransferCompanies
                .Where(tc => !tc.IsDeleted)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // ==================================================
            // 2. Map Entity → Model
            // ==================================================
            var result = _mapper.Map<List<TransferCompanyModel>>(transferCompanies);

            // ==================================================
            // 3. Trả kết quả
            // ==================================================
            return Result<List<TransferCompanyModel>>.Success(result);
        }
    }
}
