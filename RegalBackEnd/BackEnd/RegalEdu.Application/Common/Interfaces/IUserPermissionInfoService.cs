using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.Common.Interfaces
{
    public interface IUserPermissionInfoService
    {
        Task<List<AccountGroupPermissionModel>> GetPermissionsByEmployeeATID(string empATID);
        Task UpdateAccountGroupPermissionData(Guid id);
        Task<bool> CheckPermissionByEmpAndFormAndAction(string empATID, string formName, PermissionAction action);
        Task<List<string>> GetMenuAcceptByEmployee(string empATID);
        Task<Result<List<FormPermissionDTO>>> GetMenuAndPermissionByEmployee(string empATID);
        string GetUserRoleName(string empATID);
        bool CheckUserIsAdmin(string empATID);
        Task<bool> CheckUserIsAdmissionAsync(string empATID);
        Task<bool> CheckUserIsMarketingAsync(string empATID);
        Task<bool> CheckUserIsAcademicAffairsAsync(string empATID);

        Task<List<string>> GetListEmployeeATIDByFormNameAndAction(string formName, PermissionAction action);

        /// <summary>
        /// Trả về danh sách ApplicationUser.Id của các user thuộc nhóm tài khoản.
        /// </summary>
        /// <param name="accountGroupId">Id của AccountGroup.</param>
        Task<List<Guid>> GetUserIdsByAccountGroupAsync(Guid accountGroupId);

        /// <summary>
        /// Trả về danh sách ApplicationUser.Id của người dùng thuộc nhóm tuyển sinh (AdmissionGroup từ cấu hình).
        /// </summary>
        Task<List<Guid>> GetAdmissionUserIdsAsync();

        /// <summary>
        /// Trả về danh sách ApplicationUser.Id của người dùng thuộc nhóm marketing (MarketingGroup từ cấu hình).
        /// </summary>
        Task<List<Guid>> GetMarketingUserIdsAsync();
    }
}
