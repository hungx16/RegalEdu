using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AllocationEvent.Queries
{
    public class GetAllocationEventByIdQuery : IRequest<Result<AllocationEventModel>>
    {
        public required string Id { get; set; }
    }

    public class GetAllocationEventByIdQueryHandler
        : IRequestHandler<GetAllocationEventByIdQuery, Result<AllocationEventModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetAllocationEventByIdQueryHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result<AllocationEventModel>> Handle(GetAllocationEventByIdQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.Id, out var guidId))
                return Result<AllocationEventModel>.Failure("Invalid ID format.");

            var allocationEvent = await _context.AllocationEvents
                .Include(a => a.AllocationDetails.Where(d => !d.IsDeleted))
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == guidId && !x.IsDeleted, cancellationToken);

            if (allocationEvent == null)
            {
                var msg = _localizer.Format(
                    LocalizationKey.EntityWithIdNotFound,
                    _localizer[EntityName.AllocationEvent],
                    request.Id);

                return Result<AllocationEventModel>.Failure(msg);
            }

            var result = _mapper.Map<AllocationEventModel>(allocationEvent);
            return Result<AllocationEventModel>.Success(result);
        }
    }
}
