using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.WorkingTime.Queries
{
    public class GetWorkingTimeByIdQuery : IRequest<Result<WorkingTimeModel>>
    {
        public required string Id { get; set; }

        public class GetWorkingTimeByIdQueryHandler : IRequestHandler<GetWorkingTimeByIdQuery, Result<WorkingTimeModel>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public GetWorkingTimeByIdQueryHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result<WorkingTimeModel>> Handle(GetWorkingTimeByIdQuery request, CancellationToken cancellationToken)
            {
                var workingTime = await _context.WorkingTimes
                    .AsNoTracking ( )
                    .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted, cancellationToken);

                if (workingTime == null)
                {
                    var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["WorkingTime"], request.Id);
                    return Result<WorkingTimeModel>.Failure (msg);
                }

                var result = _mapper.Map<WorkingTimeModel> (workingTime);
                return Result<WorkingTimeModel>.Success (result);
            }
        }
    }
}
