using Microsoft.AspNetCore.Identity;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.User.Commands;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<Guid> GetUserIdAsync(string userName);
        Task<(Result, Guid)> CreateUserAsync(string userName, string password, bool mustChangePassword = false, string email = "", string fullName = "", Guid? departmentId = null);
        Task<(Result, Guid)> UpdateUserAsync(ApplicationUser user);
        Task<SignInResult> SignInAsync(string userName, string password, bool rememberMe);
        Task<IList<string>> GetRolesUserAsync(string userName);
        Task<string> GetRoleUserAsync(string userName);
        Task<Result> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<(Result, string)> GenerateNewPasswordAsync(string email);
        Task<ApplicationUser> GetUserByIdentifierAsync(string identifier);
        Task<List<ApplicationUser>> GetUsersAsync();
        Task<Microsoft.AspNetCore.Identity.IdentityResult> AssignUserToRole(ApplicationUser user, string role);
        Task ResetAccessFailedCountAsync(Guid userId);
        Task LockUserAsync(string userId);
        Task UnlockUserAsync(string userId);
        Task<(Result, string)> CreateUserWithTemporaryPasswordAsync(string email, string userName, string roleId);
        Task<Result> DeleteUsersAsync(List<string> arrUserId);

        Task<Result> CreateUserAsync(ApplicationUser user,
            string password);
        Task<bool> IsEmployeeATIDExistsAsync(string employeeATID);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsUserNameExistsAsync(string userName);
        Task<bool> IsUserNameExistsForOtherUserAsync(Guid userId, string userName);
        Task<bool> IsEmailExistsForOtherUserAsync(Guid userId, string email);
        Task<ApplicationUser?> FindByIdAsync(Guid userId);

    }
}
