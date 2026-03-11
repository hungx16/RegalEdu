using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.API.Tests.Integration.Common
{
    public static class BaseTestAuthHelper
    {
        // 1️⃣ Ensure user exists via AddApplicationUser
        public static async Task EnsureTestUserExistsUsingAddApplicationUser(HttpClient client, string userName, string password)
        {
            var newUser = new ApplicationUserModel
            {
                UserName = userName,
                Email = $"{userName}@example.com",
                PhoneNumber = "0987654321",
                FullName = $"Integration User {userName}",
                UserCode = "TEST-ATID-123",
                Gender = true,
                Password = password,
                PasswordConfirm = password,
                IsDeleted = false,
                GenderText = "Male"
            };

            var response = await client.PostAsJsonAsync("/api/RegalEduManagement/User/AddApplicationUser", newUser);

            Console.WriteLine($"[Test] AddApplicationUser for '{userName}': Response = {response.StatusCode}");
        }

        // 2️⃣ Ensure account group exists
        public static async Task EnsureAccountGroupExists(HttpClient client, string groupId, string groupName)
        {
            var groupModel = new
            {
                Id = groupId,
                Name = groupName,
                Description = $"Group {groupName}",
                IsDeleted = false
            };

            var response = await client.PostAsJsonAsync("/api/RegalEduManagement/AccountGroup/AddAccountGroup", groupModel);

            Console.WriteLine($"[Test] AddAccountGroup '{groupName}': Response = {response.StatusCode}");
        }

        // 3️⃣ Ensure user in group
        public static async Task EnsureUserInGroup(HttpClient client, string groupId, string userId)
        {
            var model = new
            {
                AccountGroupId = groupId,
                EmployeeIds = new List<string> { userId }
            };

            var response = await client.PostAsJsonAsync("/api/RegalEduManagement/AccountGroupEmployee/AddAccountGroupEmployee", model);

            Console.WriteLine($"[Test] AddUser '{userId}' to group '{groupId}': Response = {response.StatusCode}");
        }

        // 4️⃣ Ensure permission for group
        public static async Task EnsurePermissionForGroup(HttpClient client, string groupId, List<string> permissionIds)
        {
            var model = new
            {
                AccountGroupId = groupId,
                PermissionIds = permissionIds
            };

            var response = await client.PostAsJsonAsync("/api/RegalEduManagement/AccountGroupPermission/SaveAccountGroupPermission", model);

            Console.WriteLine($"[Test] SaveAccountGroupPermission for group '{groupId}': Response = {response.StatusCode}");
        }

        // 5️⃣ Get userId (EmployeeId) từ User API
        public static async Task<string> GetUserIdByUserName(HttpClient client, string userName)
        {
            var response = await client.GetAsync("/api/RegalEduManagement/User/GetApplicationUsers");

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Test] GetApplicationUsers Response: {responseBody}");

            var users = JsonConvert.DeserializeObject<List<ApplicationUserModel>>(responseBody);

            var user = users?.FirstOrDefault(u => u.UserName == userName);

            if (user == null)
                throw new Exception($"User '{userName}' not found in GetApplicationUsers");

            Console.WriteLine($"[Test] Found user '{userName}' with Id '{user.Id}'");

            return user.Id?.ToString() ?? throw new Exception($"User '{userName}' Id is null");
        }

        // 6️⃣ Final: Get token theo chuẩn flow
        public static async Task<string> GetTokenForUser(HttpClient client, string userName, string password, string groupId, string groupName, List<string> permissionIds)
        {
            // Ensure user exists
            await EnsureTestUserExistsUsingAddApplicationUser(client, userName, password);

            // Ensure account group exists
            await EnsureAccountGroupExists(client, groupId, groupName);

            // Get userId
            var userId = await GetUserIdByUserName(client, userName);

            // Add user to group
            await EnsureUserInGroup(client, groupId, userId);

            // Add permission to group
            await EnsurePermissionForGroup(client, groupId, permissionIds);

            // Login
            var loginRequest = new
            {
                UserName = userName,
                Password = password
            };

            var response = await client.PostAsync(
                "/api/RegalEduManagement/User/login",
                new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var loginResponseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Test] Login Response Body for user '{userName}': {loginResponseBody}");

            var loginResult = JsonConvert.DeserializeObject<IdentityResult>(loginResponseBody);
            return loginResult?.AccessToken ?? throw new Exception($"AccessToken not found for user '{userName}'");
        }

        // 7️⃣ Shortcut cho user_without_add_privilege
        public static async Task<string> GetTokenForUserWithoutAddPrivilege(HttpClient client, string groupId, string groupName, List<string> permissionIds)
        {
            return await GetTokenForUser(client, "user_without_add_privilege", "Test@123", groupId, groupName, permissionIds);
        }
    }
}
