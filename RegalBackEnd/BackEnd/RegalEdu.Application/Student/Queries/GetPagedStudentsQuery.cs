using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Student.Queries
{
    public class StudentQuery
    {
        public string? StudentCode { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? EmployeeId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedStudentsQuery : IRequest<Result<PagedResult<StudentModel>>>
    {
        public required StudentQuery StudentQuery { get; set; }
    }

    public class GetPagedStudentsQueryHandler : IRequestHandler<GetPagedStudentsQuery, Result<PagedResult<StudentModel>>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly PagingOptions _paging;

        public GetPagedStudentsQueryHandler(IRegalEducationDbContext db, PagingOptions paging)
        {
            _db = db; _paging = paging;
        }

        public async Task<Result<PagedResult<StudentModel>>> Handle(GetPagedStudentsQuery request, CancellationToken ct)
        {
            var q = request.StudentQuery;
            var query = _db.Students.AsNoTracking().Where(s => !s.IsDeleted);

            if (!string.IsNullOrWhiteSpace(q.StudentCode)) query = query.Where(s => s.StudentCode.Contains(q.StudentCode));
            if (!string.IsNullOrWhiteSpace(q.FullName)) query = query.Where(s => s.FullName.Contains(q.FullName));
            if (!string.IsNullOrWhiteSpace(q.Phone)) query = query.Where(s => s.Phone!.Contains(q.Phone));
            if (q.CompanyId.HasValue) query = query.Where(s => s.CompanyId == q.CompanyId);
            if (q.EmployeeId.HasValue) query = query.Where(s => s.EmployeeId == q.EmployeeId);

            int total = await query.CountAsync(ct);
            q.PageSize = q.PageSize > 0 ? q.PageSize : _paging.DefaultPageSize;

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(s => new StudentModel
                {
                    Id = s.Id,
                    StudentCode = s.StudentCode,
                    FullName = s.FullName,
                    Gender = s.Gender,
                    BirthDate = s.BirthDate,
                    CompanyId = s.CompanyId,
                    EmployeeId = s.EmployeeId,
                    Phone = s.Phone,
                    Email = s.Email,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync(ct);

            return Result<PagedResult<StudentModel>>.Success(new PagedResult<StudentModel> { Items = items, Total = total });
        }
    }
}
