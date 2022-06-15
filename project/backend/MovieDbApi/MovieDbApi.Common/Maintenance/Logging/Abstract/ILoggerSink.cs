namespace MovieDbApi.Common.Maintenance.Logging.Abstract
{
    public interface ILoggerSink
        : IDisposable
    {
        void Write(string message);
    }
}
