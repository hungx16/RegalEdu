using AutoMapper;
using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.Reward.Queries
{
    public class GetRewardByIdQuery : IRequest<Result<RewardDto>>
    {
        public required Guid Id { get; set; }
    }

    public class GetRewardByIdQueryHandler : IRequestHandler<GetRewardByIdQuery, Result<RewardDto>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetRewardByIdQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<RewardDto>> Handle(GetRewardByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.SetEntity<RegalEdu.Domain.Entities.Reward>().FindAsync(new object[] { request.Id });
            if (entity == null) return Result<RewardDto>.Failure("Not found");
            return Result<RewardDto>.Success(_mapper.Map<RewardDto>(entity));
        }
    }
}
