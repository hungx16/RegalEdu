using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AllocationEvent.Queries
{
    public class GetAllAllocationEventsForRegionQuery
        : IRequest<Result<List<AllocationEventModel>>>
    { }

    public class GetAllAllocationEventsForRegionQueryHandler
        : IRequestHandler<GetAllAllocationEventsForRegionQuery, Result<List<AllocationEventModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetAllAllocationEventsForRegionQueryHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _currentUserService = currentUserService ?? throw new ArgumentNullException (nameof (currentUserService));
        }

        public async Task<Result<List<AllocationEventModel>>> Handle(
            GetAllAllocationEventsForRegionQuery request,
            CancellationToken cancellationToken)
        {
            var employeeId = Guid.Empty;
            if (!Guid.TryParse (_currentUserService.EmployeeId, out employeeId))
            {
                return Result<List<AllocationEventModel>>.Success (new List<AllocationEventModel> ( ));
            }
            // 🔹 Lấy tất cả AllocationEvent chưa bị xóa mềm
            // 🔹 Bao gồm AllocationDetails chưa bị xóa mềm
            var allocationEvents = await _context.AllocationEvents
               .Where (a => !a.IsDeleted && a.AllocationEventStatus != Domain.Enums.AllocationEventStatus.Draft && a.AllocationEventStatus != Domain.Enums.AllocationEventStatus.Cancelled)
               .Where (a => a.AllocationDetails.Any (d => !d.IsDeleted &&
                                                        d.Region != null &&
                                                        d.Region.ManagerId == employeeId))
               .Select (a => new AllocationEventModel
               {
                   Id = a.Id,
                   AllocationCode = a.AllocationCode,
                   AllocationMonth = a.AllocationMonth,
                   AllocationYear = a.AllocationYear,
                   AllocationEventStatus = a.AllocationEventStatus,
                   EventBudget = a.EventBudget,

                   // Map allocation details filtered to regions managed by current employee
                   AllocationDetails = a.AllocationDetails
                       .Where (d => !d.IsDeleted &&
                                   d.Region != null &&
                                   d.Region.ManagerId == employeeId)
                       .Select (d => new AllocationDetailEventModel
                       {

                           AllocationEventId = d.AllocationEventId,
                           RegionId = d.RegionId,
                           Status = d.Status,
                           NoAllocation = d.NoAllocation,
                           CompanyId = d.CompanyId,
                           EventId = d.EventId,
                           Quantity = d.Quantity,
                           Budget = d.Budget

                           // If there are other domain-specific scalar fields (e.g., Quota, Note, AssignedCount),
                           // add them here as: FieldName = d.FieldName,
                       }).ToList ( ),
                   AllocationEventHistories = a.AllocationEventHistories
                       .Where (h => !h.IsDeleted)
                       .Select (h => new AllocationEventHistoryModel
                       {
                           Id = h.Id,
                           AllocationEventId = h.AllocationEventId,
                           TargetName = h.TargetName,
                           ActionName = h.ActionName,
                           Description = h.Description,
                           CreatedAt = h.CreatedAt,
                           UpdatedAt = h.UpdatedAt,
                           IsDeleted = h.IsDeleted,
                           CreatedBy = h.CreatedBy,
                           UpdatedBy = h.UpdatedBy,
                           Status = h.Status
                       })
                       .ToList ( )
               })
               .AsNoTracking ( )
               .ToListAsync (cancellationToken);


            // 🔹 Map sang DTO (kept as in original flow)
            var result = _mapper.Map<List<AllocationEventModel>> (allocationEvents);

            return Result<List<AllocationEventModel>>.Success (result);
        }
    }
}
