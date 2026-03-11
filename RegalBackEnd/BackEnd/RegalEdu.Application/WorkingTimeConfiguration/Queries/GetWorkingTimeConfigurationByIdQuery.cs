using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.WorkingTimeConfiguration.Queries
{
    public class GetWorkingTimeConfigurationByIdQuery : IRequest<Result<WorkingTimeConfigurationModel>>
    {
        public required string Id { get; set; }

        public class GetWorkingTimeConfigurationByIdQueryHandler : IRequestHandler<GetWorkingTimeConfigurationByIdQuery, Result<WorkingTimeConfigurationModel>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public GetWorkingTimeConfigurationByIdQueryHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result<WorkingTimeConfigurationModel>> Handle(GetWorkingTimeConfigurationByIdQuery request, CancellationToken cancellationToken)
            {
                var entity = await _context.WorkingTimeConfigurations
                    .Include (w => w.WorkingTimes)
                    .Include (w => w.Holidays)
                    .Include (w => w.WorkingTimeConfigurationCompanies)
                    .AsNoTracking ( )
                    .FirstOrDefaultAsync (w => w.Id.ToString ( ) == request.Id && !w.IsDeleted, cancellationToken);

                if (entity == null)
                {
                    var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["WorkingTimeConfiguration"], request.Id);
                    return Result<WorkingTimeConfigurationModel>.Failure (msg);
                }

                var result = _mapper.Map<WorkingTimeConfigurationModel> (entity);
                return Result<WorkingTimeConfigurationModel>.Success (result);
            }
        }
    }
}
