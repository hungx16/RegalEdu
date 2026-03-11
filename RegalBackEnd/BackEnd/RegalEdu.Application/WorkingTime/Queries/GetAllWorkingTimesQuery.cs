using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.WorkingTime.Queries
{
    public class GetAllWorkingTimesQuery : IRequest<Result<List<WorkingTimeModel>>>
    {
        public class GetAllWorkingTimesQueryHandler : IRequestHandler<GetAllWorkingTimesQuery, Result<List<WorkingTimeModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetAllWorkingTimesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<WorkingTimeModel>>> Handle(GetAllWorkingTimesQuery request, CancellationToken cancellationToken)
            {
                var workingTimes = await _context.WorkingTimes
                    .AsNoTracking ( )
                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<WorkingTimeModel>> (workingTimes);
                return Result<List<WorkingTimeModel>>.Success (result);
            }
        }
    }
}
