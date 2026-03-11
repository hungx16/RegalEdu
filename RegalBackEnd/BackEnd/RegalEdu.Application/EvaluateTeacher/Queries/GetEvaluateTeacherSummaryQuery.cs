using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;

namespace RegalEdu.Application.EvaluateTeacher.Queries
{
    public class EvaluateTeacherSummaryModel
    {
        public int TotalEvaluations { get; set; }
        public double AverageStar { get; set; }
        public Dictionary<string, int> EvaluationsByType { get; set; } = new();
        public Dictionary<int, int> StarDistribution { get; set; } = new();
    }

    public class GetEvaluateTeacherSummaryQuery : IRequest<Result<EvaluateTeacherSummaryModel>>
    {
        public Guid? TeacherId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public class GetEvaluateTeacherSummaryQueryHandler : IRequestHandler<GetEvaluateTeacherSummaryQuery, Result<EvaluateTeacherSummaryModel>>
        {
            private readonly IRegalEducationDbContext _context;

            public GetEvaluateTeacherSummaryQueryHandler(IRegalEducationDbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<Result<EvaluateTeacherSummaryModel>> Handle(GetEvaluateTeacherSummaryQuery request, CancellationToken cancellationToken)
            {
                var query = _context.EvaluateTeachers.Where(et => !et.IsDeleted);

                if (request.TeacherId.HasValue)
                    query = query.Where(et => et.TeacherId == request.TeacherId.Value);

                if (request.FromDate.HasValue)
                    query = query.Where(et => et.EvaluateDate >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(et => et.EvaluateDate <= request.ToDate.Value);

                var totalEvaluations = await query.CountAsync(cancellationToken);

                var averageStar = totalEvaluations > 0
                    ? await query.AverageAsync(et => et.StarRating ?? 0, cancellationToken)
                    : 0;

                var evaluationsByType = await query
                    .GroupBy(et => et.EvaluateType ?? "Unknown")
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Type, x => x.Count, cancellationToken);

                var starDistribution = new Dictionary<int, int>();
                for (var i = 1; i <= 5; i++)
                {
                    var lower = i - 1;
                    var upper = i;
                    var count = await query.CountAsync(et => et.StarRating >= lower && et.StarRating < upper, cancellationToken);
                    starDistribution[i] = count;
                }

                var summary = new EvaluateTeacherSummaryModel
                {
                    TotalEvaluations = totalEvaluations,
                    AverageStar = Math.Round(averageStar, 2),
                    EvaluationsByType = evaluationsByType,
                    StarDistribution = starDistribution
                };

                return Result<EvaluateTeacherSummaryModel>.Success(summary);
            }
        }
    }
}
