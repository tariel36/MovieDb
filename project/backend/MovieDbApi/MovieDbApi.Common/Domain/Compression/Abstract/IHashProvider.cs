namespace MovieDbApi.Common.Domain.Compression.Abstract
{
    public interface IHashProvider
        : IDisposable
    {
        string Get(string value);
    }
}
