using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AllocationEvent.Queries
{
    public class GetAllAllocationEventsQuery
        : IRequest<Result<List<AllocationEventModel>>>
    { }

    public class GetAllAllocationEventsQueryHandler
        : IRequestHandler<GetAllAllocationEventsQuery, Result<List<AllocationEventModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllAllocationEventsQueryHandler(
            IRegalEducationDbContext context,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<AllocationEventModel>>> Handle(
            GetAllAllocationEventsQuery request,
            CancellationToken cancellationToken)
        {
            // 🔹 Lấy tất cả AllocationEvent chưa bị xóa mềm
            // 🔹 Bao gồm AllocationDetails chưa bị xóa mềm
            var allocationEvents = await _context.AllocationEvents
                .Where (a => !a.IsDeleted)
                .Include (a => a.AllocationDetails.Where (d => !d.IsDeleted))
                .Include (a => a.AllocationEventHistories.Where (h => !h.IsDeleted))
                .AsNoTracking ( )
                .ToListAsync (cancellationToken);

            // 🔹 Map sang DTO
            var result = _mapper.Map<List<AllocationEventModel>> (allocationEvents);

            return Result<List<AllocationEventModel>>.Success (result);
        }
    }
}
