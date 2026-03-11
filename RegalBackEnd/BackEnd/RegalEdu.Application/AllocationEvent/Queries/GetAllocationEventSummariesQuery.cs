using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;

namespace RegalEdu.Application.AllocationEvent.Queries
{
    public class AllocationEventSummaryModel
    {
        public string AllocationCode { get; set; } = string.Empty;
        public string AllocationMonthYear { get; set; } = string.Empty;
        public int RegionCount { get; set; }
        public int CompanyCount { get; set; }
        public int EventCount { get; set; }
        public decimal TotalBudget { get; set; }
        public string AllocationEventStatusName { get; set; } = string.Empty;
    }

    public class GetAllocationEventSummariesQuery : IRequest<Result<List<AllocationEventSummaryModel>>> { }

    public class GetAllocationEventSummariesQueryHandler
        : IRequestHandler<GetAllocationEventSummariesQuery, Result<List<AllocationEventSummaryModel>>>
    {
        private readonly IRegalEducationDbContext _context;

        public GetAllocationEventSummariesQueryHandler(IRegalEducationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
        }

        public async Task<Result<List<AllocationEventSummaryModel>>> Handle(
            GetAllocationEventSummariesQuery request,
            CancellationToken cancellationToken)
        {
            var allocationEvents = await _context.AllocationEvents
                .Where (a => !a.IsDeleted)
                .Include (a => a.AllocationDetails.Where (d => !d.IsDeleted))
                .AsNoTracking ( )
                .Select (a => new
                {
                    a.AllocationCode,
                    a.AllocationMonth,
                    a.AllocationYear,
                    a.AllocationEventStatus,
                    a.EventBudget,
                    RegionCount = a.AllocationDetails.Select (d => d.RegionId).Distinct ( ).Count ( ),
                    CompanyCount = a.AllocationDetails.Select (d => d.CompanyId).Distinct ( ).Count ( ),
                    EventCount = a.AllocationDetails.Sum (d => d.Quantity)
                })
                .Select (x => new AllocationEventSummaryModel
                {
                    AllocationCode = x.AllocationCode,
                    AllocationMonthYear = $"Tháng {x.AllocationMonth}/{x.AllocationYear}",
                    RegionCount = x.RegionCount,
                    CompanyCount = x.CompanyCount,
                    EventCount = x.EventCount,
                    TotalBudget = x.CompanyCount * x.EventBudget,
                    AllocationEventStatusName = x.AllocationEventStatus.ToString ( ),
                })
                .OrderByDescending (a => a.AllocationCode)
                .ToListAsync (cancellationToken);

            return Result<List<AllocationEventSummaryModel>>.Success (allocationEvents);
        }
    }
}
