using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AccountGroupEmployee.Queries
{
    public class GetAccountGroupEmployeeByGroupIdQuery : IRequest<Result<List<AccountGroupEmployeeModel>>>
    {
        public string AccountGroupId { get; set; }
    }
    public class GetAccountGroupEmployeeByGroupIdQueryHandler : IRequestHandler<GetAccountGroupEmployeeByGroupIdQuery, Result<List<AccountGroupEmployeeModel>>>
    {
        private readonly IMapper _mapper;
        private readonly IRegalEducationDbContext _context;

        public GetAccountGroupEmployeeByGroupIdQueryHandler(IMapper mapper, IRegalEducationDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Result<List<AccountGroupEmployeeModel>>> Handle(GetAccountGroupEmployeeByGroupIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                List<RegalEdu.Domain.Entities.AccountGroupEmployee> listGroupEmployee = await _context.AccountGroupEmployees
                 .Where(t => t.AccountGroupId.ToString() == request.AccountGroupId).AsNoTracking().ToListAsync();

                return Result<List<AccountGroupEmployeeModel>>.Success( _mapper.Map<List<AccountGroupEmployeeModel>>(listGroupEmployee));
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
