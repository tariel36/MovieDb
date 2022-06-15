using MovieDbApi.Common.Maintenance.Logging.Abstract;

namespace MovieDbApi.Common.Maintenance.Logging.Specific
{
    public class ConsoleLoggerSink
        : ILoggerSink
    {
        public void Dispose()
        {
            
        }

        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}
