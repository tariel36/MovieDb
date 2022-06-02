using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieDbApi.Common.Domain.Crawling.Services;
using MovieDbApi.Common.Domain.Media.Services.Specific;
using MovieDbApi.Common.Domain.Notifications.Abstract;
using MovieDbApi.Common.Domain.Utility;

namespace MovieDbApi.Common.Domain.Tasks
{
    public class ServicesContainer
    {
        private readonly MediaMonitor _mediaMonitor;
        private readonly INotificationService _notificationService;

        public ServicesContainer(INotificationService notificationService,
            MediaMonitor mediaMonitor)
        {
            Tasks = new List<ServiceTask>();

            _notificationService = notificationService;
            _mediaMonitor = mediaMonitor;
        }

        private List<ServiceTask> Tasks { get; }

        public void Start()
        {
            Tasks.Add(new ServiceTask(
                DateTime.UtcNow.Date.AddHours(1),
                TimeSpan.FromHours(24),
                (ct) => _mediaMonitor.Work())
            );

            Tasks.Add(new ServiceTask(
                DateTime.UtcNow.Date.AddHours(2),
                TimeSpan.FromHours(24),
                (ct) => _notificationService.Work())
            );
        }

        public void Stop()
        {
            foreach (ServiceTask task in Tasks)
            {
                task.Stop();
                Extensions.SafeDispose(task);
            }
        }
    }
}
