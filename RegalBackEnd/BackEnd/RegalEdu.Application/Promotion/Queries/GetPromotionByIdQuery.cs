using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Promotion.Queries
{
    public class GetPromotionByIdQuery : IRequest<Result<PromotionModel>>
    {
        public required string Id { get; set; }
    }

    public class GetPromotionByIdQueryHandler : IRequestHandler<GetPromotionByIdQuery, Result<PromotionModel>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetPromotionByIdQueryHandler(IRegalEducationDbContext db, IMapper mapper, ILocalizationService localizer)
        {
            _db = db; _mapper = mapper; _localizer = localizer;
        }

        public async Task<Result<PromotionModel>> Handle(GetPromotionByIdQuery request, CancellationToken ct)
        {
            var entity = await _db.Promotions.AsNoTracking()
                .Include(x => x.Discounts)!.ThenInclude(d => d.DiscountDetails)
                .Include(x => x.PromotionFixedPrice)
                .Include(x => x.PromotionGift)!.ThenInclude(g => g.PromotionGiftDetails)
                .Include(x => x.PromotionCoupon)
                .FirstOrDefaultAsync(x => x.Id.ToString() == request.Id && !x.IsDeleted, ct);

            if (entity == null)
                return Result<PromotionModel>.Failure(_localizer.Format(LocalizationKey.EntityWithIdNotFound, EntityName.Promotion, request.Id));

            return Result<PromotionModel>.Success(_mapper.Map<PromotionModel>(entity));
        }
    }
}
