using MediatR;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;
using AutoMapper;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Degree.Commands
{
    public class AddDegreeCommand : IRequest<Result>
    {
        public required DegreeModel DegreeModel { get; set; }

        public class AddDegreeCommandHandler : IRequestHandler<AddDegreeCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public AddDegreeCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(AddDegreeCommand request, CancellationToken cancellationToken)
            {
                var entity = _mapper.Map<RegalEdu.Domain.Entities.Degree> (request.DegreeModel);
                await _context.Degrees.AddAsync (entity, cancellationToken);

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                if (success)
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, _localizer["Degree"]));
                else
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Degree"]));
            }
        }
    }
}
