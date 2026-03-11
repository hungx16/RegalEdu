using MediatR;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;
using AutoMapper;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.WorkingTime.Commands
{
    public class AddWorkingTimeCommand : IRequest<Result>
    {
        public required WorkingTimeModel WorkingTimeModel { get; set; }

        public class AddWorkingTimeCommandHandler : IRequestHandler<AddWorkingTimeCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public AddWorkingTimeCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(AddWorkingTimeCommand request, CancellationToken cancellationToken)
            {
                var entity = _mapper.Map<RegalEdu.Domain.Entities.WorkingTime> (request.WorkingTimeModel);
                await _context.WorkingTimes.AddAsync (entity, cancellationToken);

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                if (success)
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, _localizer["WorkingTime"]));
                else
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["WorkingTime"]));
            }
        }
    }
}
