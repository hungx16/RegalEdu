using RegalEdu.Application.Common.Results;

namespace RegalEdu.Application.Common.Interfaces
{
    public interface ISoftDeleteService
    {
        //Task ScheduleRecursiveSoftDelete(Guid id, Type entityType);
        Task<Result> RecursiveSoftDelete(Guid id, Type entityType);
    }

}
