namespace RegalEdu.Domain.Models
{
    public class AccountGroupPermissionModel: BaseEntityModel
    {
        public virtual AccountGroupModel? AccountGroup { get; set; }
        public required string FormName { get; set; }
        public required string Action { get; set; }
        public bool AllowAction { get; set; }
    }
}
