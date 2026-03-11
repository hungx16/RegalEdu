using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Employee.Queries
{
    public class GetAllEmployeesQuery : IRequest<Result<List<EmployeeModel>>> { }

    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, Result<List<EmployeeModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllEmployeesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<EmployeeModel>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _context.Employees
                    .Where (e => _context.Companies.Any (c => c.Id == e.CompanyId))  // Chỉ lấy Employees có CompanyId hợp lệ

                                    .Include (t => t.Company)
                                    .Include (t => t.ApplicationUser)
                                    .Include (e => e.Position)
                                    .Include (e => e.Department)
                                    .AsNoTracking ( ).ToListAsync ( );
            var result = _mapper.Map<List<EmployeeModel>> (employees);
            return Result<List<EmployeeModel>>.Success (result);
        }
    }
}
