using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;


namespace RegalEdu.Infrastructure.Services
{
    public class BackgroundWorkerService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<BackgroundWorkerService> _logger;

        public BackgroundWorkerService(IBackgroundTaskQueue taskQueue, ILogger<BackgroundWorkerService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        // Phương thức này sẽ chạy khi ứng dụng bắt đầu, và tiếp tục xử lý công việc nền
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation ("Background worker service is starting.");

            // Tiếp tục chạy cho đến khi ứng dụng yêu cầu dừng
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Lấy công việc từ queue và thực thi nó
                    var workItem = await _taskQueue.DequeueAsync (stoppingToken);
                    if (workItem != null)
                    {
                        // Thực thi công việc
                        await workItem ( );
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError ($"An error occurred while processing a background task: {ex.Message}");
                }
            }

            _logger.LogInformation ("Background worker service is stopping.");
        }
    }


}
