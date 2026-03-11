using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using System.Security.Claims;


namespace RegalEdu.Application.Auth.Queries
{
    public class GetUserLoginResultQuery : IRequest<IdentityResult>
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class GetUserLoginResultQueryHandler : IRequestHandler<GetUserLoginResultQuery, IdentityResult>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetUserLoginResultQueryHandler> _logger;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IRegalEducationDbContext _regalEducationDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public GetUserLoginResultQueryHandler(
             ILogger<GetUserLoginResultQueryHandler> logger,
             IIdentityService identityService,
             IJwtAuthManager jwtAuthManager,
             IRegalEducationDbContext regalEducationDbContext,
             IFileService fileService,
             IWebHostEnvironment webHostEnvironment)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(identityService));
            _jwtAuthManager = jwtAuthManager ?? throw new ArgumentNullException(nameof(identityService));
            _regalEducationDbContext = regalEducationDbContext ?? throw new ArgumentNullException(nameof(regalEducationDbContext));
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IdentityResult> Handle(GetUserLoginResultQuery request, CancellationToken cancellationToken)
        {
            var result = await _identityService.SignInAsync(request.UserName, request.Password, request.RememberMe);
            var user = await _identityService.GetUserByIdentifierAsync(request.UserName);

            if (result.IsLockedOut)
            {
                return IdentityResult.LockedOut();
            }

            if (result.Succeeded)
            {
                if (user.RequireChangePassword)
                {
                    return IdentityResult.MustChangePassword();
                }

                var roles = await _identityService.GetRolesUserAsync(request.UserName);
                var userId = await _identityService.GetUserIdAsync(request.UserName);
                var employee = await _regalEducationDbContext.Employees.Include(t => t.ApplicationUser)
                        .Where(t => t.ApplicationUser != null && t.ApplicationUser.Id == userId)
                        .FirstOrDefaultAsync(cancellationToken: cancellationToken);
                var teacher = await _regalEducationDbContext.Teachers.Include(t => t.ApplicationUser)
                        .Where(t => t.ApplicationUser != null && t.ApplicationUser.Id == userId)
                        .FirstOrDefaultAsync(cancellationToken: cancellationToken);
                var student = await _regalEducationDbContext.Students.Include(t => t.ApplicationUser)
                        .Where(t => t.ApplicationUser != null && t.ApplicationUser.Id == userId)
                        .FirstOrDefaultAsync(cancellationToken: cancellationToken);
                await _identityService.ResetAccessFailedCountAsync(userId);

                var employeeIdString = employee != null ? employee.Id.ToString() : string.Empty;
                var teacherIdString = teacher != null ? teacher.Id.ToString() : string.Empty;
                var studentIdString = student != null ? student.Id.ToString() : string.Empty;
                var userCode = student != null && !string.IsNullOrWhiteSpace(student.StudentCode) ? student.StudentCode : user.UserCode;
                var userNameToReturn = user.Email ?? user.UserName ?? request.UserName;

                List<Claim> claims = BuidUserClaims(userNameToReturn, userId.ToString(), roles, userCode, user.FullName, employeeIdString, teacherIdString, studentIdString);
                var jwtResult = _jwtAuthManager.GenerateTokens(userNameToReturn, claims.ToArray(), DateTime.Now);
                var avatarUrl = string.Empty;
                var avatarBytes = employee?.ApplicationUser?.Avatar
                    ?? teacher?.ApplicationUser?.Avatar
                    ?? student?.ApplicationUser?.Avatar
                    ?? user?.Avatar;
                if (avatarBytes != null)
                {
                    avatarUrl = Convert.ToBase64String(avatarBytes);
                }
                if (string.IsNullOrEmpty(avatarUrl))
                {
                    // convert avatar to base 64
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, "avatar.png");
                    if (File.Exists(path))
                    {
                        byte[] imageData = File.ReadAllBytes(path);
                        avatarUrl = Convert.ToBase64String(imageData);
                    }
                }

                _logger.LogInformation($"User [{request.UserName}] logged in the system.");

                user.RefreshToken = jwtResult.RefreshToken;
                user.RefreshTokenExpiry = DateTime.Now.AddDays(1);
                await _identityService.UpdateUserAsync(user);
                return IdentityResult.Success(userNameToReturn, roles, user.FullName, jwtResult.AccessToken, jwtResult.RefreshToken, avatarUrl);
            }

            return IdentityResult.Error("UserName or Password incorrect");
        }

        private static List<Claim> BuidUserClaims(string userName, string userId, IList<string> roles, string userCode, string fullName, string employeeId, string teacherId, string studentId)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("FullName", fullName),
                new Claim("EmployeeId", employeeId),
                new Claim("TeacherId", teacherId),
                new Claim("StudentId", studentId),
                new Claim("preferred_username", userName),
                new Claim("UserCode", userCode),

            };

            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }

            return claims;
        }
    }
}
