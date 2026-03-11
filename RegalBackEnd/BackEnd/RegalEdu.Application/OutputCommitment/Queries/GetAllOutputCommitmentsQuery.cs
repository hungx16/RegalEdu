using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.OutputCommitment.Queries
{
    public class GetAllOutputCommitmentsQuery : IRequest<Result<List<OutputCommitmentModel>>> { }

    public class GetAllOutputCommitmentsQueryHandler : IRequestHandler<GetAllOutputCommitmentsQuery, Result<List<OutputCommitmentModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllOutputCommitmentsQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<OutputCommitmentModel>>> Handle(GetAllOutputCommitmentsQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.OutputCommitments
                .Include (x => x.Student)
                .AsNoTracking ( )
                .ToListAsync (cancellationToken);

            var data = _mapper.Map<List<OutputCommitmentModel>> (list);
            return Result<List<OutputCommitmentModel>>.Success (data);
        }
    }
}
