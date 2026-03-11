using MediatR;
using AutoMapper;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.OutputCommitment.Commands
{
    public class AddOutputCommitmentCommand : IRequest<Result>
    {
        public required OutputCommitmentModel OutputCommitmentModel { get; set; }

        public class Handler : IRequestHandler<AddOutputCommitmentCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public Handler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(AddOutputCommitmentCommand request, CancellationToken cancellationToken)
            {
                var entity = _mapper.Map<RegalEdu.Domain.Entities.OutputCommitment> (request.OutputCommitmentModel);
                await _context.OutputCommitments.AddAsync (entity, cancellationToken);

                var ok = await _context.SaveChangesAsync (cancellationToken) > 0;
                return ok
                    ? Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, _localizer["OutputCommitment"]))
                    : Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["OutputCommitment"]));
            }
        }
    }
}
