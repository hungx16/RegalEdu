using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Promotion.Queries
{
    public class GetGlobalPromotionQuery : IRequest<Result<List<PromotionModel>>> { }

    public class GetGlobalPromotionQueryHandler : IRequestHandler<GetGlobalPromotionQuery, Result<List<PromotionModel>>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;

        public GetGlobalPromotionQueryHandler(IRegalEducationDbContext db, IMapper mapper)
        {
            _db = db; _mapper = mapper;
        }

        public async Task<Result<List<PromotionModel>>> Handle(GetGlobalPromotionQuery request, CancellationToken ct)
        {
            DateTime now = DateTime.UtcNow;
            var data = await _db.Promotions.AsNoTracking()
                .Include(x => x.Discounts)!.ThenInclude(d => d.DiscountDetails)
                .Include(x => x.PromotionFixedPrice)
                .Include(x => x.PromotionGift)!.ThenInclude(g => g.PromotionGiftDetails)
                .Include(x => x.PromotionCoupon)
                .Where(p => p.AllCompany == true && p.AllCourse == true && p.AllStudent == true && p.Status==Domain.Enums.StatusType.Active && p.StartDate<=now && p.EndDate>=now)
                .ToListAsync(ct);

            return Result<List<PromotionModel>>.Success(_mapper.Map<List<PromotionModel>>(data));
        }
    }
}
