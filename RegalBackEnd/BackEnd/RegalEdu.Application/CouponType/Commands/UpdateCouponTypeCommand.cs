using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Logging;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CouponType.Commands
{
    public class UpdateCouponTypeCommand : IRequest<Result>
    {
        public required CouponTypeModel CouponTypeModel { get; set; }
    }

    public class UpdateCouponTypeCommandHandler : IRequestHandler<UpdateCouponTypeCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<UpdateCouponTypeCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateCouponTypeCommandHandler(
            IRegalEducationDbContext context,
            ILogger<UpdateCouponTypeCommandHandler> logger,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result> Handle(UpdateCouponTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.CouponType
                .Include(x => x.CouponTypeDiscounts!)
                .ThenInclude(x => x.CouponTypeDiscountDetail)
                .FirstOrDefaultAsync(x => x.Id == request.CouponTypeModel.Id, cancellationToken);

            if (entity == null)
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, EntityName.CouponType));

            if (entity.CouponTypeDiscounts != null)
            {
                // Xóa tất cả CouponTypeDiscountDetail liên quan trong bộ nhớ
                foreach (var discount in entity.CouponTypeDiscounts)
                {
                    if (discount.CouponTypeDiscountDetail != null)
                    {
                        _context.CouponTypeDiscountDetail.RemoveRange(discount.CouponTypeDiscountDetail);
                    }
                }

                // Xóa tất cả CouponTypeDiscounts liên quan trong bộ nhớ
                _context.CouponTypeDiscount.RemoveRange(entity.CouponTypeDiscounts);
            }
            _mapper.Map(request.CouponTypeModel, entity);
           // _context.Update(entity);
            // entity.CompanyId = request.CouponTypeModel.CompanyId;
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            return success
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.CouponType))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.CouponType));
        }
    }
}
