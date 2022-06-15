namespace MovieDbApi.Common.Maintenance.Logging.Abstract
{
    public interface ILoggerService
    {
        void Log(string message);
        void Log(string message, Exception exception);
        void Log(Exception exception);
    }
}
