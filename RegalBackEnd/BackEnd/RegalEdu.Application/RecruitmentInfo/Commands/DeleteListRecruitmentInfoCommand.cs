using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.RecruitmentInfo.Commands
{
    public class DeleteListRecruitmentInfoCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }

        public class Handler : IRequestHandler<DeleteListRecruitmentInfoCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly ILocalizationService _localizer;

            public Handler(IRegalEducationDbContext context, ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(DeleteListRecruitmentInfoCommand request, CancellationToken cancellationToken)
            {
                if (request.ListIds == null || request.ListIds.Count == 0)
                    return Result.Failure (_localizer.Format (LocalizationKey.NoModelToDelete, _localizer["RecruitmentInfo"]));

                var ids = request.ListIds
                    .Select (id => Guid.TryParse (id, out var g) ? g : Guid.Empty)
                    .Where (g => g != Guid.Empty)
                    .ToList ( );

                var entities = await _context.RecruitmentInfos.Where (x => ids.Contains (x.Id)).ToListAsync (cancellationToken);
                if (!entities.Any ( ))
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, _localizer["RecruitmentInfo"]));

                foreach (var e in entities)
                {
                    e.IsDeleted = true;
                }

                var success = await _context.SaveChangesAsync (cancellationToken) > 0;
                return success
                    ? Result.Success (_localizer.Format (LocalizationKey.MSG_DELETE_SUCCESS, _localizer["RecruitmentInfo"]))
                    : Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["RecruitmentInfo"]));
            }
        }
    }
}
