using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Gift.Queries
{
    public class GetGiftByIdQuery : IRequest<Result<GiftModel>>
    {
        public required string Id { get; set; }
    }

    public class GetGiftByIdQueryHandler : IRequestHandler<GetGiftByIdQuery, Result<GiftModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetGiftByIdQueryHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<GiftModel>> Handle(GetGiftByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Gift
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.ToString() == request.Id && !x.IsDeleted, cancellationToken);

            if (entity == null)
                return Result<GiftModel>.Failure(
                    _localizer.Format(LocalizationKey.EntityWithIdNotFound, EntityName.Gift, request.Id)
                );

            return Result<GiftModel>.Success(_mapper.Map<GiftModel>(entity));
        }
    }
}