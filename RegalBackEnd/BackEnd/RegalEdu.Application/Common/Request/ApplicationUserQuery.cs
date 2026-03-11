using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Common.Request
{
    public class ApplicationUserQuery : ApplicationUserModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class AccountGroupQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Name { get; set; }
        public bool? Enable { get; set; }
        public bool? UseDefault { get; set; }
    }
}