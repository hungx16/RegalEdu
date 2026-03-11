namespace RegalEdu.Domain.Models.Request
{
    public class AccountGroupEmployeeRequestModel
    {
        public Guid AccountGroupId { get; set; }
        public required List<string> ListUserCode { get; set; }

        public required List<bool> ListIsApprover { get; set; }
    }
}
