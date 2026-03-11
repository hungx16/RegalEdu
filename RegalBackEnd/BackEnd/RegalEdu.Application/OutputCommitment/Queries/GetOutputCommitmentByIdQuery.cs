using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.OutputCommitment.Queries
{
    public class GetOutputCommitmentByIdQuery : IRequest<Result<OutputCommitmentModel>>
    {
        public required string Id { get; set; }

        public class Handler : IRequestHandler<GetOutputCommitmentByIdQuery, Result<OutputCommitmentModel>>
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

            public async Task<Result<OutputCommitmentModel>> Handle(GetOutputCommitmentByIdQuery request, CancellationToken cancellationToken)
            {
                if (!Guid.TryParse (request.Id, out var guid))
                    return Result<OutputCommitmentModel>.Failure (_localizer.Format (LocalizationKey.InvalidIdFormat, _localizer["OutputCommitment"], request.Id));

                var entity = await _context.OutputCommitments
                    .Include (x => x.Student)
                    .AsNoTracking ( )
                    .FirstOrDefaultAsync (x => x.Id == guid && !x.IsDeleted, cancellationToken);

                if (entity == null)
                    return Result<OutputCommitmentModel>.Failure (_localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer["OutputCommitment"], request.Id));

                var model = _mapper.Map<OutputCommitmentModel> (entity);
                return Result<OutputCommitmentModel>.Success (model);
            }
        }
    }
}
