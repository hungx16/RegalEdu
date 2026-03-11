
namespace RegalEdu.Domain.Entities
{
    public class AccountGroupEmployee : BaseEntity
    {
        public Guid AccountGroupId { get; set; }
        public AccountGroup? AccountGroup { get; set; }
        public string UserCode { get; set; } = string.Empty;

        public bool IsApprover { get; set; }
    }
}
