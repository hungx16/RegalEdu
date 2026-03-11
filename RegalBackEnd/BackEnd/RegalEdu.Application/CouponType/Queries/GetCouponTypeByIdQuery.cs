using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CouponType.Queries
{
    public class GetCouponTypeByIdQuery : IRequest<Result<CouponTypeModel>>
    {
        public required string Id { get; set; }
    }

    public class GetCouponTypeByIdQueryHandler : IRequestHandler<GetCouponTypeByIdQuery, Result<CouponTypeModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetCouponTypeByIdQueryHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<CouponTypeModel>> Handle(GetCouponTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.CouponType
                .AsNoTracking()
                .Include(x => x.CouponTypeDiscounts)
                //.Include(x => x.CouponTypeFixedPrices)
                //.Include(x => x.CouponTypeGifts)
                //.Include(x => x.CouponTypeCoupons)
                .FirstOrDefaultAsync(x => x.Id.ToString() == request.Id && !x.IsDeleted, cancellationToken);

            if (entity == null)
                return Result<CouponTypeModel>.Failure(
                    _localizer.Format(LocalizationKey.EntityWithIdNotFound, EntityName.CouponType, request.Id));

            return Result<CouponTypeModel>.Success(_mapper.Map<CouponTypeModel>(entity));
        }
    }
}