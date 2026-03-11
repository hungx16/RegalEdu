using MediatR;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;
using AutoMapper;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.Holiday.Commands
{
    public class UpdateHolidayCommand : IRequest<Result>
    {
        public required HolidayModel HolidayModel { get; set; }

        public class UpdateHolidayCommandHandler : IRequestHandler<UpdateHolidayCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public UpdateHolidayCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(UpdateHolidayCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.Holidays.FirstOrDefaultAsync (x => x.Id == request.HolidayModel.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, "Holiday"));

                _mapper.Map (request.HolidayModel, entity);

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                if (success)
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["Holiday"]));
                else
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Holiday"]));
            }
        }
    }
}
