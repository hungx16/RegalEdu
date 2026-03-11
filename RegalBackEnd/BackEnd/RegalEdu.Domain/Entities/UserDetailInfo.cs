namespace RegalEdu.Domain.Entities
{
    public class UserDetailInfo : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public Guid? AccountGroupId { get; set; }   
        public virtual AccountGroup? AccountGroup { get; set; }
    }
}
