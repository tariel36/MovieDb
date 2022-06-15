using Microsoft.Extensions.DependencyInjection;
using MovieDbApi.Common.Domain.Utility;
using MovieDbApi.Common.Maintenance.Logging.Abstract;

namespace MovieDbApi.Common.Maintenance.Logging.Specific
{
    public class LoggerService
        : ILoggerService
        , IDisposable
    {
        private readonly ICollection<ILoggerSink> _sinks;
        private readonly IServiceScope _scope;

        public LoggerService(ILoggerSink[] sinks)
        {
            _sinks = sinks;
        }

        public LoggerService(IServiceScopeFactory serviceScopeFactory)
        {
            _scope = serviceScopeFactory.CreateScope();
            _sinks = _scope.ServiceProvider.GetServices<ILoggerSink>().ToList();
        }
        
        public void Dispose()
        {
            _sinks.ForEach(Extensions.SafeDispose);
        }

        public void Log(string message)
        {
            foreach (ILoggerSink sink in _sinks)
            {
                try
                {
                    sink.Write(message);
                }
                catch
                {
                    // Ignored
                }
            }
        }

        public void Log(string message, Exception exception)
        {
            Log(message);
            Log(exception.ToString());
        }

        public void Log(Exception exception)
        {
            Log(exception.ToString());
        }
    }
}
