namespace RegalEdu.Domain.Models
{
    public class AccountGroupModel : BaseEntityModel
    {
        public string Name { get; set; } = string.Empty;
        public bool Enable { get; set; }
        public bool UseDefault { get; set; }
        public string? Note { get; set; }

        public virtual ICollection<AccountGroupPermissionModel>? UserGroupPermissions { get; set; }
        public virtual ICollection<UserDetailInfoModel>? UserDetailInfos { get; set; }
    }
}
