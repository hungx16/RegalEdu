namespace RegalEdu.Domain.Models.Request
{
    public class AccountGroupPermissionRequestModel
    {
        public Guid AccountGroupId { get; set; }
        public required List<AccountGroupPermissionModel> ListGroupPermission { get; set; }
    }
}
