using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RecruitmentInfo.Queries
{
    public class GetAllRecruitmentInfoQuery : IRequest<Result<List<RecruitmentInfoModel>>> { }

    public class GetAllRecruitmentInfoQueryHandler : IRequestHandler<GetAllRecruitmentInfoQuery, Result<List<RecruitmentInfoModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public GetAllRecruitmentInfoQueryHandler(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<List<RecruitmentInfoModel>>> Handle(GetAllRecruitmentInfoQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.RecruitmentInfos
                .Include (x => x.Department)
                .AsNoTracking ( )
                .ToListAsync (cancellationToken);

            var data = _mapper.Map<List<RecruitmentInfoModel>> (list);
            return Result<List<RecruitmentInfoModel>>.Success (data);
        }
    }
}
