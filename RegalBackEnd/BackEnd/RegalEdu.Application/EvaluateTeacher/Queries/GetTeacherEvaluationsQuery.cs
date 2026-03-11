using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.EvaluateTeacher.Queries
{
    public class GetTeacherEvaluationsQuery : IRequest<Result<List<EvaluateTeacherModel>>>
    {
        public required Guid TeacherId { get; set; }

        public class GetTeacherEvaluationsQueryHandler : IRequestHandler<GetTeacherEvaluationsQuery, Result<List<EvaluateTeacherModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetTeacherEvaluationsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Result<List<EvaluateTeacherModel>>> Handle(GetTeacherEvaluationsQuery request, CancellationToken cancellationToken)
            {
                var evaluations = await _context.EvaluateTeachers.Include(et => et.Teacher).Include(t => t.ClassSchedule)
                .Include(t => t.Class)
                    .Where(et => et.TeacherId == request.TeacherId && !et.IsDeleted)
                    .OrderByDescending(et => et.ClassSchedule != null ? et.ClassSchedule.Date : DateTime.MinValue)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                var result = _mapper.Map<List<EvaluateTeacherModel>>(evaluations);
                return Result<List<EvaluateTeacherModel>>.Success(result);
            }
        }
    }
}