using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CouponType.Queries
{
    public class GetAllCouponTypesQuery : IRequest<Result<List<CouponTypeModel>>> { }

    public class GetAllCouponTypesQueryHandler : IRequestHandler<GetAllCouponTypesQuery, Result<List<CouponTypeModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllCouponTypesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<CouponTypeModel>>> Handle(GetAllCouponTypesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _context.CouponType
                .AsNoTracking()
                .Include(x => x.CouponTypeDiscounts).ThenInclude(x=>x.CouponTypeDiscountDetail)
                //.Include(x => x.CouponTypeFixedPrices)
                //.Include(x => x.CouponTypeGifts)
                //.Include(x => x.CouponTypeCoupons)
                .ToListAsync(cancellationToken);

            return Result<List<CouponTypeModel>>.Success(_mapper.Map<List<CouponTypeModel>>(entities));
        }
    }
}
