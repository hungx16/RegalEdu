using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Department.Queries
{
    public class DepartmentQuery
    {
        public string? DepartmentCode { get; set; }
        public string? DepartmentName { get; set; }
        public byte? Status { get; set; }
        public Guid? DivisionId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedDepartmentsQuery : IRequest<Result<PagedResult<DepartmentModel>>>
    {
        public DepartmentQuery? DepartmentQuery { get; set; }
    }

    public class GetPagedDepartmentsQueryHandler : IRequestHandler<GetPagedDepartmentsQuery, Result<PagedResult<DepartmentModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _pagingOptions;

        public GetPagedDepartmentsQueryHandler(IRegalEducationDbContext context, PagingOptions pagingOptions)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
        }

        public async Task<Result<PagedResult<DepartmentModel>>> Handle(GetPagedDepartmentsQuery request, CancellationToken cancellationToken)
        {
            if (request.DepartmentQuery == null)
            {
                throw new ArgumentNullException (nameof (request.DepartmentQuery));
            }
            var query = _context.Departments.AsNoTracking ( ).Where (d => !d.IsDeleted);

            if (!string.IsNullOrWhiteSpace (request.DepartmentQuery.DepartmentCode))
            {
                query = query.Where (d => d.DepartmentCode.Contains (request.DepartmentQuery.DepartmentCode));
            }
            if (!string.IsNullOrWhiteSpace (request.DepartmentQuery.DepartmentName))
            {
                query = query.Where (d => d.DepartmentName.Contains (request.DepartmentQuery.DepartmentName));
            }

            if (request.DepartmentQuery.DivisionId.HasValue)
            {
                query = query.Where (d => d.DivisionId == request.DepartmentQuery.DivisionId.Value);
            }

            int totalRecords = await query.CountAsync (cancellationToken);
            request.DepartmentQuery.PageSize = _pagingOptions.DefaultPageSize;
            var paged = await query
                .OrderByDescending (x => x.CreatedAt)
                .Skip ((request.DepartmentQuery.Page - 1) * request.DepartmentQuery.PageSize)
                .Take (request.DepartmentQuery.PageSize)
                .ToListAsync (cancellationToken);

            var result = paged.Select (d => new DepartmentModel
            {
                Id = d.Id,
                DepartmentCode = d.DepartmentCode,
                DepartmentName = d.DepartmentName,
                DivisionId = d.DivisionId,
                Description = d.Description,
                Status = d.Status,
                CreatedAt = d.CreatedAt,
            }).ToList ( );

            var pagedResult = new PagedResult<DepartmentModel>
            {
                Items = result,
                Total = totalRecords
            };

            return Result<PagedResult<DepartmentModel>>.Success (pagedResult);
        }
    }
}
