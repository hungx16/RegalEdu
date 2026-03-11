using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Promotion.Commands
{
    public class DeleteListPromotionCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    public class DeleteListPromotionCommandHandler : IRequestHandler<DeleteListPromotionCommand, Result>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDelete;

        public DeleteListPromotionCommandHandler(IRegalEducationDbContext db, ILocalizationService localizer, ISoftDeleteService softDelete)
        {
            _db = db; _localizer = localizer; _softDelete = softDelete;
        }

        public async Task<Result> Handle(DeleteListPromotionCommand request, CancellationToken ct)
        {
            if (request.ListIds == null || !request.ListIds.Any())
                return Result.Failure(_localizer.Format(LocalizationKey.NoModelToDelete, EntityName.Promotion));

            int ok = 0, fail = 0;
            var notes = new List<string>();

            foreach (var id in request.ListIds)
            {
                var entity = _db.Promotions.FirstOrDefault(x => x.Id.ToString() == id);
                if (entity == null)
                {
                    fail++;
                    notes.Add(_localizer.Format(LocalizationKey.EntityWithIdNotFound, EntityName.Promotion, id));
                    continue;
                }

                var res = await _softDelete.RecursiveSoftDelete(entity.Id, typeof(Domain.Entities.Promotion));
                if (res.Succeeded) ok++;
                else { fail++; notes.Add(_localizer.Format(LocalizationKey.EntityDeleteFailed, EntityName.Promotion, id, res.Errors)); }
            }

            var msg = _localizer.Format(LocalizationKey.MSG_DELETE_RESULT, EntityName.Promotion, ok, fail);
            if (notes.Any()) msg += " " + string.Join(" ", notes);
            return ok > 0 ? Result.Success(msg) : Result.Failure(msg);
        }
    }
}
