using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Region.Commands
{
    public class UpdateRegionCommand : IRequest<Result>
    {
        public required RegionModel RegionModel { get; set; }

        public class UpdateRegionCommandHandler : IRequestHandler<UpdateRegionCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public UpdateRegionCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(UpdateRegionCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.Regions.FirstOrDefaultAsync (x => x.Id == request.RegionModel.Id, cancellationToken);
                if (entity == null)
                {
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, _localizer["Region"]));
                }
                _mapper.Map (request.RegionModel, entity);

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                if (success)
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["Region"]));
                else
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Region"]));
            }
        }
    }
}
