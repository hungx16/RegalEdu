using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CouponType.Commands
{
    public class AddCouponTypeCommand : IRequest<Result>
    {
        public required CouponTypeModel CouponTypeModel { get; set; }
    }

    public class AddCouponTypeCommandHandler : IRequestHandler<AddCouponTypeCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<AddCouponTypeCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public AddCouponTypeCommandHandler(
            IRegalEducationDbContext context,
            ILogger<AddCouponTypeCommandHandler> logger,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result> Handle(AddCouponTypeCommand request, CancellationToken cancellationToken)
        {
            var m = request.CouponTypeModel;
            var entity = _mapper.Map<Domain.Entities.CouponType>(request.CouponTypeModel);
            if (m.CouponTypeDiscounts != null)
                entity.CouponTypeDiscounts = _mapper.Map<List<Domain.Entities.CouponTypeDiscount>>(m.CouponTypeDiscounts);
            //await _context.CouponType.AddAsync(entity, cancellationToken);
            // 4. Xử lý các thực thể con tùy thuộc vào loại khuyến mại (Type)
            //if (m.CouponTypeDiscounts != null && m.CouponTypeDiscounts.Any())
            //        {
            //            foreach (var discountModel in m.CouponTypeDiscounts)
            //            {
            //                var discount = new CouponTypeDiscount
            //                {
            //                    Method = discountModel.Method,
            //                    DiscountMax = discountModel.DiscountMax,
            //                    CouponTypeId = entity.Id,
            //                    CouponTypeDiscountDetail = discountModel.CouponTypeDiscountDetails?.Select(d => new CouponTypeDiscountDetail
            //                    {
            //                        MinAmount = d.MinAmount,
            //                        Limit = d.Limit,
            //                        DiscountType = d.DiscountType,
            //                        DiscountAmount = d.DiscountAmount,
            //                    }).ToList()
            //                };
            //                await _context.CouponTypeDiscount.AddAsync(discount, cancellationToken);
            //                //if (discount.CouponTypeDiscountDetail != null)
            //                //{
            //                //    foreach (var item in discount.CouponTypeDiscountDetail)
            //                //    {
            //                //        item.CouponTypeDiscountId = discount.Id;
            //                //        await _context.CouponTypeDiscountDetail.AddAsync(item, cancellationToken);
            //                //    }
            //                //}
            //            }
            //        }
            //if (m.CouponTypeGifts != null && m.CouponTypeGifts.Any())
            //{
            //    foreach (var coupontypeGiftModel in m.CouponTypeGifts)
            //    {
            //        var coupontypeGift = new CouponTypeGift
            //        {
            //            GiftType = coupontypeGiftModel.GiftType,
            //            GiftCount = coupontypeGiftModel.GiftCount,
            //            CouponTypeId = coupontypeGiftModel.CouponTypeId,
            //            CouponTypeGiftDetail = coupontypeGiftModel.CouponTypeGiftDetail?.Select(c => new CouponTypeGiftDetail
            //            {
            //                GiftName = c.GiftName,
            //                QuantityGift = c.QuantityGift
            //            }).ToList()

            //        };

            //        await _context.CouponTypeGift.AddAsync(coupontypeGift, cancellationToken);
            //    }
            //}
            //if (m.CouponTypeCoupons != null && m.CouponTypeCoupons.Any())
            //{
            //    foreach (var couponTypeCouponModel in m.CouponTypeCoupons)
            //    {
            //        var coupon = new CouponTypeCoupon
            //        {
            //            MinQuantity = couponTypeCouponModel.MinQuantity,
            //            Limit = couponTypeCouponModel.Limit,
            //            CouponCode = couponTypeCouponModel.CouponCode,
            //            CouponTypeID = entity.Id,
            //        };
            //        await _context.CouponTypeCoupon.AddAsync(coupon, cancellationToken);
            //    }
            //}

            //if (m.CouponTypeFixedPrices != null && m.CouponTypeFixedPrices.Any())
            //{
            //    foreach (var fixedPriceModel in m.CouponTypeFixedPrices)
            //    {
            //        var fixedPrice = new CouponTypeFixedPrice
            //        {
            //            MinPrice = fixedPriceModel.MinPrice,
            //            Limit = fixedPriceModel.Limit,
            //            PriceSale = fixedPriceModel.PriceSale,
            //            CouponTypeId = entity.Id,
            //        };
            //        await _context.CouponTypeFixedPrice.AddAsync(fixedPrice, cancellationToken);
            //    }
            //}
            await _context.CouponType.AddAsync(entity, cancellationToken);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            return success
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, EntityName.CouponType))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.CouponType));
        }
    }

}
