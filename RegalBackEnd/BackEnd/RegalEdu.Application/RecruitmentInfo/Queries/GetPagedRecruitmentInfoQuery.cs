using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RecruitmentInfo.Queries
{
    public class RecruitmentInfoQuery
    {
        public string? RecruitmentInfoName { get; set; }
        public string? Position { get; set; }
        public string? Province { get; set; }
        public Guid? DepartmentId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedRecruitmentInfoQuery : IRequest<Result<PagedResult<RecruitmentInfoModel>>>
    {
        public required RecruitmentInfoQuery Query { get; set; }
    }

    public class GetPagedRecruitmentInfoQueryHandler : IRequestHandler<GetPagedRecruitmentInfoQuery, Result<PagedResult<RecruitmentInfoModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly PagingOptions _paging;

        public GetPagedRecruitmentInfoQueryHandler(IRegalEducationDbContext context, IMapper mapper, PagingOptions paging)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _paging = paging ?? throw new ArgumentNullException (nameof (paging));
        }

        public async Task<Result<PagedResult<RecruitmentInfoModel>>> Handle(GetPagedRecruitmentInfoQuery request, CancellationToken cancellationToken)
        {
            var q = _context.RecruitmentInfos
                .Include (x => x.Department)
                .AsNoTracking ( );

            if (!string.IsNullOrWhiteSpace (request.Query.RecruitmentInfoName))
                q = q.Where (x => x.RecruitmentInfoName.Contains (request.Query.RecruitmentInfoName));

            if (!string.IsNullOrWhiteSpace (request.Query.Position))
                q = q.Where (x => x.Position.Contains (request.Query.Position));

            if (!string.IsNullOrWhiteSpace (request.Query.Province))
                q = q.Where (x => x.ProvinceCode.Contains (request.Query.Province));

            if (request.Query.DepartmentId.HasValue)
                q = q.Where (x => x.DepartmentId == request.Query.DepartmentId.Value);

            int total = await q.CountAsync (cancellationToken);

            var pageSize = request.Query.PageSize > 0 ? request.Query.PageSize : _paging.DefaultPageSize;
            var page = request.Query.Page > 0 ? request.Query.Page : 1;

            var items = await q
                .OrderByDescending (x => x.CreatedAt)
                .Skip ((page - 1) * pageSize)
                .Take (pageSize)
                .ToListAsync (cancellationToken);

            var models = _mapper.Map<List<RecruitmentInfoModel>> (items);

            return Result<PagedResult<RecruitmentInfoModel>>.Success (new PagedResult<RecruitmentInfoModel>
            {
                Items = models,
                Total = total
            });
        }
    }
}
