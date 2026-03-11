using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Region.Commands
{
    public class AddRegionCommand : IRequest<Result>
    {
        public required RegionModel RegionModel { get; set; }

        public class AddRegionCommandHandler : IRequestHandler<AddRegionCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public AddRegionCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(AddRegionCommand request, CancellationToken cancellationToken)
            {
                var info = AutoCodeConfig.Get (AutoCodeType.Region);
                // Ensure _context is not null and cast to DbContext
                if (_context is not DbContext dbContext)
                {
                    throw new InvalidOperationException (_localizer[LocalizationKey.InvalidDbContextInstance]);
                }

                var result = await AutoCodeHelper.CreateWithAutoCodeRetryAsync (
                    info,
                    async (code) =>
                    {
                        var region = _mapper.Map<RegalEdu.Domain.Entities.Region> (request.RegionModel);
                        region.RegionCode = code;

                        await _context.Regions.AddAsync (region, cancellationToken);
                        var success = await _context.SaveChangesAsync (cancellationToken) > 0;

                        if (success)
                        {
                            return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, EntityName.Region));
                        }
                        else
                        {
                            return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Region));
                        }
                    },
                    dbContext
                );
                return result;
            }

        }
    }
}
