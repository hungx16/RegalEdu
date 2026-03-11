using AutoMapper;
using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.CustomerReward.Queries
{
    public class GetCustomerRewardByIdQuery : IRequest<Result<CustomerRewardDto>>
    {
        public required Guid Id { get; set; }
    }

    public class GetCustomerRewardByIdQueryHandler : IRequestHandler<GetCustomerRewardByIdQuery, Result<CustomerRewardDto>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetCustomerRewardByIdQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<CustomerRewardDto>> Handle(GetCustomerRewardByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.SetEntity<RegalEdu.Domain.Entities.CustomerReward>().FindAsync(new object[] { request.Id });
            if (entity == null) return Result<CustomerRewardDto>.Failure("Not found");
            return Result<CustomerRewardDto>.Success(_mapper.Map<CustomerRewardDto>(entity));
        }
    }
}
