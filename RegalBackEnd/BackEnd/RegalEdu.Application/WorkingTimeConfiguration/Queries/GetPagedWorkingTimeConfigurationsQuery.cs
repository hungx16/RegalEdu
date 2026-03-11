using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.WorkingTimeConfiguration.Queries
{
    public class WorkingTimeConfigurationQuery
    {
        public string? NameConfiguration { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetPagedWorkingTimeConfigurationsQuery : IRequest<Result<PagedResult<WorkingTimeConfigurationModel>>>
    {
        public WorkingTimeConfigurationQuery? WorkingTimeConfigurationQuery { get; set; }

        public class GetPagedWorkingTimeConfigurationsQueryHandler : IRequestHandler<GetPagedWorkingTimeConfigurationsQuery, Result<PagedResult<WorkingTimeConfigurationModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly PagingOptions _pagingOptions;
            private readonly IMapper _mapper;

            public GetPagedWorkingTimeConfigurationsQueryHandler(
                IRegalEducationDbContext context,
                PagingOptions pagingOptions,
                IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<PagedResult<WorkingTimeConfigurationModel>>> Handle(GetPagedWorkingTimeConfigurationsQuery request, CancellationToken cancellationToken)
            {
                if (request.WorkingTimeConfigurationQuery == null)
                    throw new ArgumentNullException (nameof (request.WorkingTimeConfigurationQuery));

                var query = _context.WorkingTimeConfigurations
                    .Include (x => x.WorkingTimes)
                    .Include (x => x.Holidays)
                    .Include (x => x.WorkingTimeConfigurationCompanies)
                    .AsNoTracking ( );

                if (!string.IsNullOrWhiteSpace (request.WorkingTimeConfigurationQuery.NameConfiguration))
                    query = query.Where (x => x.NameConfiguration.Contains (request.WorkingTimeConfigurationQuery.NameConfiguration));

                int totalRecords = await query.CountAsync (cancellationToken);
                request.WorkingTimeConfigurationQuery.PageSize = _pagingOptions.DefaultPageSize;
                var paged = await query
                    .OrderByDescending (x => x.CreatedAt)
                    .Skip ((request.WorkingTimeConfigurationQuery.Page - 1) * request.WorkingTimeConfigurationQuery.PageSize)
                    .Take (request.WorkingTimeConfigurationQuery.PageSize)
                    .ToListAsync (cancellationToken);

                var result = paged.Select (x => _mapper.Map<WorkingTimeConfigurationModel> (x)).ToList ( );

                var pagedResult = new PagedResult<WorkingTimeConfigurationModel>
                {
                    Items = result,
                    Total = totalRecords
                };

                return Result<PagedResult<WorkingTimeConfigurationModel>>.Success (pagedResult);
            }
        }
    }
}
