using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.OutputCommitment.Commands
{
    public class UpdateOutputCommitmentCommand : IRequest<Result>
    {
        public required OutputCommitmentModel OutputCommitmentModel { get; set; }

        public class Handler : IRequestHandler<UpdateOutputCommitmentCommand, Result>
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

            public async Task<Result> Handle(UpdateOutputCommitmentCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.OutputCommitments
                    .FirstOrDefaultAsync (x => x.Id == request.OutputCommitmentModel.Id, cancellationToken);

                if (entity == null)
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, _localizer["OutputCommitment"]));

                _mapper.Map (request.OutputCommitmentModel, entity);

                var ok = await _context.SaveChangesAsync (cancellationToken) > 0;
                return ok
                    ? Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["OutputCommitment"]))
                    : Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["OutputCommitment"]));
            }
        }
    }
}
