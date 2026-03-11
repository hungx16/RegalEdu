using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Request;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AccountGroup.Queries
{
    public class GetAllAccountGroupsQuery : IRequest<Result<List<AccountGroupModel>>>
    {
    }
    public class GetAllAccountGroupsQueryHandler : IRequestHandler<GetAllAccountGroupsQuery, Result<List<AccountGroupModel>>>
    {
        private readonly IMapper _mapper;
        private readonly IRegalEducationDbContext _context;

        public GetAllAccountGroupsQueryHandler(IMapper mapper, IRegalEducationDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<List<AccountGroupModel>>> Handle(GetAllAccountGroupsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Start querying
                var queryData = await _context.AccountGroups.AsNoTracking().ToListAsync(cancellationToken);

                // Mapping
                var result = _mapper.Map<List<AccountGroupModel>>(queryData);

                return Result<List<AccountGroupModel>>.Success(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
