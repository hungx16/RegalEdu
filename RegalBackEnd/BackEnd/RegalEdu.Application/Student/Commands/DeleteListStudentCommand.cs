using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Student.Commands
{
    public class DeleteListStudentCommand : IRequest<Result>
    {
        public required List<string> ListIds { get; set; }
    }

    public class DeleteListStudentCommandHandler : IRequestHandler<DeleteListStudentCommand, Result>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly ILocalizationService _localizer;
        private readonly ISoftDeleteService _softDelete;

        public DeleteListStudentCommandHandler(IRegalEducationDbContext db, ILocalizationService localizer, ISoftDeleteService softDelete)
        {
            _db = db; _localizer = localizer; _softDelete = softDelete;
        }

        public async Task<Result> Handle(DeleteListStudentCommand request, CancellationToken ct)
        {
            if (request.ListIds == null || !request.ListIds.Any())
                return Result.Failure(_localizer.Format(LocalizationKey.NoModelToDelete, EntityName.Student));

            int ok = 0, fail = 0;
            var notes = new List<string>();

            foreach (var id in request.ListIds)
            {
                var entity = _db.Students.FirstOrDefault(x => x.Id.ToString() == id);
                if (entity == null)
                {
                    fail++;
                    notes.Add(_localizer.Format(LocalizationKey.EntityWithIdNotFound, EntityName.Student, id));
                    continue;
                }

                // recursive soft delete để xóa kèm dữ liệu con nếu đã cấu hình
                var res = await _softDelete.RecursiveSoftDelete(entity.Id, typeof(Domain.Entities.Student));
                if (res.Succeeded) ok++;
                else { fail++; notes.Add(_localizer.Format(LocalizationKey.EntityDeleteFailed, EntityName.Student, id, res.Errors)); }
            }

            var msg = _localizer.Format(LocalizationKey.MSG_DELETE_RESULT, EntityName.Student, ok, fail);
            if (notes.Any()) msg += " " + string.Join(" ", notes);
            return ok > 0 ? Result.Success(msg) : Result.Failure(msg);
        }
    }
}
