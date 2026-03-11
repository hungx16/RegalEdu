using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CouponIssue.Commands
{
    public class AddCouponIssueCommand : IRequest<Result>
    {
        public required CouponIssueModel CouponIssueModel { get; set; }
    }

    public class AddCouponIssueCommandHandler : IRequestHandler<AddCouponIssueCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly ILogger<AddCouponIssueCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly IRegalEducationDbContext _db;

        public AddCouponIssueCommandHandler(
            IRegalEducationDbContext context,
            ILogger<AddCouponIssueCommandHandler> logger,
            IMapper mapper,
            ILocalizationService localizer,
            IRegalEducationDbContext db)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
            _db = db;
        }

        public async Task<Result> Handle(AddCouponIssueCommand request, CancellationToken cancellationToken)
        {
            if (_db is not DbContext dbContext)
                throw new InvalidOperationException(_localizer[LocalizationKey.InvalidDbContextInstance]);
            var m = request.CouponIssueModel;
            var entity = _mapper.Map<Domain.Entities.CouponIssue>(request.CouponIssueModel);
            var couponType = await _context.CouponType
                .FirstOrDefaultAsync(ct => ct.Id == m.CouponTypeId, cancellationToken);
            if (m.IssueType==IssueType.Quantity && entity.Quantity > 0) {
                for (int i = 0; i < entity.Quantity; i++)
                {
                    var coupon = new Coupon
                    {
                        Id = Guid.NewGuid(),
                        Code = await AutoCodeHelper.GenerateCodeAsync(
                            new AutoCodeInfo
                            {

                                TableName = "Coupon",
                                ColumnName = "Code",
                                Prefix = couponType.Prefix,
                                Suffix = couponType.Suffix,
                                Length = (int)couponType.CharacterCount,
                                OrderNumber = i,
                            },
                            dbContext
                        ),
                        CouponIssueId = entity.Id,
                        CreatedDate = (DateTime)m.IssueDate,
                        ExpiredDate = couponType.DueType == DueType.duration && couponType.DurationInDays.HasValue && m.IssueDate.HasValue
                            ? m.IssueDate.Value.AddDays(couponType.DurationInDays.Value)
                            : couponType.EndDate,
                        CouponStatus = CouponStatus.NotUsed,

                    };
                    if (entity.Coupons == null)
                    {
                        entity.Coupons = new List<Coupon>();
                    }
                    entity.Coupons.Add(coupon);
                }
            }
            if(entity.IssueType == IssueType.SelectedStudent && (m.CouponIssueStudent!=null) && (m.CouponIssueStudent.Any()))
            {
                //
                int i = 0;
                foreach (var student in m.CouponIssueStudent)
                {
                    var coupon = new Coupon
                    {
                        Id = Guid.NewGuid(),
                        Code = await AutoCodeHelper.GenerateCodeAsync(
                            new AutoCodeInfo
                            {

                                TableName = "Coupon",
                                ColumnName = "Code",
                                Prefix = couponType.Prefix,
                                Suffix = couponType.Suffix,
                                Length = (int)couponType.CharacterCount,
                                OrderNumber = i,
                            },
                            dbContext
                        ),
                        CouponIssueId = entity.Id,
                        CreatedDate = (DateTime)m.IssueDate,
                        ExpiredDate = couponType.DueType == DueType.duration && couponType.DurationInDays.HasValue && m.IssueDate.HasValue
                            ? m.IssueDate.Value.AddDays(couponType.DurationInDays.Value)
                            : couponType.EndDate,
                        CouponStatus = CouponStatus.NotUsed,
                        StudentId = student.StudentId,

                    };
                    if (entity.Coupons == null)
                    {
                        entity.Coupons = new List<Coupon>();
                    }
                    entity.Coupons.Add(coupon);
                    i++;
                }
            }
            await _context.CouponIssues.AddAsync(entity, cancellationToken);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            return success
                ? Result.Success(_localizer.Format(LocalizationKey.MSG_CREATE_SUCCESS, EntityName.CouponIssue))
                : Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.CouponIssue));
        }
    }

}
