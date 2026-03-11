using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.WorkingTimeConfiguration.Queries
{
    public class GetAllWorkingTimeConfigurationsQuery : IRequest<Result<List<WorkingTimeConfigurationModel>>>
    {
        public class GetAllWorkingTimeConfigurationsQueryHandler : IRequestHandler<GetAllWorkingTimeConfigurationsQuery, Result<List<WorkingTimeConfigurationModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetAllWorkingTimeConfigurationsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<WorkingTimeConfigurationModel>>> Handle(GetAllWorkingTimeConfigurationsQuery request, CancellationToken cancellationToken)
            {
                var configs = await _context.WorkingTimeConfigurations
                    .Include (w => w.WorkingTimes)
                    .Include (w => w.Holidays)
                    .Include (w => w.WorkingTimeConfigurationCompanies)
                    .AsSplitQuery ( )
                    .AsNoTracking ( )
                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<WorkingTimeConfigurationModel>> (configs);
                return Result<List<WorkingTimeConfigurationModel>>.Success (result);
            }
        }
    }
}
