using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.WorkBoardTeacher.Queries
{
    public class GetAllWorkBoardTeachersQuery : IRequest<Result<List<WorkBoardTeacherModel>>>
    {
        public class GetAllWorkBoardTeachersQueryHandler : IRequestHandler<GetAllWorkBoardTeachersQuery, Result<List<WorkBoardTeacherModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetAllWorkBoardTeachersQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Result<List<WorkBoardTeacherModel>>> Handle(GetAllWorkBoardTeachersQuery request, CancellationToken cancellationToken)
            {
                var workBoardTeachers = await _context.WorkBoardTeachers
                        .AsNoTracking().AsSplitQuery()
                        .Include(wbt => wbt.Teacher)
                    .ToListAsync(cancellationToken);

                var result = _mapper.Map<List<WorkBoardTeacherModel>>(workBoardTeachers);
                return Result<List<WorkBoardTeacherModel>>.Success(result);
            }
        }
    }
}