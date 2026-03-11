using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models.Request;


namespace RegalEdu.Application.AccountGroupEmployee.Commands
{
    public class SaveAccountGroupEmployeeCommand : IRequest<Result>
    {
        public AccountGroupEmployeeRequestModel AccountGroupEmployeeRequestModel { get; set; }
    }
    public class UpdateListAccountGroupEmployeeCommandHandler : IRequestHandler<SaveAccountGroupEmployeeCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IRegalEducationDbContext _context;
        private readonly IUserPermissionInfoService _userPermissionInfoService;
        public UpdateListAccountGroupEmployeeCommandHandler(IMapper mapper, IRegalEducationDbContext context, IUserPermissionInfoService userPermissionInfoService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userPermissionInfoService = userPermissionInfoService ?? throw new ArgumentNullException(nameof(userPermissionInfoService));
        }

        public async Task<Result> Handle(SaveAccountGroupEmployeeCommand request, CancellationToken cancellationToken)
        {
            // remove exists data
            List<Domain.Entities.AccountGroupEmployee> listDataRemove = await _context.AccountGroupEmployees
                 .Where(t => t.AccountGroupId == request.AccountGroupEmployeeRequestModel.AccountGroupId).ToListAsync();
            _context.AccountGroupEmployees.RemoveRange(listDataRemove);
            await _context.SaveChangesAsync();

            for (int i = 0; i < request.AccountGroupEmployeeRequestModel.ListUserCode.Count; i++)
            {
                Domain.Entities.AccountGroupEmployee data = new Domain.Entities.AccountGroupEmployee();
                data.AccountGroupId = request.AccountGroupEmployeeRequestModel.AccountGroupId;
                data.UserCode = request.AccountGroupEmployeeRequestModel.ListUserCode[i];
                data.IsApprover = request.AccountGroupEmployeeRequestModel.ListIsApprover[i];
                _context.AccountGroupEmployees.Add(data);
            }
            int result = await _context.SaveChangesAsync();
            await _userPermissionInfoService.UpdateAccountGroupPermissionData(request.AccountGroupEmployeeRequestModel.AccountGroupId);

            return result > 0 ? Result.Success() : Result.Failure("Failed to save AccountGroupEmployee");
        }
    }
}
