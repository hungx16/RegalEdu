using MediatR;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;
using AutoMapper;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace RegalEdu.Application.Degree.Commands
{
    public class UpdateDegreeCommand : IRequest<Result>
    {
        public required DegreeModel DegreeModel { get; set; }

        public class UpdateDegreeCommandHandler : IRequestHandler<UpdateDegreeCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public UpdateDegreeCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(UpdateDegreeCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.Degrees.FirstOrDefaultAsync (x => x.Id == request.DegreeModel.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, "Degree"));

                _mapper.Map (request.DegreeModel, entity);

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                if (success)
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["Degree"]));
                else
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Degree"]));
            }
        }
    }
}
