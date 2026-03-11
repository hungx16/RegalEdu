namespace RegalEdu.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string UserCode { get; }
        string UserName { get; }

        string EmployeeId { get; }

        string TeacherId { get; }
        // Thiện 
        string? GetCurrentUserId();
        Guid? GetCurrentUserIdAsGuid();
        bool IsAuthenticated { get; }
    }
}
