using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RecruitmentInfo.Queries
{
    public class GetDeletedRecruitmentInfoQuery : IRequest<Result<List<RecruitmentInfoModel>>> { }

    public class GetDeletedRecruitmentInfoQueryHandler : IRequestHandler<GetDeletedRecruitmentInfoQuery, Result<List<RecruitmentInfoModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetDeletedRecruitmentInfoQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<RecruitmentInfoModel>>> Handle(GetDeletedRecruitmentInfoQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.RecruitmentInfos
                .IgnoreQueryFilters ( )
                .Where (x => x.IsDeleted)
                .Include (x => x.Department)
                .AsNoTracking ( )
                .ToListAsync (cancellationToken);

            var data = _mapper.Map<List<RecruitmentInfoModel>> (list);
            return Result<List<RecruitmentInfoModel>>.Success (data);
        }
    }
}
