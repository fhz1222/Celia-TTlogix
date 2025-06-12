using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using TT.Services.Interfaces;
using TT.Services.Utilities;

namespace TT.Services.Services
{
    public class LockRemovalService : IHostedService
    {
        private readonly int idleWaitTimeMs = 5000;
        private readonly int interval = 60000;
        private readonly CancellationTokenSource stoppingCts;
        private Timer timer;
        private Task executingTask;
        private readonly IServiceScopeFactory serviceScopeFactory;
        public LockRemovalService(IServiceScopeFactory serviceScopeFactory)
        {
            stoppingCts = new();
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var locksService = serviceScopeFactory.CreateScope().ServiceProvider.GetService<ILocksService>();
            await locksService.DeleteAllAccessLocks();
            timer = new Timer(ExecuteTask, null, TimeSpan.FromMilliseconds(idleWaitTimeMs), Timeout.InfiniteTimeSpan);
        }

        private void ExecuteTask(object state)
        {
            timer?.Stop();
            executingTask = ExecuteTaskAsync(stoppingCts.Token);
        }

        private async Task ExecuteTaskAsync(CancellationToken stoppingToken)
        {
            await Task.Factory.StartNew(async () => await DoWork(), stoppingToken);
            timer?.Start(interval);
        }

        private async Task DoWork()
        {
            var locksService = serviceScopeFactory.CreateScope().ServiceProvider.GetService<ILocksService>();
            await locksService.DeletedTimeoutedLocks();
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Stop();

            // Stop called without start
            if (executingTask == null) { return; }

            try
            {
                // Signal cancellation to the executing method
                stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public void Dispose()
        {
            stoppingCts.Cancel();
            timer?.Dispose();
        }
    }
}
