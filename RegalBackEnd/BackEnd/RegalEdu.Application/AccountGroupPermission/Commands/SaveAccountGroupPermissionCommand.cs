
using AutoMapper;
using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.Request;


namespace RegalEdu.Application.AccountGroupPermission.Commands
{
    public class SaveAccountGroupPermissionCommand : IRequest<Result>
    {
        public required AccountGroupPermissionRequestModel RequestModel { get; set; }
    }
    public class CreateListAccountGroupPermissionCommandHandler : IRequestHandler<SaveAccountGroupPermissionCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IRegalEducationDbContext _context;
        private readonly IUserPermissionInfoService _userPermissionInfoService;
        public CreateListAccountGroupPermissionCommandHandler(IMapper mapper, IRegalEducationDbContext context, IUserPermissionInfoService userPermissionInfoService)
        {
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _userPermissionInfoService = userPermissionInfoService ?? throw new ArgumentNullException (nameof (userPermissionInfoService));
        }

        public async Task<Result> Handle(SaveAccountGroupPermissionCommand request, CancellationToken cancellationToken)
        {
            List<Domain.Entities.AccountGroupPermission> listGroupPermission = _context.AccountGroupPermissions
                .Where (t => t.AccountGroupId == request.RequestModel.AccountGroupId).ToList ( );

            foreach (AccountGroupPermissionModel item in request.RequestModel.ListGroupPermission)
            {
                Domain.Entities.AccountGroupPermission? groupPermissionData = listGroupPermission.Where (t => t.FormName == item.FormName && t.Action == item.Action).FirstOrDefault ( );
                if (groupPermissionData == null)
                {
                    groupPermissionData = new Domain.Entities.AccountGroupPermission ( );
                    groupPermissionData.AccountGroupId = request.RequestModel.AccountGroupId;
                    groupPermissionData.FormName = item.FormName;
                    groupPermissionData.Action = item.Action;

                    _context.AccountGroupPermissions.Add (groupPermissionData);
                }
                groupPermissionData.AllowAction = item.AllowAction;
            }
            int result = await _context.SaveChangesAsync ( );
            if (result > 0)
            {
                await _userPermissionInfoService.UpdateAccountGroupPermissionData (request.RequestModel.AccountGroupId);
            }
            return result > 0 ? Result.Success ( ) : Result.Failure ("Failed to update Access management");
        }
    }
}
