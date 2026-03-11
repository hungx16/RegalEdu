using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Teacher.Queries
{
    public class GetDeletedTeachersQuery : IRequest<Result<List<TeacherModel>>>
    {
        public class GetDeletedTeachersQueryHandler : IRequestHandler<GetDeletedTeachersQuery, Result<List<TeacherModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetDeletedTeachersQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<TeacherModel>>> Handle(GetDeletedTeachersQuery request, CancellationToken cancellationToken)
            {
                var regions = await _context.Teachers
                    .IgnoreQueryFilters ( )
                    //.Include (r => r.Companies)
                    .Where (r => r.IsDeleted)
                    .AsNoTracking ( )
                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<TeacherModel>> (regions);
                return Result<List<TeacherModel>>.Success (result);
            }
        }
    }
}
