using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Receipt.Queries
{
    public class ReceiptQuery
    {
        public string? ReceiptCode { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? RegisterStudyId { get; set; }
        public PaymenMeThodType? PaymentMethodType { get; set; }
        public PaymentType? PaymentType { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedReceiptsQuery : IRequest<Result<PagedResult<ReceiptsModel>>>
    {
        public required ReceiptQuery ReceiptQuery { get; set; }
    }

    public class GetPagedReceiptsQueryHandler : IRequestHandler<GetPagedReceiptsQuery, Result<PagedResult<ReceiptsModel>>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly PagingOptions _paging;

        public GetPagedReceiptsQueryHandler(IRegalEducationDbContext db, PagingOptions paging)
        {
            _db = db; _paging = paging;
        }

        public async Task<Result<PagedResult<ReceiptsModel>>> Handle(GetPagedReceiptsQuery request, CancellationToken ct)
        {
            var q = request.ReceiptQuery;
            var query = _db.Receipts.AsNoTracking().Where(s => !s.IsDeleted);

            if (!string.IsNullOrWhiteSpace(q.ReceiptCode)) query = query.Where(s => s.ReceiptCode!.Contains(q.ReceiptCode));
            if (q.StudentId.HasValue) query = query.Where(s => s.StudentId == q.StudentId);
            if (q.CourseId.HasValue) query = query.Where(s => s.CourseId == q.CourseId);
            if (q.EmployeeId.HasValue) query = query.Where(s => s.EmployeeId == q.EmployeeId);
            if (q.RegisterStudyId.HasValue) query = query.Where(s => s.RegisterStudyId == q.RegisterStudyId);
            if (q.PaymentMethodType.HasValue) query = query.Where(s => s.PaymentMethodType == q.PaymentMethodType);
            if (q.PaymentType.HasValue) query = query.Where(s => s.PaymentType == q.PaymentType);
            if (q.CreatedFrom.HasValue) query = query.Where(s => s.CreatedAt >= q.CreatedFrom);
            if (q.CreatedTo.HasValue) query = query.Where(s => s.CreatedAt < q.CreatedTo);

            int total = await query.CountAsync(ct);
            q.PageSize = q.PageSize > 0 ? q.PageSize : _paging.DefaultPageSize;

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(s => new ReceiptsModel
                {
                    Id = s.Id,
                    ReceiptCode = s.ReceiptCode,
                    ReceiptType = s.ReceiptType,
                    StudentId = s.StudentId,
                    CourseId = s.CourseId,
                    EmployeeId = s.EmployeeId,
                    RegisterStudyId = s.RegisterStudyId,
                    PaymentMethodType = s.PaymentMethodType,
                    PaymentType = s.PaymentType,
                    PaymentMethod = s.PaymentMethod,
                    TotalAmount = s.TotalAmount,
                    Note = s.Note,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync(ct);

            return Result<PagedResult<ReceiptsModel>>.Success(new PagedResult<ReceiptsModel> { Items = items, Total = total });
        }
    }
}
