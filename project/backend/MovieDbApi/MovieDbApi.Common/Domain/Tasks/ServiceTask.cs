using MovieDbApi.Common.Domain.Utility;

namespace MovieDbApi.Common.Domain.Tasks
{
    public class ServiceTask
        : IDisposable
    {
        public ServiceTask(DateTime startTime, TimeSpan delay, Action<CancellationToken> taskProcedure)
        {
            StartTime = startTime;
            Delay = delay;
            TaskProcedure = taskProcedure;

            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;

            Task = Task.Factory.StartNew(ServiceProcedure, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        private Task Task { get; set; }

        private CancellationTokenSource CancellationTokenSource { get; }
        
        private CancellationToken CancellationToken { get; }

        private DateTime StartTime { get; }
        
        private TimeSpan Delay { get; }

        private Action<CancellationToken> TaskProcedure { get; }

        public void Dispose()
        {
            Extensions.SafeDispose(CancellationTokenSource);
        }

        public void Stop()
        {
            CancellationTokenSource.Cancel();
        }

        private async Task ServiceProcedure()
        {
            TimeSpan firstRunDelay = StartTime.AddDays(1) - DateTime.UtcNow;

            await Task.Delay(firstRunDelay, CancellationToken);

            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    TaskProcedure(CancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                await Task.Delay(Delay, CancellationToken);
            }
        }
    }
}
