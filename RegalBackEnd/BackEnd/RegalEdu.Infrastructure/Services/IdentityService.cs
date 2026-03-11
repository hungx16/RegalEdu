using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Infrastructure.Extensions;
using RegalEducation.Persistence;
using System.Text;


namespace RegalEdu.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly RegalEducationDbContext _context;

        public IdentityService(
            RegalEducationDbContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager
            )
        {
            _userManager = userManager ?? throw new ArgumentNullException (nameof (userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException (nameof (signInManager));
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _roleManager = roleManager ?? throw new ArgumentNullException (nameof (roleManager));
        }

        public async Task<Guid> GetUserIdAsync(string userName)
        {
            var user = await _userManager.Users.FirstAsync (u => string.Equals (userName, u.UserName));
            return user.Id;
        }

        public async Task<(Result, Guid)> CreateUserAsync(string userName,
            string password,
            bool mustChangePassword = false,
            string email = "",
            string fullName = "",
            Guid? departmentId = null)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email ?? userName,
                LockoutEnabled = false,
                LockoutEnd = DateTime.Now,
                RequireChangePassword = mustChangePassword,
                FullName = fullName,
            };

            var result = await _userManager.CreateAsync (user, password);
            return (result.ToApplicationResult ( ), user.Id);
        }
        public async Task<Result> CreateUserAsync(ApplicationUser user,
            string password)
        {
            var result = await _userManager.CreateAsync (user, password);
            return result.ToApplicationResult ( );
        }
        public async Task<Microsoft.AspNetCore.Identity.IdentityResult> AssignUserToRole(ApplicationUser user, string role)
        {
            return await _userManager.AddToRoleAsync (user, role);
        }

        public async Task<SignInResult> SignInAsync(string userName, string password, bool rememberMe)
        {
            return await _signInManager.PasswordSignInAsync (userName, password, rememberMe, lockoutOnFailure: false);
        }

        public async Task<IList<string>> GetRolesUserAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync (userName);
            var roles = await _userManager.GetRolesAsync (user);
            return roles;
        }

        public async Task<Result> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync (userId);
            if (user == null)
            {
                return Result.Failure ("Can not be found the user");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync (user, oldPassword, newPassword);
            if (!changePasswordResult.Succeeded)
            {
                return changePasswordResult.ToApplicationResult ( );
            }

            await _signInManager.RefreshSignInAsync (user);

            return Result.Success ( );
        }

        public async Task<(Result, string)> GenerateNewPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync (email);

            if (user == null)
            {
                return (Result.Failure ("Can not be found the user"), string.Empty);
            }

            var temporaryPassword = CreatePassword (10);
            var token = await _userManager.GeneratePasswordResetTokenAsync (user);
            var setNewPasswordResult = await _userManager.ResetPasswordAsync (user, token, temporaryPassword);

            if (!setNewPasswordResult.Succeeded)
            {
                return (setNewPasswordResult.ToApplicationResult ( ), string.Empty);
            }

            //await _userManager.UpdateAsync(user);
            //await _signInManager.RefreshSignInAsync(user);

            return (Result.Success ( ), temporaryPassword);
        }

        public async Task<(Result, string)> CreateUserWithTemporaryPasswordAsync(string email, string userName, string roleId)
        {
            var user = await _userManager.FindByEmailAsync (email);
            if (user != null)
            {
                return (Result.Failure ("User has been existed in system"), string.Empty);
            }

            var temporaryPassword = CreatePassword (10);

            (Result result, Guid userId) = await CreateUserAsync (userName, temporaryPassword, false, email);

            if (result.Succeeded)
            {
                user = await _userManager.FindByIdAsync (userId.ToString ( ));

                var role = await _roleManager.Roles.FirstOrDefaultAsync (x => x.Id.Equals (roleId));

                if (role == null)
                {
                    throw new ArgumentNullException (nameof (role));
                }

                await AssignUserToRole (user, role.Name);
            }

            return (result, temporaryPassword);
        }

        public async Task<ApplicationUser> GetUserByIdentifierAsync(string identifier)
        {

            var user = await _userManager.FindByNameAsync (identifier);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync (identifier);
            }
            return user;
        }

        public async Task<List<ApplicationUser>> GetUsersAsync( )
        {
            return await _context.Users.ToListAsync ( );
        }

        public async Task LockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync (userId);

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.UtcNow.AddDays (36500);

            await _userManager.UpdateAsync (user);
        }

        public async Task UnlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync (userId);
            user.LockoutEnabled = false;
            user.LockoutEnd = DateTime.Now;
            await _userManager.UpdateAsync (user);
        }

        public async Task ResetAccessFailedCountAsync(Guid userId)
        {
            ApplicationUser user = new ApplicationUser ( )
            {
                Id = userId,
                FullName = string.Empty,
            };
            await _userManager.ResetAccessFailedCountAsync (user);
        }

        internal string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder ( );
            Random rnd = new Random ( );
            while (0 < length--)
            {
                res.Append (valid[rnd.Next (valid.Length)]);
            }
            return res.ToString ( );
        }

        public async Task<string> GetRoleUserAsync(string userName)
        {
            var roles = await GetRolesUserAsync (userName);
            return roles.FirstOrDefault ( );
        }

        public async Task<(Result, Guid)> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync (user);
            return (result.ToApplicationResult ( ), user.Id);
        }

        public async Task<Result> DeleteUsersAsync(List<string> arrUserId)
        {
            var errors = new List<string> ( );

            foreach (var userId in arrUserId)
            {
                var user = await _userManager.FindByIdAsync (userId);
                if (user == null)
                {
                    errors.Add ($"User with ID {userId} not found.");
                    continue;
                }

                var result = await _userManager.DeleteAsync (user);
                if (!result.Succeeded)
                {
                    errors.Add ($"Failed to delete user with ID {userId}: {string.Join (", ", result.Errors.Select (e => e.Description))}");
                }
            }

            if (errors.Any ( ))
            {
                return Result.Failure (string.Join ("; ", errors));
            }

            return Result.Success ( );
        }

        public async Task<bool> IsEmployeeATIDExistsAsync(string userCode)
        {
            if (string.IsNullOrWhiteSpace (userCode))
            {
                return false;
            }

            return await _userManager.Users
                .AnyAsync (u => u.UserCode == userCode);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace (email))
            {
                return false;
            }

            return await _userManager.Users
                .AnyAsync (u => u.Email == email);
        }
        public async Task<bool> IsUserNameExistsAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace (userName))
            {
                return false;
            }

            return await _userManager.Users
                .AnyAsync (u => u.UserName == userName);
        }
        public async Task<bool> IsUserNameExistsForOtherUserAsync(Guid userId, string userName)
        {
            var user = await _userManager.Users
                .Where (u => u.Id != userId && u.UserName == userName)
                .FirstOrDefaultAsync ( );

            return user != null;
        }
        public async Task<bool> IsEmailExistsForOtherUserAsync(Guid userId, string email)
        {
            var user = await _userManager.Users
                .Where (u => u.Id != userId && u.Email == email)
                .FirstOrDefaultAsync ( );

            return user != null;
        }

        public async Task<ApplicationUser?> FindByIdAsync(Guid userId)
        {
            return await _userManager.FindByIdAsync (userId.ToString ( ));
        }
    }
}
