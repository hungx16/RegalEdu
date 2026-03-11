using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Teacher.Queries
{
    public class GetAllTeachersQuery : IRequest<Result<List<TeacherModel>>>
    {
        public class GetAllTeachersQueryHandler : IRequestHandler<GetAllTeachersQuery, Result<List<TeacherModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetAllTeachersQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<TeacherModel>>> Handle(GetAllTeachersQuery request, CancellationToken cancellationToken)
            {
                var teachers = await _context.Teachers
                    .Include (t => t.Company)
                    .Include (t => t.ApplicationUser)
                    .AsNoTracking ( )
                    .AsSplitQuery ( )
                    .ToListAsync (cancellationToken);
                var result = _mapper.Map<List<TeacherModel>> (teachers);

                return Result<List<TeacherModel>>.Success (result);
            }
        }
    }
}
