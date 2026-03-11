namespace RegalEdu.Domain.Entities
{
    public class AccountGroupPermission : BaseEntity
    {
        public Guid AccountGroupId { get; set; }
        public virtual AccountGroup? AccountGroup { get; set; }
        public string FormName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public bool AllowAction { get; set; }
        
    }
}
