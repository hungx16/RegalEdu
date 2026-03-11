using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RecruitmentApply.Queries
{
    public class GetDeletedRecruitmentAppliesQuery : IRequest<Result<List<RecruitmentApplyModel>>> { }

    public class GetDeletedRecruitmentAppliesQueryHandler : IRequestHandler<GetDeletedRecruitmentAppliesQuery, Result<List<RecruitmentApplyModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetDeletedRecruitmentAppliesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<RecruitmentApplyModel>>> Handle(GetDeletedRecruitmentAppliesQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.RecruitmentApplies
                .IgnoreQueryFilters ( )
                .Where (x => x.IsDeleted)
                .Include (x => x.RecruitmentInfo)
                .Include (x => x.Attachment)
                .AsNoTracking ( )
                .ToListAsync (cancellationToken);

            var data = _mapper.Map<List<RecruitmentApplyModel>> (list);
            return Result<List<RecruitmentApplyModel>>.Success (data);
        }
    }
}
