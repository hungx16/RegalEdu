using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Promotion.Commands
{
    public class UpdatePromotionCommand : IRequest<Result>
    {
        public required PromotionModel PromotionModel { get; set; }
    }

    public class UpdatePromotionCommandHandler : IRequestHandler<UpdatePromotionCommand, Result>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdatePromotionCommandHandler(IRegalEducationDbContext db, IMapper mapper, ILocalizationService localizer)
        {
            _db = db;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result> Handle(UpdatePromotionCommand request, CancellationToken ct)
        {
            var m = request.PromotionModel;

            var entity = await _db.Promotions
                .Include(x => x.Discounts)!.ThenInclude(d => d.DiscountDetails)
                .Include(x => x.PromotionFixedPrice)
                .Include(x => x.PromotionGift)!.ThenInclude(g => g.PromotionGiftDetails)
                .Include(x => x.PromotionCoupon)
                .FirstOrDefaultAsync(x => x.Id == m.Id, ct);

            if (entity == null)
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, EntityName.Promotion));

            // Map scalar
            _mapper.Map(m, entity);

            // --- Sync Discounts (+Details)
            if (m.Discounts != null)
            {
                entity.Discounts ??= new List<Domain.Entities.Discount>();
                SyncCollection(entity.Discounts, m.Discounts,
                    e => e.Id, x => x.Id,
                    (e, x) => _mapper.Map(x, e),
                    x => _mapper.Map<Domain.Entities.Discount>(x));

                foreach (var d in entity.Discounts)
                {
                    var md = m.Discounts.FirstOrDefault(z => z.Id == d.Id);
                    var mdDetails = md?.DiscountDetails ?? new List<DiscountDetailModel>();
                    d.DiscountDetails ??= new List<Domain.Entities.DiscountDetail>();

                    SyncCollection(d.DiscountDetails, mdDetails,
                        e => e.Id, x => x.Id,
                        (e, x) => _mapper.Map(x, e),
                        x => _mapper.Map<Domain.Entities.DiscountDetail>(x));
                }
            }

            // --- Sync FixedPrice
            if (m.PromotionFixedPrice != null)
            {
                entity.PromotionFixedPrice ??= new List<Domain.Entities.PromotionFixedPrice>();
                SyncCollection(entity.PromotionFixedPrice, m.PromotionFixedPrice,
                    e => e.Id, x => x.Id,
                    (e, x) => _mapper.Map(x, e),
                    x => _mapper.Map<Domain.Entities.PromotionFixedPrice>(x));
            }

            // --- Sync Gifts (+Details)
            if (m.PromotionGift != null)
            {
                entity.PromotionGift ??= new List<Domain.Entities.PromotionGift>();
                SyncCollection(entity.PromotionGift, m.PromotionGift,
                    e => e.Id, x => x.Id,
                    (e, x) => _mapper.Map(x, e),
                    x => _mapper.Map<Domain.Entities.PromotionGift>(x));

                foreach (var g in entity.PromotionGift)
                {
                    var mg = m.PromotionGift.FirstOrDefault(z => z.Id == g.Id);
                    var mgDetails = mg?.PromotionGiftDetails ?? new List<PromotionGiftDetailModel>();
                    g.PromotionGiftDetails ??= new List<Domain.Entities.PromotionGiftDetail>();

                    SyncCollection(g.PromotionGiftDetails, mgDetails,
                        e => e.Id, x => x.Id,
                        (e, x) => _mapper.Map(x, e),
                        x => _mapper.Map<Domain.Entities.PromotionGiftDetail>(x));
                }
            }

            // --- Sync Coupons
            if (m.PromotionCoupon != null)
            {
                entity.PromotionCoupon ??= new List<Domain.Entities.PromotionCoupon>();
                SyncCollection(entity.PromotionCoupon, m.PromotionCoupon,
                    e => e.Id, x => x.Id,
                    (e, x) => _mapper.Map(x, e),
                    x => _mapper.Map<Domain.Entities.PromotionCoupon>(x));
            }

            var ok = await _db.SaveChangesAsync(ct) > 0;

            return ok
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.Promotion))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Promotion));
        }

        // Helper sync add/update/remove theo Id
        private static void SyncCollection<TEntity, TModel>(
            ICollection<TEntity> existing,
            IEnumerable<TModel> incoming,
            Func<TEntity, Guid> keyE,
            Func<TModel, Guid?> keyM,
            Action<TEntity, TModel> updater,
            Func<TModel, TEntity> creator) where TEntity : class
        {
            var removeList = existing.Where(e => !incoming.Any(m => keyM(m).HasValue && keyM(m)!.Value == keyE(e))).ToList();
            foreach (var r in removeList) existing.Remove(r);

            foreach (var m in incoming)
            {
                var k = keyM(m);
                var found = k.HasValue ? existing.FirstOrDefault(e => keyE(e) == k.Value) : null;
                if (found != null) updater(found, m);
                else existing.Add(creator(m));
            }
        }
    }
}
