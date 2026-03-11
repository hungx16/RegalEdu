using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Request;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AccountGroup.Queries
{
    public class GetAccountGroupsQuery : IRequest<Result<PagedResult<AccountGroupModel>>>
    {
        public AccountGroupQuery? AccountGroupQuery { get; set; }
    }
    public class GetAccountGroupsQueryHandler : IRequestHandler<GetAccountGroupsQuery, Result<PagedResult<AccountGroupModel>>>
    {
        private readonly IMapper _mapper;
        private readonly IRegalEducationDbContext _context;

        public GetAccountGroupsQueryHandler(IMapper mapper, IRegalEducationDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<PagedResult<AccountGroupModel>>> Handle(GetAccountGroupsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.AccountGroupQuery == null)
                {
                    throw new ArgumentNullException(nameof(request.AccountGroupQuery));
                }

                // Start querying
                IQueryable<Domain.Entities.AccountGroup> queryData = _context.AccountGroups.AsNoTracking();

                // Filter by each field
                if (!string.IsNullOrWhiteSpace(request.AccountGroupQuery.Name))
                {
                    queryData = queryData.Where(u => u.Name!.Contains(request.AccountGroupQuery.Name));
                }
                if (request.AccountGroupQuery.UseDefault.HasValue)
                {
                    queryData = queryData.Where(u => u.UseDefault == request.AccountGroupQuery.UseDefault.Value);
                }
                if (request.AccountGroupQuery.Enable.HasValue)
                {
                    queryData = queryData.Where(u => u.Enable == request.AccountGroupQuery.Enable.Value);
                }
                if (request.AccountGroupQuery.CreatedAt.HasValue)
                {
                    queryData = queryData.Where(u => u.CreatedAt.Date == request.AccountGroupQuery.CreatedAt.Value.Date);
                }

                int totalRecords = await queryData.CountAsync(cancellationToken);

                // Pagination
                var pageData = await queryData
                    .OrderBy(u => u.CreatedAt)
                    .Skip((request.AccountGroupQuery.Page - 1) * request.AccountGroupQuery.PageSize)
                    .Take(request.AccountGroupQuery.PageSize)
                    .ToListAsync(cancellationToken);

                // Mapping
                var result = pageData.Select(user => _mapper.Map<AccountGroupModel>(user)).ToList();

                var pagedResult = new PagedResult<AccountGroupModel>
                {
                    Items = result,
                    Total = totalRecords
                };

                return Result<PagedResult<AccountGroupModel>>.Success(pagedResult);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
