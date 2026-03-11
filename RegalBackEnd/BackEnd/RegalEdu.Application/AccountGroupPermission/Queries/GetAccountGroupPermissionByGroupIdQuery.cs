using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;


namespace RegalEdu.Application.AccountGroupPermission.Queries
{
    public class GetAccountGroupPermissionByGroupIdQuery : IRequest<Result<List<AccountGroupPermissionModel>>>
    {
        public string AccountGroupId { get; set; }
    }
    public class GetAccountGroupPermissionByGroupIdQueryHandler : IRequestHandler<GetAccountGroupPermissionByGroupIdQuery, Result<List<AccountGroupPermissionModel>>>
    {
        private readonly IMapper _mapper;
        private readonly IRegalEducationDbContext _context;

        public GetAccountGroupPermissionByGroupIdQueryHandler(IMapper mapper, IRegalEducationDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Result<List<AccountGroupPermissionModel>>> Handle(GetAccountGroupPermissionByGroupIdQuery request, CancellationToken cancellationToken)
        {
            
            List<Domain.Entities.AccountGroupPermission> listGroupPermission = await _context.AccountGroupPermissions
                 .Where(t => t.AccountGroupId.ToString() == request.AccountGroupId).ToListAsync();

            return Result<List<AccountGroupPermissionModel>>.Success( _mapper.Map<List<AccountGroupPermissionModel>>(listGroupPermission));
        }
    }
}
