namespace RegalEdu.Application.Common.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        Task EnqueueAsync(Func<Task> workItem);
        Task<Func<Task>> DequeueAsync(CancellationToken cancellationToken);

    }

}
