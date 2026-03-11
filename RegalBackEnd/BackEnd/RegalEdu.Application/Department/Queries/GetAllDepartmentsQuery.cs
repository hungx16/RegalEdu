using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Department.Queries
{
    public class GetAllDepartmentsQuery : IRequest<Result<List<DepartmentModel>>> { }

    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, Result<List<DepartmentModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllDepartmentsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<DepartmentModel>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var departments = await _context.Departments.Include (t => t.Division)
                .AsNoTracking ( ).ToListAsync (cancellationToken);
            var result = _mapper.Map<List<DepartmentModel>> (departments);

            return Result<List<DepartmentModel>>.Success (result);
        }
    }
}
