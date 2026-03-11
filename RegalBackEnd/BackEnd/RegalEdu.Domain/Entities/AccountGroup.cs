namespace RegalEdu.Domain.Entities
{
    public class AccountGroup : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool Enable { get; set; }
        public bool UseDefault { get; set; }
        public string? Note { get; set; }

        public virtual ICollection<AccountGroupPermission>? UserGroupPermissions { get; set; }
        public virtual ICollection<UserDetailInfo>? UserDetailInfos { get; set; }
    }
}
