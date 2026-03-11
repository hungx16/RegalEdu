using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CouponIssue.Queries
{
    public class GetAllCouponIssuesQuery : IRequest<Result<List<CouponIssueModel>>> { }

    public class GetAllCouponIssuesQueryHandler : IRequestHandler<GetAllCouponIssuesQuery, Result<List<CouponIssueModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllCouponIssuesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<CouponIssueModel>>> Handle(GetAllCouponIssuesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _context.CouponIssues
                .AsNoTracking()
                .Include(x => x.CouponType)
                .Include(x => x.Coupons)
                .Include(x => x.CouponIssueStudent)
                .ToListAsync(cancellationToken);

            return Result<List<CouponIssueModel>>.Success(_mapper.Map<List<CouponIssueModel>>(entities));
        }
    }
}
