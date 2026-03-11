using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Employee.Queries
{
    public class GetDeletedEmployeesQuery : IRequest<Result<List<EmployeeModel>>> { }

    public class GetDeletedEmployeesQueryHandler : IRequestHandler<GetDeletedEmployeesQuery, Result<List<EmployeeModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetDeletedEmployeesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<EmployeeModel>>> Handle(GetDeletedEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _context.Employees
                .IgnoreQueryFilters ( )
                .Include (e => e.Position)
                .Include (e => e.Department)
                .Where (e => e.IsDeleted)
                .AsNoTracking ( )
                .ToListAsync (cancellationToken);

            var result = _mapper.Map<List<EmployeeModel>> (employees);
            return Result<List<EmployeeModel>>.Success (result);
        }
    }
}
