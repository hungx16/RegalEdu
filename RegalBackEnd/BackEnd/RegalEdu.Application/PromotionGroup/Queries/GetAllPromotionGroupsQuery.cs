using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PromotionGroup.Queries
{
    public class GetAllPromotionGroupsQuery : IRequest<Result<List<PromotionGroupModel>>> { }

    public class GetAllPromotionGroupsQueryHandler : IRequestHandler<GetAllPromotionGroupsQuery, Result<List<PromotionGroupModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllPromotionGroupsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<PromotionGroupModel>>> Handle(GetAllPromotionGroupsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _context.PromotionGroup
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result<List<PromotionGroupModel>>.Success(
                _mapper.Map<List<PromotionGroupModel>>(entities)
            );
        }
    }
}
