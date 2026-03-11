using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Promotion.Queries
{
    public class GetAllPromotionsQuery : IRequest<Result<List<PromotionModel>>> { }

    public class GetAllPromotionsQueryHandler : IRequestHandler<GetAllPromotionsQuery, Result<List<PromotionModel>>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;

        public GetAllPromotionsQueryHandler(IRegalEducationDbContext db, IMapper mapper)
        {
            _db = db; _mapper = mapper;
        }

        public async Task<Result<List<PromotionModel>>> Handle(GetAllPromotionsQuery request, CancellationToken ct)
        {
            var data = await _db.Promotions.AsNoTracking()
                .Include(x => x.Discounts)!.ThenInclude(d => d.DiscountDetails)
                .Include(x => x.PromotionFixedPrice)
                .Include(x => x.PromotionGift)!.ThenInclude(g => g.PromotionGiftDetails)
                .Include(x => x.PromotionCoupon)
                .ToListAsync(ct);

            return Result<List<PromotionModel>>.Success(_mapper.Map<List<PromotionModel>>(data));
        }
    }
}
