using Microsoft.Extensions.DependencyInjection;
using MovieDbApi.Common.Data.Caches.Abstract;
using MovieDbApi.Common.Domain.Media.Services.Abstract;
using MovieDbApi.Common.Domain.Notifications.Abstract;
using MovieDbApi.Common.Domain.Utility;
using MovieDbApi.Common.Maintenance.Logging.Abstract;

namespace MovieDbApi.Common.Domain.Tasks
{
    public class ServicesContainer
        : IDisposable
    {
        private readonly IMediaMonitor _mediaMonitor;
        private readonly INotificationService _notificationService;
        private readonly ILoggerService _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScope _scope;
        private readonly ITranslationItemCache _traslationCache;

        public ServicesContainer(ILoggerService logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;

            _serviceScopeFactory = serviceScopeFactory;
            _scope = _serviceScopeFactory.CreateScope();

            _notificationService = _scope.ServiceProvider.GetService<INotificationService>();
            _mediaMonitor = _scope.ServiceProvider.GetService<IMediaMonitor>();
            _traslationCache = _scope.ServiceProvider.GetService<ITranslationItemCache>();

            Tasks = new List<ServiceTask>();
        }

        private List<ServiceTask> Tasks { get; }

        public void Start()
        {
            Tasks.Add(new ServiceTask(_logger,
                DateTime.UtcNow.Date.AddHours(1),
                TimeSpan.FromHours(24),
                (ct) => _mediaMonitor.Work()));
            
            Tasks.Add(new ServiceTask(_logger,
                DateTime.UtcNow.Date.AddHours(2),
                TimeSpan.FromHours(24),
                (ct) => _notificationService.Work()));
            
            Tasks.Add(new ServiceTask(_logger,
                DateTime.UtcNow.Date.AddHours(3),
                TimeSpan.FromHours(24),
                (ct) => _traslationCache.Update(),
                (ct) => _traslationCache.Initialize()));
        }

        public void Stop()
        {
            foreach (ServiceTask task in Tasks)
            {
                task.Stop();
                Extensions.SafeDispose(task);
            }
        }

        public void Execute(string what)
        {
            switch (what)
            {
                case nameof(IMediaMonitor):
                {
                    Task.Factory.StartNew(() => _mediaMonitor.Work());
                    break;
                }
            }
        }

        public void Dispose()
        {
            Extensions.SafeDispose(_scope);
        }
    }
}
