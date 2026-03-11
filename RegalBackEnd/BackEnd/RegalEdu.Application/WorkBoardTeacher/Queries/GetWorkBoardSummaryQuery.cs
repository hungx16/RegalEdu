using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;

namespace RegalEdu.Application.WorkBoardTeacher.Queries
{
    public class WorkBoardSummaryModel
    {
        public int TotalRecords { get; set; }
        public int OnTimeCount { get; set; }
        public int LateCount { get; set; }
        public int AbsentCount { get; set; }
        public decimal TotalWorkHours { get; set; }
        public int ConfirmedCount { get; set; }
        public int UnconfirmedCount { get; set; }
    }
    /// <summary>
    /// Lấy tổng kết công
    /// </summary>
    public class GetWorkBoardSummaryQuery : IRequest<Result<WorkBoardSummaryModel>>
    {
        public Guid? TeacherId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public class GetWorkBoardSummaryQueryHandler : IRequestHandler<GetWorkBoardSummaryQuery, Result<WorkBoardSummaryModel>>
        {
            private readonly IRegalEducationDbContext _context;

            public GetWorkBoardSummaryQueryHandler(IRegalEducationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<Result<WorkBoardSummaryModel>> Handle(GetWorkBoardSummaryQuery request, CancellationToken cancellationToken)
            {
                var query = _context.WorkBoardTeachers.Where(wbt => !wbt.IsDeleted);

                if (request.TeacherId.HasValue)
                    query = query.Where(wbt => wbt.TeacherId == request.TeacherId.Value);

                if (request.FromDate.HasValue)
                    query = query.Where(wbt => wbt.Date >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(wbt => wbt.Date <= request.ToDate.Value);

                var totalRecords = await query.CountAsync(cancellationToken);
                var onTimeCount = await query.CountAsync(wbt => wbt.Status == 1, cancellationToken);
                var lateCount = await query.CountAsync(wbt => wbt.Status == 2, cancellationToken);
                var absentCount = await query.CountAsync(wbt => wbt.Status == 3, cancellationToken);
                var totalWorkHours = await query.SumAsync(wbt => wbt.WorkHours, cancellationToken);
                var confirmedCount = await query.CountAsync(wbt => wbt.IsConfirmed, cancellationToken);
                var unconfirmedCount = totalRecords - confirmedCount;

                var summary = new WorkBoardSummaryModel
                {
                    TotalRecords = totalRecords,
                    OnTimeCount = onTimeCount,
                    LateCount = lateCount,
                    AbsentCount = absentCount,
                    TotalWorkHours = totalWorkHours,
                    ConfirmedCount = confirmedCount,
                    UnconfirmedCount = unconfirmedCount
                };

                return Result<WorkBoardSummaryModel>.Success(summary);
            }
        }
    }
}