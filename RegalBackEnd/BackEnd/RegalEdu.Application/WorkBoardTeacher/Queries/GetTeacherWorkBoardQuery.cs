using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.WorkBoardTeacher.Queries
{
    /// <summary>
    /// Lấy bảng công theo giáo viên
    /// </summary>
    public class GetTeacherWorkBoardQuery : IRequest<Result<List<WorkBoardTeacherModel>>>
    {
        public required Guid TeacherId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public class GetTeacherWorkBoardQueryHandler : IRequestHandler<GetTeacherWorkBoardQuery, Result<List<WorkBoardTeacherModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetTeacherWorkBoardQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Result<List<WorkBoardTeacherModel>>> Handle(GetTeacherWorkBoardQuery request, CancellationToken cancellationToken)
            {
                var query = _context.WorkBoardTeachers
                    .Where(wbt => wbt.TeacherId == request.TeacherId && !wbt.IsDeleted);

                if (request.FromDate.HasValue)
                    query = query.Where(wbt => wbt.Date >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(wbt => wbt.Date <= request.ToDate.Value);

                var workBoards = await query
                    .OrderByDescending(wbt => wbt.Date)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                var result = _mapper.Map<List<WorkBoardTeacherModel>>(workBoards);
                return Result<List<WorkBoardTeacherModel>>.Success(result);
            }
        }
    }
}