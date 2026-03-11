using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.WorkingTimeConfiguration.Queries
{
    public class GetDeletedWorkingTimeConfigurationsQuery : IRequest<Result<List<WorkingTimeConfigurationModel>>>
    {
        public class GetDeletedWorkingTimeConfigurationsQueryHandler : IRequestHandler<GetDeletedWorkingTimeConfigurationsQuery, Result<List<WorkingTimeConfigurationModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetDeletedWorkingTimeConfigurationsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<WorkingTimeConfigurationModel>>> Handle(GetDeletedWorkingTimeConfigurationsQuery request, CancellationToken cancellationToken)
            {
                var configs = await _context.WorkingTimeConfigurations
                    .IgnoreQueryFilters ( )
                    .Include (w => w.WorkingTimes)
                    .Include (w => w.Holidays)
                    .Include (w => w.WorkingTimeConfigurationCompanies)
                    .Where (x => x.IsDeleted)
                    .AsNoTracking ( )
                    .ToListAsync (cancellationToken);

                var result = _mapper.Map<List<WorkingTimeConfigurationModel>> (configs);
                return Result<List<WorkingTimeConfigurationModel>>.Success (result);
            }
        }
    }
}
