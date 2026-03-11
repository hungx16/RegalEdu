
using RegalEdu.Application.Common.Interfaces;
using System.Threading.Channels;

namespace RegalEdu.Infrastructure.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<Task>> _queue;

        public BackgroundTaskQueue( )
        {
            _queue = Channel.CreateUnbounded<Func<Task>> ( );
        }

        public async Task<Func<Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            var workItem = await _queue.Reader.ReadAsync (cancellationToken); // Đọc công việc từ hàng đợi
            return workItem;
        }

        public async Task EnqueueAsync(Func<Task> workItem)
        {
            await _queue.Writer.WriteAsync (workItem);
        }

        public async Task ExecuteAsync( )
        {
            await foreach (var workItem in _queue.Reader.ReadAllAsync ( ))
            {
                await workItem ( );
            }
        }
    }

}
