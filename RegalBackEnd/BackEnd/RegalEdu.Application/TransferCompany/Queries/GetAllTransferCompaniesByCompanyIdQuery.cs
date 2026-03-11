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
    // QUERY: Lấy danh sách Phiếu Chuyển theo Chi nhánh bao gồm cả chi nhánh nguồn và chi nhánh đích
    // ======================================================
    public class GetAllTransferCompaniesByCompanyIdQuery
        : IRequest<Result<List<TransferCompanyModel>>>
    {
        public Guid CompanyId { get; set; }
    }

    // ======================================================
    // QUERY HANDLER
    // ======================================================
    public class GetAllTransferCompaniesByCompanyIdQueryHandler
        : IRequestHandler<GetAllTransferCompaniesByCompanyIdQuery, Result<List<TransferCompanyModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllTransferCompaniesByCompanyIdQueryHandler(
            IRegalEducationDbContext context,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<List<TransferCompanyModel>>> Handle(
            GetAllTransferCompaniesByCompanyIdQuery request,
            CancellationToken cancellationToken)
        {
            // ==================================================
            // 1. Lấy danh sách Phiếu Chuyển theo Chi nhánh
            // ==================================================
            var transferCompanies = await _context.TransferCompanies
                .Where(tc =>
                    !tc.IsDeleted &&
                    (
                        tc.SourceCompanyId == request.CompanyId ||
                        tc.DestinationCompanyId == request.CompanyId
                    ))
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