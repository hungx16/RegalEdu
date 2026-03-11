using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Infrastructure.Services
{
    public class UserPermissionInfoService : IUserPermissionInfoService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserPermissionInfoService> _logger;
        private readonly IMapper _mapper;
        private readonly List<string> _listAdminGroup, _listAdmissionGroup, _listMarketingGroup, _listAcademicAffairsGroup;

        private List<UserPermissionDTO> ListUserPermission;
        public UserPermissionInfoService(IServiceProvider serviceProvider, IMapper mapper, IConfiguration configuration,
           ILogger<UserPermissionInfoService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            ListUserPermission = new List<UserPermissionDTO>();
            string? strGroup = configuration.GetSection("AdminGroup")?.Value;
            string? strAdmissionGroup = configuration.GetSection("AdmissionGroup")?.Value;
            string? strMarketingGroup = configuration.GetSection("MarketingGroup")?.Value;
            string? strAcademicAffairsGroup = configuration.GetSection("AcademicAffairsGroup")?.Value;

            _listAdminGroup = strGroup?.Split(';').ToList() ?? new List<string>();
            _listAdmissionGroup = strAdmissionGroup?.Split(';').ToList() ?? new List<string>();
            _listMarketingGroup = strMarketingGroup?.Split(';').ToList() ?? new List<string>();
            _listAcademicAffairsGroup = strAcademicAffairsGroup?.Split(';').ToList() ?? new List<string>();
        }
        public async Task<List<AccountGroupPermissionModel>> GetPermissionsByEmployeeATID(string empATID)
        {
            var scope = _serviceProvider.CreateScope();
            IRegalEducationDbContext? context = scope.ServiceProvider.GetService<IRegalEducationDbContext>();

            if (context == null)
            {
                throw new InvalidOperationException("Failed to resolve IRegalEducationDbContext from the service provider.");
            }

            UserPermissionDTO? userPermission = ListUserPermission.FirstOrDefault(t => t.ListUserCode != null && t.ListUserCode.Contains(empATID));
            List<AccountGroupPermissionModel> listPermissionModel = new List<AccountGroupPermissionModel>();
            if (userPermission == null)
            {
                userPermission = new UserPermissionDTO();
                List<Domain.Entities.AccountGroup> listGroup = await context.AccountGroups.AsNoTracking().ToListAsync();

                if (listGroup == null || listGroup.Count == 0)
                {
                    _logger.LogWarning("No account groups found in the database.");
                    return listPermissionModel;
                }
                Domain.Entities.AccountGroup? accountGroup = null;
                // get account group by emp  
                Domain.Entities.AccountGroupEmployee? groupEmployee = await context.AccountGroupEmployees.Where(t => t.UserCode == empATID).AsNoTracking().FirstOrDefaultAsync();
                // ko có group -> lấy group default  
                if (groupEmployee == null)
                {
                    accountGroup = listGroup.Where(t => t.UseDefault == true).FirstOrDefault();
                }
                else
                {
                    accountGroup = listGroup.Where(t => t.Id == groupEmployee.AccountGroupId).FirstOrDefault();
                }

                if (accountGroup != null)
                {
                    userPermission.GroupData = _mapper.Map<AccountGroupModel>(accountGroup);
                    userPermission.ListUserCode = await context.AccountGroupEmployees.Where(t => t.AccountGroupId == userPermission.GroupData.Id)
                        .Select(t => t.UserCode).AsNoTracking().ToListAsync();
                    List<Domain.Entities.AccountGroupPermission> listPermission = await context.AccountGroupPermissions.Where(t => t.AccountGroupId == userPermission.GroupData.Id)
                        .AsNoTracking().ToListAsync();
                    userPermission.ListPermissions = _mapper.Map<List<AccountGroupPermissionModel>>(listPermission);
                    listPermissionModel = userPermission.ListPermissions ?? new List<AccountGroupPermissionModel>();
                    ListUserPermission.Add(userPermission);
                }
            }
            else
            {
                listPermissionModel = userPermission.ListPermissions ?? new List<AccountGroupPermissionModel>();
            }

            return listPermissionModel;
        }
        public async Task UpdateAccountGroupPermissionData(Guid id)
        {
            var scope = _serviceProvider.CreateScope();
            IRegalEducationDbContext? context = scope.ServiceProvider.GetService<IRegalEducationDbContext>();

            if (context == null)
            {
                throw new InvalidOperationException("Failed to resolve IRegalEducationDbContext from the service provider.");
            }

            UserPermissionDTO? userPermission = ListUserPermission.Where(t => t.GroupData != null && t.GroupData.Id == id).FirstOrDefault();
            if (userPermission != null)
            {
                userPermission.ListUserCode = await context.AccountGroupEmployees
                    .Where(t => t.AccountGroupId == userPermission.GroupData!.Id) // Use null-forgiving operator (!) to suppress nullability warning
                    .Select(t => t.UserCode)
                    .AsNoTracking()
                    .ToListAsync();

                List<Domain.Entities.AccountGroupPermission> listPermission = await context.AccountGroupPermissions
                    .Where(t => t.AccountGroupId == userPermission.GroupData!.Id) // Use null-forgiving operator (!) to suppress nullability warning
                    .AsNoTracking()
                    .ToListAsync();

                userPermission.ListPermissions = _mapper.Map<List<AccountGroupPermissionModel>>(listPermission);
            }
        }
        public async Task<bool> CheckPermissionByEmpAndFormAndAction(string empATID, string formName, PermissionAction action)
        {
            List<AccountGroupPermissionModel> listPermission = await GetPermissionsByEmployeeATID(empATID);
            List<string> listFormName = new List<string> { formName };

            AccountGroupPermissionModel? group = listPermission.FirstOrDefault(t => listFormName.Contains(t.FormName) && t.Action == action.ToString());
            if (group == null)
            {
                return false;
            }
            bool result = false;
            if (group != null && group.AllowAction == true)
            {
                result = true;
            }

            return result;
        }
        public async Task<List<string>> GetMenuAcceptByEmployee(string empATID)
        {
            List<AccountGroupPermissionModel> listPermission = await GetPermissionsByEmployeeATID(empATID);
            List<string> listMenuAccept = new List<string>();
            for (int i = 0; i < listPermission.Count; i++)
            {

                if (listPermission[i].Action == PermissionAction.view.ToString() && listPermission[i].AllowAction == true)
                {
                    listMenuAccept.Add(listPermission[i].FormName);
                }
            }
            return listMenuAccept;
        }
        public async Task<Result<List<FormPermissionDTO>>> GetMenuAndPermissionByEmployee(string empATID)
        {
            try
            {
                List<AccountGroupPermissionModel> listPermission = await GetPermissionsByEmployeeATID(empATID);
                List<FormPermissionDTO> listData = new List<FormPermissionDTO>();
                for (int i = 0; i < listPermission.Count; i++)
                {
                    FormPermissionDTO? data = listData.FirstOrDefault(t => t.FormName == listPermission[i].FormName);
                    if (data == null)
                    {
                        data = new FormPermissionDTO
                        {
                            FormName = listPermission[i].FormName,
                            ListAction = new List<string>()
                        };
                        listData.Add(data);
                    }
                    if (listPermission[i].AllowAction == true && data.ListAction != null)
                    {
                        data.ListAction.Add(listPermission[i].Action);
                    }
                }
                return Result<List<FormPermissionDTO>>.Success(listData);
            }
            catch (Exception)
            {
                throw;
            }

        }
        public string GetUserRoleName(string empATID)
        {
            UserPermissionDTO? userPermission = ListUserPermission.FirstOrDefault(t => t.ListUserCode != null && t.ListUserCode.Contains(empATID));
            string roleName = "";
            if (userPermission != null && userPermission.GroupData != null)
            {
                roleName = userPermission.GroupData.Name;
            }
            return roleName;
        }
        public bool CheckUserIsAdmin(string empATID)
        {
            string role = GetUserRoleName(empATID);
            if (role != "" && _listAdminGroup.Contains(role))
            {
                return true;
            }

            return false;
        }
        public bool CheckUserIsAdmission(string empATID)
        {
            string role = GetUserRoleName(empATID);
            if (role != "" && _listAdmissionGroup.Contains(role))
            {
                return true;
            }

            return false;
        }
        public bool CheckUserIsMarketing(string empATID)
        {
            string role = GetUserRoleName(empATID);
            if (role != "" && _listMarketingGroup.Contains(role))
            {
                return true;
            }

            return false;
        }

        public bool CheckUserIsAcademicAffairs(string empATID)
        {
            string role = GetUserRoleName(empATID);
            if (role != "" && _listAcademicAffairsGroup.Contains(role))
            {
                return true;
            }

            return false;
        }
        public async Task<bool> CheckUserIsAdmissionAsync(string empATID)
        {
            if (string.IsNullOrWhiteSpace(empATID))
            {
                return false;
            }

            return CheckUserIsAdmission(empATID);
        }

        public async Task<bool> CheckUserIsMarketingAsync(string empATID)
        {
            if (string.IsNullOrWhiteSpace(empATID))
            {
                return false;
            }

            return CheckUserIsMarketing(empATID);
        }

        public async Task<bool> CheckUserIsAcademicAffairsAsync(string empATID)
        {
            if (string.IsNullOrWhiteSpace(empATID))
            {
                return false;
            }

            return CheckUserIsAcademicAffairs(empATID);
        }

        public async Task<List<string>> GetListEmployeeATIDByFormNameAndAction(string formName, PermissionAction action)
        {
            var scope = _serviceProvider.CreateScope();
            IRegalEducationDbContext? context = scope.ServiceProvider.GetService<IRegalEducationDbContext>();
            if (context == null)
            {
                throw new InvalidOperationException("Failed to resolve IRegalEducationDbContext from the service provider.");
            }
            List<Domain.Entities.AccountGroupPermission> listPermission = await context.AccountGroupPermissions.Where(t => t.FormName == formName
                && t.Action == action.ToString() && t.AllowAction == true)
                .AsNoTracking().ToListAsync();
            List<Guid> listGroupId = listPermission.Select(t => t.AccountGroupId).Distinct().ToList();
            List<Domain.Entities.AccountGroupEmployee> listEmployee = await context.AccountGroupEmployees.Where(t => listGroupId.Contains(t.AccountGroupId)).AsNoTracking().ToListAsync();
            List<string> listEmployeeATID = listEmployee.Select(t => t.UserCode).ToList();

            return listEmployeeATID;
        }

        public async Task<List<Guid>> GetUserIdsByAccountGroupAsync(Guid accountGroupId)
        {
            var scope = _serviceProvider.CreateScope();
            IRegalEducationDbContext? context = scope.ServiceProvider.GetService<IRegalEducationDbContext>();
            if (context == null)
            {
                throw new InvalidOperationException("Failed to resolve IRegalEducationDbContext from the service provider.");
            }

            var userIds = await context.AccountGroupEmployees
                .AsNoTracking()
                .Where(t => t.AccountGroupId == accountGroupId)
                .Join(context.ApplicationUsers,
                    age => age.UserCode,
                    au => au.UserCode,
                    (age, au) => au.Id)
                .Distinct()
                .ToListAsync();

            return userIds;
        }

        public async Task<List<Guid>> GetAdmissionUserIdsAsync()
        {
            var scope = _serviceProvider.CreateScope();
            IRegalEducationDbContext? context = scope.ServiceProvider.GetService<IRegalEducationDbContext>();
            if (context == null)
            {
                throw new InvalidOperationException("Failed to resolve IRegalEducationDbContext from the service provider.");
            }

            // Lấy các nhóm có tên nằm trong danh sách AdmissionGroup cấu hình
            var admissionGroupIds = await context.AccountGroups
                .Where(g => _listAdmissionGroup.Contains(g.Name))
                .Select(g => g.Id)
                .ToListAsync();

            if (admissionGroupIds.Count == 0)
            {
                return new List<Guid>();
            }

            var userIds = await context.AccountGroupEmployees
               .AsNoTracking()
                .Where(t => admissionGroupIds.Contains(t.AccountGroupId))
                .Join(context.ApplicationUsers,
                    age => age.UserCode,
                    au => au.UserCode,
                    (age, au) => au.Id)
                .Distinct()

                .ToListAsync();

            return userIds;
        }

        public async Task<List<Guid>> GetMarketingUserIdsAsync()
        {
            var scope = _serviceProvider.CreateScope();
            IRegalEducationDbContext? context = scope.ServiceProvider.GetService<IRegalEducationDbContext>();
            if (context == null)
            {
                throw new InvalidOperationException("Failed to resolve IRegalEducationDbContext from the service provider.");
            }

            var marketingGroupIds = await context.AccountGroups
                .Where(g => _listMarketingGroup.Contains(g.Name))
                .Select(g => g.Id)
                .ToListAsync();

            if (marketingGroupIds.Count == 0)
            {
                return new List<Guid>();
            }

            var userIds = await context.AccountGroupEmployees
                .AsNoTracking()
                .Where(t => marketingGroupIds.Contains(t.AccountGroupId))
                .Join(context.ApplicationUsers,
                    age => age.UserCode,
                    au => au.UserCode,
                    (age, au) => au.Id)
                .Distinct()
                .ToListAsync();

            return userIds;
        }


    }
}
