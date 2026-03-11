using AutoMapper;
using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.WorkingTimeConfiguration.Commands
{
    public class AddWorkingTimeConfigurationCommand : IRequest<Result>
    {
        public required WorkingTimeConfigurationModel WorkingTimeConfigurationModel { get; set; }

        public class AddWorkingTimeConfigurationCommandHandler : IRequestHandler<AddWorkingTimeConfigurationCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public AddWorkingTimeConfigurationCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(AddWorkingTimeConfigurationCommand request, CancellationToken cancellationToken)
            {
                var entity = _mapper.Map<Domain.Entities.WorkingTimeConfiguration> (request.WorkingTimeConfigurationModel);
                await _context.WorkingTimeConfigurations.AddAsync (entity, cancellationToken); // Fixed the method call

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                if (success)
                    return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, _localizer["WorkingTimeConfiguration"]));
                else
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["WorkingTimeConfiguration"]));
            }
        }
    }
}
