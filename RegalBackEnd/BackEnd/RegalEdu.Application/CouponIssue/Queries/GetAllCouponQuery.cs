using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.CouponIssue.Queries
{
    public class GetAllCouponQuery : IRequest<Result<List<CouponModel>>> { }

    public class GetAllCouponQueryHandler : IRequestHandler<GetAllCouponQuery, Result<List<CouponModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllCouponQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<CouponModel>>> Handle(GetAllCouponQuery request, CancellationToken cancellationToken)
        {
            var entities = await _context.Coupons
                .Include(s=>s.CouponIssue).ThenInclude(s=>s.CouponType)
                .AsNoTracking()
                
                .ToListAsync(cancellationToken);

            return Result<List<CouponModel>>.Success(_mapper.Map<List<CouponModel>>(entities));
        }
    }
}
