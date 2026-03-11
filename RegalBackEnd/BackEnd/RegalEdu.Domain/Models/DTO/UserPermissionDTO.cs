
namespace RegalEdu.Domain.Models.DTO
{
    public class UserPermissionDTO
    {


        public AccountGroupModel? GroupData { get; set; }
        public List<AccountGroupPermissionModel>? ListPermissions { get; set; }
        public List<string>? ListUserCode { get; set; }
    }
}

