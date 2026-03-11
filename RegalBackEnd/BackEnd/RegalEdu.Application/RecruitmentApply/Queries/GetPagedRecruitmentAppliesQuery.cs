using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.RecruitmentApply.Queries
{
    public class RecruitmentApplyQuery
    {
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
        public Guid? RecruitmentInfoId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedRecruitmentAppliesQuery : IRequest<Result<PagedResult<RecruitmentApplyModel>>>
    {
        public required RecruitmentApplyQuery Query { get; set; }

        public class Handler : IRequestHandler<GetPagedRecruitmentAppliesQuery, Result<PagedResult<RecruitmentApplyModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly PagingOptions _paging;

            public Handler(IRegalEducationDbContext context, IMapper mapper, PagingOptions paging)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _paging = paging ?? throw new ArgumentNullException (nameof (paging));
            }

            public async Task<Result<PagedResult<RecruitmentApplyModel>>> Handle(GetPagedRecruitmentAppliesQuery request, CancellationToken cancellationToken)
            {
                var q = _context.RecruitmentApplies
                    .Include (x => x.RecruitmentInfo)
                    .Include (x => x.Attachment)
                    .AsNoTracking ( );

                if (!string.IsNullOrWhiteSpace (request.Query.CandidateName))
                    q = q.Where (x => x.CandidateName.Contains (request.Query.CandidateName));

                if (!string.IsNullOrWhiteSpace (request.Query.CandidateEmail))
                    q = q.Where (x => x.CandidateEmail.Contains (request.Query.CandidateEmail));

                if (request.Query.RecruitmentInfoId.HasValue)
                    q = q.Where (x => x.RecruitmentInfoId == request.Query.RecruitmentInfoId.Value);

                int total = await q.CountAsync (cancellationToken);

                var pageSize = request.Query.PageSize > 0 ? request.Query.PageSize : _paging.DefaultPageSize;
                var page = request.Query.Page > 0 ? request.Query.Page : 1;

                var items = await q
                    .OrderByDescending (x => x.CreatedAt)
                    .Skip ((page - 1) * pageSize)
                    .Take (pageSize)
                    .ToListAsync (cancellationToken);

                var data = _mapper.Map<List<RecruitmentApplyModel>> (items);
                return Result<PagedResult<RecruitmentApplyModel>>.Success (new PagedResult<RecruitmentApplyModel>
                {
                    Items = data,
                    Total = total
                });
            }
        }
    }
}
