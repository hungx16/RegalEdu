using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models.Request;


namespace RegalEdu.Application.AccountGroupEmployee.Commands
{
    public class AddAccountGroupEmployeeCommand : IRequest<Result>
    {
        public AccountGroupEmployeeRequestModel AccountGroupEmployeeRequestModel { get; set; }
    }
    public class AddAccountGroupEmployeeCommandHandler : IRequestHandler<AddAccountGroupEmployeeCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IRegalEducationDbContext _context;
        private readonly IUserPermissionInfoService _userPermissionInfoService;
        public AddAccountGroupEmployeeCommandHandler(IMapper mapper, IRegalEducationDbContext context, IUserPermissionInfoService userPermissionInfoService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userPermissionInfoService = userPermissionInfoService ?? throw new ArgumentNullException(nameof(userPermissionInfoService));
        }

        public async Task<Result> Handle(AddAccountGroupEmployeeCommand request, CancellationToken cancellationToken)
        {
            List<Domain.Entities.AccountGroupEmployee> listDataByGroupId = await _context.AccountGroupEmployees
                .Where(t => t.AccountGroupId == request.AccountGroupEmployeeRequestModel.AccountGroupId).ToListAsync();

            for (int i = 0; i < request.AccountGroupEmployeeRequestModel.ListUserCode.Count; i++)
            {
                Domain.Entities.AccountGroupEmployee data = new Domain.Entities.AccountGroupEmployee();
                data.AccountGroupId = request.AccountGroupEmployeeRequestModel.AccountGroupId;
                data.UserCode = request.AccountGroupEmployeeRequestModel.ListUserCode[i];
                data.IsApprover = request.AccountGroupEmployeeRequestModel.ListIsApprover[i];

                _context.AccountGroupEmployees.Add(data);
            }
            int result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _userPermissionInfoService.UpdateAccountGroupPermissionData(request.AccountGroupEmployeeRequestModel.AccountGroupId);
            }
            return result > 0 ? Result.Success() : Result.Failure("Failed to save AccountGroupEmployee");
        }
    }
}
