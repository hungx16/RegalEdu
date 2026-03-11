namespace RegalEdu.Domain.Entities
{
    public class AccountGroupUser : BaseEntity
    {
        public Guid AccountGroupId { get; set; }
        public AccountGroup? AccountGroup { get; set; }
        public string UserCode { get; set; } = string.Empty;

    }
}
