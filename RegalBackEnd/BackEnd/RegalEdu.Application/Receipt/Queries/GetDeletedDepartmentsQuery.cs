using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Receipt.Queries
{
    public class GetDeletedDepartmentsQuery : IRequest<Result<List<DepartmentModel>>> { }

    public class GetDeletedDepartmentsQueryHandler : IRequestHandler<GetDeletedDepartmentsQuery, Result<List<DepartmentModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetDeletedDepartmentsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper)); ;
        }

        public async Task<Result<List<DepartmentModel>>> Handle(GetDeletedDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var departments = await _context.Departments.Include (t => t.Division)
                .IgnoreQueryFilters ( ) // Lấy tất cả các phòng ban, bao gồm cả những phòng ban đã xóa
                .Where (d => d.IsDeleted) // Chỉ lấy những phòng ban đã xóa
                .AsNoTracking ( ).ToListAsync (cancellationToken);
            var result = _mapper.Map<List<DepartmentModel>> (departments);

            return Result<List<DepartmentModel>>.Success (result);
        }
    }
}