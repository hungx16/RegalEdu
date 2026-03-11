using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RegisterStudy.Commands
{
    public class UpdateRegisterStudyCommand : IRequest<Result>
    {
        public required RegisterStudyModel RegisterStudyModel { get; set; }
    }

    public class UpdateRegisterStudyCommandHandler : IRequestHandler<UpdateRegisterStudyCommand, Result>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly ILogger<UpdateRegisterStudyCommandHandler> _logger;

        public UpdateRegisterStudyCommandHandler(
            IRegalEducationDbContext db,
            IMapper mapper,
            ILocalizationService localizer,
            ILogger<UpdateRegisterStudyCommandHandler> logger)
        {
            _db = db; _mapper = mapper; _localizer = localizer; _logger = logger;
        }

        public async Task<Result> Handle(UpdateRegisterStudyCommand request, CancellationToken ct)
        {
            var m = request.RegisterStudyModel;

            var entity = await _db.RegisterStudys
                .Include(x => x.DetailRegisterStudys)
                .FirstOrDefaultAsync(x => x.Id == m.Id, ct);

            if (entity == null)
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, EntityName.RegisterStudy));

            // Map scalar fields & FKs
            _mapper.Map(m, entity);

            // ---- Sync DetailRegisterStudys (1-n) ----
            entity.DetailRegisterStudys ??= new List<Domain.Entities.DetailRegisterStudy>();
            var incomingDetails = m.DetailRegisterStudys ?? new List<DetailRegisterStudyModel>();
            SyncCollection(
                entity.DetailRegisterStudys,
                incomingDetails,
                e => e.Id,
                md => md.Id,
                (e, md) => _mapper.Map(md, e),
                md => _mapper.Map<Domain.Entities.DetailRegisterStudy>(md)
            );

            // ---- Sync RegisterPromotionList (n - qua DbSet, không có collection trên entity gốc) ----
            // Load hiện trạng các RPL của RegisterStudy
            var existRpl = await _db.RegisterPromotionList
                .Where(r => r.RegisterStudyId == entity.Id && !r.IsDeleted)
                .ToListAsync(ct);

            var incomingRpl = m.RegisterPromotion ?? new List<RegisterPromotionListModel>();

            // remove
            foreach (var r in existRpl.Where(r => !incomingRpl.Any(i => i.Id.HasValue && i.Id == r.Id)).ToList())
            {
                _db.RegisterPromotionList.Remove(r);
            }
            // upsert
            foreach (var i in incomingRpl)
            {
                var found = i.Id.HasValue ? existRpl.FirstOrDefault(r => r.Id == i.Id) : null;
                if (found != null)
                {
                    _mapper.Map(i, found);
                }
                else
                {
                    var newR = _mapper.Map<Domain.Entities.RegisterPromotionList>(i);
                    newR.RegisterStudyId = entity.Id;
                    await _db.RegisterPromotionList.AddAsync(newR, ct);
                }
            }

            var saved = await _db.SaveChangesAsync(ct) > 0;

            return saved
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.RegisterStudy))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.RegisterStudy));
        }

        // Helper: sync a collection by Id (add/update/remove)
        private static void SyncCollection<TEntity, TModel>(
            ICollection<TEntity> existing,
            IEnumerable<TModel> incoming,
            Func<TEntity, Guid> entityKey,
            Func<TModel, Guid?> modelKey,
            Action<TEntity, TModel> applyUpdate,
            Func<TModel, TEntity> createNew) where TEntity : class
        {
            // remove
            var toRemove = existing.Where(e => !incoming.Any(m => modelKey(m).HasValue && modelKey(m)!.Value == entityKey(e))).ToList();
            foreach (var r in toRemove) existing.Remove(r);

            // upsert
            foreach (var m in incoming)
            {
                var mk = modelKey(m);
                var found = mk.HasValue ? existing.FirstOrDefault(e => entityKey(e) == mk.Value) : null;
                if (found != null) applyUpdate(found, m);
                else existing.Add(createNew(m));
            }
        }
    }
}
