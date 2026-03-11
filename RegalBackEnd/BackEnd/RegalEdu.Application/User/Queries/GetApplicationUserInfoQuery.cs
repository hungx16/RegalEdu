using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.User.Queries
{
    public class GetApplicationUserInfoQuery : IRequest<ApplicationUserModel>
    {
    }

    public class GetApplicationUserInfoQueryHandler(
           IMapper mapper,
           ILogger<GetApplicationUserInfoQueryHandler> logger,
           ICurrentUserService currentUserService,
           IIdentityService identityService) : IRequestHandler<GetApplicationUserInfoQuery, ApplicationUserModel>
    {
        private readonly IIdentityService _identityService = identityService ?? throw new ArgumentNullException (nameof (identityService));
        private readonly ILogger<GetApplicationUserInfoQueryHandler> _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        private readonly ICurrentUserService _currentUserService = currentUserService ?? throw new ArgumentNullException (nameof (currentUserService));

        public async Task<ApplicationUserModel> Handle(GetApplicationUserInfoQuery request, CancellationToken cancellationToken)
        {
            var users = await _identityService.GetUsersAsync ( );
            if (users == null || users.Count == 0)
            {
                _logger.LogWarning ("No users found.");
                return new ApplicationUserModel ( );
            }
            else
            {
                var user = users.FirstOrDefault (x => x.UserName == _currentUserService.UserName);
                if (user != null)
                {
                    var empInfo = new ApplicationUserModel
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        UserCode = user.UserCode,
                        UserName = user.UserName,
                        Email = user.Email,
                        Avatar = user?.Avatar,
                        Gender = user.Gender,
                        GenderText = user.Gender ? "Nam" : "Nữ",
                        CreatedAt = user.CreatedAt,
                        IsDeleted = user.IsDeleted,
                        PhoneNumber = user.PhoneNumber,
                    };
                    return empInfo;
                }
            }
            _logger.LogWarning ("No users found.");

            return new ApplicationUserModel ( );

        }
    }


}
