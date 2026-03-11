using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.WorkingTime.Commands
{
    public class UpdateWorkingTimeCommand : IRequest<Result>
    {
        public required WorkingTimeModel WorkingTimeModel { get; set; }

        public class UpdateWorkingTimeCommandHandler : IRequestHandler<UpdateWorkingTimeCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public UpdateWorkingTimeCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(UpdateWorkingTimeCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.WorkingTimes.FirstOrDefaultAsync (x => x.Id == request.WorkingTimeModel.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, "WorkingTime"));

                _mapper.Map (request.WorkingTimeModel, entity);

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                if (success)
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["WorkingTime"]));
                else
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["WorkingTime"]));
            }
        }
    }
}
