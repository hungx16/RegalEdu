using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RecruitmentApply.Queries
{
    public class GetAllRecruitmentAppliesQuery : IRequest<Result<List<RecruitmentApplyModel>>> { }

    public class GetAllRecruitmentAppliesQueryHandler : IRequestHandler<GetAllRecruitmentAppliesQuery, Result<List<RecruitmentApplyModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllRecruitmentAppliesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<RecruitmentApplyModel>>> Handle(GetAllRecruitmentAppliesQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.RecruitmentApplies
                .Include (x => x.RecruitmentInfo)
                .Include (x => x.Attachment)
                .AsNoTracking ( )
                .ToListAsync (cancellationToken);

            var data = _mapper.Map<List<RecruitmentApplyModel>> (list);
            return Result<List<RecruitmentApplyModel>>.Success (data);
        }
    }
}
