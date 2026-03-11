using MediatR;
using RegalEdu.Application.Common.Results;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.RegisterStudy.Commands
{
    public class DeleteListRegisterStudyCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    public class DeleteListRegisterStudyCommandHandler : IRequestHandler<DeleteListRegisterStudyCommand, Result>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDelete;
        private readonly ILogger<DeleteListRegisterStudyCommandHandler> _logger;

        public DeleteListRegisterStudyCommandHandler(
            IRegalEducationDbContext db,
            ILocalizationService localizer,
            ISoftDeleteService softDelete,
            ILogger<DeleteListRegisterStudyCommandHandler> logger)
        {
            _db = db; _localizer = localizer; _softDelete = softDelete; _logger = logger;
        }

        public async Task<Result> Handle(DeleteListRegisterStudyCommand request, CancellationToken ct)
        {
            if (request.ListIds == null || !request.ListIds.Any())
                return Result.Failure(_localizer.Format(LocalizationKey.NoModelToDelete, EntityName.RegisterStudy));

            int ok = 0, fail = 0;
            var notes = new List<string>();

            foreach (var id in request.ListIds)
            {
                var entity = _db.RegisterStudys.FirstOrDefault(x => x.Id.ToString() == id);
                if (entity == null)
                {
                    fail++;
                    notes.Add(_localizer.Format(LocalizationKey.EntityWithIdNotFound, EntityName.RegisterStudy, id));
                    continue;
                }

                // Dùng recursive soft delete để xóa kèm các bảng con nếu hệ thống đã cấu hình
                var res = await _softDelete.RecursiveSoftDelete(entity.Id, typeof(Domain.Entities.RegisterStudy));
                if (res.Succeeded) ok++;
                else { fail++; notes.Add(_localizer.Format(LocalizationKey.EntityDeleteFailed, EntityName.RegisterStudy, id, res.Errors)); }
            }

            var msg = _localizer.Format(LocalizationKey.MSG_DELETE_RESULT, EntityName.RegisterStudy, ok, fail);
            if (notes.Any()) msg += " " + string.Join(" ", notes);
            return ok > 0 ? Result.Success(msg) : Result.Failure(msg);
        }
    }
}
