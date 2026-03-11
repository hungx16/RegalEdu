using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RegisterStudy.Queries
{
    public class RegisterStudyQuery
    {
        public string? Code { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? PromotionId { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime? FromCreatedAt { get; set; }
        public DateTime? ToCreatedAt { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedRegisterStudysQuery : IRequest<Result<PagedResult<RegisterStudyModel>>>
    {
        public required RegisterStudyQuery RegisterStudyQuery { get; set; }
    }

    public class GetPagedRegisterStudysQueryHandler : IRequestHandler<GetPagedRegisterStudysQuery, Result<PagedResult<RegisterStudyModel>>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly PagingOptions _paging;

        public GetPagedRegisterStudysQueryHandler(IRegalEducationDbContext db, PagingOptions paging)
        {
            _db = db; _paging = paging;
        }

        public async Task<Result<PagedResult<RegisterStudyModel>>> Handle(GetPagedRegisterStudysQuery request, CancellationToken ct)
        {
            var q = request.RegisterStudyQuery;

            var query = _db.RegisterStudys.AsNoTracking().Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(q.Code)) query = query.Where(x => x.Code.Contains(q.Code));
            if (q.StudentId.HasValue) query = query.Where(x => x.StudentId == q.StudentId);
            if (q.CompanyId.HasValue) query = query.Where(x => x.CompanyId == q.CompanyId);
            if (q.PromotionId.HasValue) query = query.Where(x => x.PromotionId == q.PromotionId);
            if (q.PaymentStatus.HasValue) query = query.Where(x => x.PaymentStatus == q.PaymentStatus);
            if (q.FromCreatedAt.HasValue) query = query.Where(x => x.CreatedAt >= q.FromCreatedAt);
            if (q.ToCreatedAt.HasValue) query = query.Where(x => x.CreatedAt <= q.ToCreatedAt);

            int total = await query.CountAsync(ct);
            q.PageSize = q.PageSize > 0 ? q.PageSize : _paging.DefaultPageSize;

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(x => new RegisterStudyModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    CouponCode = x.CouponCode,
                    StudentId = x.StudentId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    EmployeeId = x.EmployeeId,
                    TeacherId = x.TeacherId,
                    PromotionId = x.PromotionId,
                    PaymentStatus = x.PaymentStatus,
                    TotalAmount = x.TotalAmount,
                    CreatedAt = x.CreatedAt
                    
                })
                .ToListAsync(ct);

            return Result<PagedResult<RegisterStudyModel>>.Success(new PagedResult<RegisterStudyModel> { Items = items, Total = total });
        }
    }
}
