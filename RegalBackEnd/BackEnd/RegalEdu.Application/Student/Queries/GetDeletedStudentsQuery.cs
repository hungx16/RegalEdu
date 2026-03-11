using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Student.Queries
{
    public class GetDeletedStudentsQuery : IRequest<Result<List<StudentModel>>>
    {
        public class GetDeletedStudentsQueryHandler : IRequestHandler<GetDeletedStudentsQuery, Result<List<StudentModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetDeletedStudentsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<StudentModel>>> Handle(GetDeletedStudentsQuery request, CancellationToken cancellationToken)
            {
                var regions = await _context.Students
                    .IgnoreQueryFilters ( )
                    //.Include (r => r.Companies)
                    .Where (r => r.IsDeleted)
                    .AsNoTracking ( )
                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<StudentModel>> (regions);
                return Result<List<StudentModel>>.Success (result);
            }
        }
    }
}
