
namespace RegalEdu.Domain.Models
{
    public class AccountGroupEmployeeModel : BaseEntityModel
    {
        public Guid AccountGroupId { get; set; }
        public AccountGroupModel? AccountGroup { get; set; }
        public required string UserCode { get; set; }
        public bool IsApprover { get; set; }

    }
}
