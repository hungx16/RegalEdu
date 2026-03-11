using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.WorkingTime.Queries
{
    public class GetAllWorkingTimesByConfigIdQuery : IRequest<Result<List<WorkingTimeModel>>>
    {
        public required string ConfigurationId { get; set; }

        public class GetAllWorkingTimesByConfigIdQueryHandler : IRequestHandler<GetAllWorkingTimesByConfigIdQuery, Result<List<WorkingTimeModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public GetAllWorkingTimesByConfigIdQueryHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result<List<WorkingTimeModel>>> Handle(GetAllWorkingTimesByConfigIdQuery request, CancellationToken cancellationToken)
            {
                var workingTime = await _context.WorkingTimes
                    .AsNoTracking ( )
                    .Where (x => x.WorkingTimeConfigurationId.ToString ( ) == request.ConfigurationId).ToListAsync (cancellationToken);

                var result = _mapper.Map<List<WorkingTimeModel>> (workingTime);
                return Result<List<WorkingTimeModel>>.Success (result);
            }
        }
    }
}
