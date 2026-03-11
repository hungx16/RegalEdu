using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PromotionGroup.Queries
{
    public class GetPromotionGroupByIdQuery : IRequest<Result<PromotionGroupModel>>
    {
        public required string Id { get; set; }
    }

    public class GetPromotionGroupByIdQueryHandler : IRequestHandler<GetPromotionGroupByIdQuery, Result<PromotionGroupModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetPromotionGroupByIdQueryHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<PromotionGroupModel>> Handle(GetPromotionGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.PromotionGroup
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.ToString() == request.Id && !x.IsDeleted, cancellationToken);

            if (entity == null)
                return Result<PromotionGroupModel>.Failure(
                    _localizer.Format(LocalizationKey.EntityWithIdNotFound, EntityName.PromotionGroup, request.Id)
                );

            return Result<PromotionGroupModel>.Success(_mapper.Map<PromotionGroupModel>(entity));
        }
    }
}