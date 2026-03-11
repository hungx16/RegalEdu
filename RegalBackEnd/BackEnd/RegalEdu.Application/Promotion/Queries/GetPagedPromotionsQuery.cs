using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Promotion.Queries
{
    public class PromotionQuery
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? PromotionGroupId { get; set; }
        public DateTime? FromStart { get; set; }
        public DateTime? ToEnd { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedPromotionsQuery : IRequest<Result<PagedResult<PromotionModel>>>
    {
        public required PromotionQuery PromotionQuery { get; set; }
    }

    public class GetPagedPromotionsQueryHandler : IRequestHandler<GetPagedPromotionsQuery, Result<PagedResult<PromotionModel>>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly PagingOptions _paging;

        public GetPagedPromotionsQueryHandler(IRegalEducationDbContext db, PagingOptions paging)
        {
            _db = db; _paging = paging;
        }

        public async Task<Result<PagedResult<PromotionModel>>> Handle(GetPagedPromotionsQuery request, CancellationToken ct)
        {
            var q = request.PromotionQuery;

            var query = _db.Promotions.AsNoTracking().Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(q.Name)) query = query.Where(p => p.Name.Contains(q.Name));
            if (!string.IsNullOrWhiteSpace(q.Code)) query = query.Where(p => p.Code!.Contains(q.Code));
            if (q.CompanyId.HasValue) query = query.Where(p => p.CompanyId == q.CompanyId);
            if (q.CourseId.HasValue) query = query.Where(p => p.CourseId == q.CourseId);
            //if (q.StudentId.HasValue) query = query.Where(p => p.StudentId == q.StudentId);
            if (q.PromotionGroupId.HasValue) query = query.Where(p => p.PromotionGroupId == q.PromotionGroupId);
            if (q.FromStart.HasValue) query = query.Where(p => p.StartDate >= q.FromStart.Value);
            if (q.ToEnd.HasValue) query = query.Where(p => p.EndDate <= q.ToEnd.Value);

            int total = await query.CountAsync(ct);
            q.PageSize = q.PageSize > 0 ? q.PageSize : _paging.DefaultPageSize;

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(p => new PromotionModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    ApplyWith = p.ApplyWith,
                    CompanyId = p.CompanyId,
                    CourseId = p.CourseId,
                    //StudentId = p.StudentId,
                    CodeUsage = p.CodeUsage,
                    PromoCode = p.PromoCode,
                    Type = p.Type,
                    Qtymonth = p.Qtymonth,
                    AllCompany = p.AllCompany,
                    AllCourse = p.AllCourse,
                    AllStudent = p.AllStudent,
                    Code = p.Code,
                    PromotionGroupId = p.PromotionGroupId,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync(ct);

            return Result<PagedResult<PromotionModel>>.Success(new PagedResult<PromotionModel> { Items = items, Total = total });
        }
    }
}
