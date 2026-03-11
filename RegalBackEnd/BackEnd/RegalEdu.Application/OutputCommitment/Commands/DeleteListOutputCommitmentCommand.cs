using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.OutputCommitment.Commands
{
    public class DeleteListOutputCommitmentCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }

        public class Handler : IRequestHandler<DeleteListOutputCommitmentCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly ILocalizationService _localizer;

            public Handler(IRegalEducationDbContext context, ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(DeleteListOutputCommitmentCommand request, CancellationToken cancellationToken)
            {
                if (request.ListIds == null || request.ListIds.Count == 0)
                    return Result.Failure (_localizer.Format (LocalizationKey.NoModelToDelete, _localizer["OutputCommitment"]));

                var ids = request.ListIds
                    .Select (id => Guid.TryParse (id, out var g) ? g : Guid.Empty)
                    .Where (g => g != Guid.Empty)
                    .ToList ( );

                var items = await _context.OutputCommitments.Where (x => ids.Contains (x.Id)).ToListAsync (cancellationToken);
                if (!items.Any ( ))
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, _localizer["OutputCommitment"]));

                foreach (var e in items)
                {
                    e.IsDeleted = true;
                }

                var ok = await _context.SaveChangesAsync (cancellationToken) > 0;
                return ok
                    ? Result.Success (_localizer.Format (LocalizationKey.MSG_DELETE_SUCCESS, _localizer["OutputCommitment"]))
                    : Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["OutputCommitment"]));
            }
        }
    }
}
