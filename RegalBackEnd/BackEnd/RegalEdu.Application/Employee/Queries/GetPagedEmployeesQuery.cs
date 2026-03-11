using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Employee.Queries
{
    public class EmployeeQuery
    {
        public Guid? ApplicationUserId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? EmployeeTax { get; set; }
        public bool? IsSupport { get; set; }
        public bool? OperationalSupportTeam { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedEmployeesQuery : IRequest<Result<PagedResult<EmployeeModel>>>
    {
        public EmployeeQuery? EmployeeQuery { get; set; }
    }

    public class GetPagedEmployeesQueryHandler : IRequestHandler<GetPagedEmployeesQuery, Result<PagedResult<EmployeeModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _pagingOptions;
        private readonly IMapper _mapper;

        public GetPagedEmployeesQueryHandler(
            IRegalEducationDbContext context,
            PagingOptions pagingOptions,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<PagedResult<EmployeeModel>>> Handle(GetPagedEmployeesQuery request, CancellationToken cancellationToken)
        {
            if (request.EmployeeQuery == null)
                throw new ArgumentNullException (nameof (request.EmployeeQuery));

            var query = _context.Employees
                .Include (e => e.Position)
                .Include (e => e.Department)
                .AsNoTracking ( );

            if (request.EmployeeQuery.ApplicationUserId.HasValue)
                query = query.Where (e => e.ApplicationUserId == request.EmployeeQuery.ApplicationUserId.Value);
            if (request.EmployeeQuery.CompanyId.HasValue)
                query = query.Where (e => e.CompanyId == request.EmployeeQuery.CompanyId.Value);
            if (request.EmployeeQuery.PositionId.HasValue)
                query = query.Where (e => e.PositionId == request.EmployeeQuery.PositionId.Value);
            if (request.EmployeeQuery.DepartmentId.HasValue)
                query = query.Where (e => e.DepartmentId == request.EmployeeQuery.DepartmentId.Value);
            if (!string.IsNullOrWhiteSpace (request.EmployeeQuery.EmployeeTax))
                query = query.Where (e => e.EmployeeTax.Contains (request.EmployeeQuery.EmployeeTax));
            if (request.EmployeeQuery.IsSupport.HasValue)
                query = query.Where (e => e.IsSupport == request.EmployeeQuery.IsSupport.Value);


            int totalRecords = await query.CountAsync (cancellationToken);
            request.EmployeeQuery.PageSize = _pagingOptions.DefaultPageSize;
            var paged = await query
                .OrderByDescending (x => x.CreatedAt)
                .Skip ((request.EmployeeQuery.Page - 1) * request.EmployeeQuery.PageSize)
                .Take (request.EmployeeQuery.PageSize)
                .ToListAsync (cancellationToken);

            var result = paged.Select (e => _mapper.Map<EmployeeModel> (e)).ToList ( );

            var pagedResult = new PagedResult<EmployeeModel>
            {
                Items = result,
                Total = totalRecords
            };

            return Result<PagedResult<EmployeeModel>>.Success (pagedResult);
        }
    }
}
