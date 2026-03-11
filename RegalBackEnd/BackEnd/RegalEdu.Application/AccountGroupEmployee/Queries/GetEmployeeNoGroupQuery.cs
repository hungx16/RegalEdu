using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;



namespace RegalEdu.Application.AccountGroupEmployee.Queries
{
    public class GetEmployeeNoGroupQuery : IRequest<Result<List<string>>>
    {
    }
    public class GetAccountGroupEmployeeNoGroupQueryHandler : IRequestHandler<GetEmployeeNoGroupQuery, Result<List<string>>>
    {
        private readonly IMapper _mapper;
        private readonly IRegalEducationDbContext _context;
        public GetAccountGroupEmployeeNoGroupQueryHandler(IMapper mapper, IRegalEducationDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _context = context ?? throw new ArgumentNullException (nameof (context));
        }

        public async Task<Result<List<string>>> Handle(GetEmployeeNoGroupQuery request, CancellationToken cancellationToken)
        {
            try
            {
                List<string> listEmpInGroup = await _context.AccountGroupEmployees.Select (t => t.UserCode).ToListAsync ( );
                List<string> listEmpFull = _context.ApplicationUsers.Select (t => t.UserCode).ToList ( );
                List<string> listEmpNoGroup = new List<string> ( );
                for (int i = 0; i < listEmpFull.Count; i++)
                {
                    if (listEmpInGroup.Contains (listEmpFull[i]) == false && listEmpNoGroup.Contains (listEmpFull[i]) == false)
                    {
                        listEmpNoGroup.Add (listEmpFull[i]);
                    }
                }
                return Result<List<string>>.Success (listEmpNoGroup);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
