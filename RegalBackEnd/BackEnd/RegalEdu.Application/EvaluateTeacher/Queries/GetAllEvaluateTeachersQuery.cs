using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.EvaluateTeacher.Queries
{
    public class GetAllEvaluateTeachersQuery : IRequest<Result<List<EvaluateTeacherModel>>>
    {
        public class GetAllEvaluateTeachersQueryHandler : IRequestHandler<GetAllEvaluateTeachersQuery, Result<List<EvaluateTeacherModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetAllEvaluateTeachersQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Result<List<EvaluateTeacherModel>>> Handle(GetAllEvaluateTeachersQuery request, CancellationToken cancellationToken)
            {
                var evaluateTeachers = await _context.EvaluateTeachers
                        .AsNoTracking().AsSplitQuery()
                        .Include(et => et.Teacher)
                    .ToListAsync(cancellationToken);

                var result = _mapper.Map<List<EvaluateTeacherModel>>(evaluateTeachers);
                return Result<List<EvaluateTeacherModel>>.Success(result);
            }
        }
    }
}