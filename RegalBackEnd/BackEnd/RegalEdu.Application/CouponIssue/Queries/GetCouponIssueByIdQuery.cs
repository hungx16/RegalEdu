using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CouponIssue.Queries
{
    public class GetCouponIssueByIdQuery : IRequest<Result<CouponIssueModel>>
    {
        public required string Id { get; set; }
    }

    public class GetCouponIssueByIdQueryHandler : IRequestHandler<GetCouponIssueByIdQuery, Result<CouponIssueModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetCouponIssueByIdQueryHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<CouponIssueModel>> Handle(GetCouponIssueByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.CouponIssues
                .AsNoTracking()
                .Include(x => x.CouponType)
                .Include(x => x.Coupons)
                .Include(x => x.CouponIssueStudent)
                .FirstOrDefaultAsync(x => x.Id.ToString() == request.Id && !x.IsDeleted, cancellationToken);

            if (entity == null)
                return Result<CouponIssueModel>.Failure(
                    _localizer.Format(LocalizationKey.EntityWithIdNotFound, EntityName.CouponIssue, request.Id));

            return Result<CouponIssueModel>.Success(_mapper.Map<CouponIssueModel>(entity));
        }
    }
}