using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Promotion.Commands
{
    public class AddPromotionCommand : IRequest<Result>
    {
        public required PromotionModel PromotionModel { get; set; }
    }

    public class AddPromotionCommandHandler : IRequestHandler<AddPromotionCommand, Result>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public AddPromotionCommandHandler(IRegalEducationDbContext db, IMapper mapper, ILocalizationService localizer)
        {
            _db = db;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result> Handle(AddPromotionCommand request, CancellationToken ct)
        {
            var m = request.PromotionModel;
            var entity = _mapper.Map<Domain.Entities.Promotion>(m);

            // Map collections nếu có
            if (m.Discounts != null)
                entity.Discounts = _mapper.Map<List<Domain.Entities.Discount>>(m.Discounts);

            if (m.PromotionFixedPrice != null)
                entity.PromotionFixedPrice = _mapper.Map<List<Domain.Entities.PromotionFixedPrice>>(m.PromotionFixedPrice);

            if (m.PromotionGift != null)
                entity.PromotionGift = _mapper.Map<List<Domain.Entities.PromotionGift>>(m.PromotionGift);

            if (m.PromotionCoupon != null)
                entity.PromotionCoupon = _mapper.Map<List<Domain.Entities.PromotionCoupon>>(m.PromotionCoupon);

            await _db.Promotions.AddAsync(entity, ct);
            var ok = await _db.SaveChangesAsync(ct) > 0;

            return ok
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, EntityName.Promotion))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Promotion));
        }
    }
}

