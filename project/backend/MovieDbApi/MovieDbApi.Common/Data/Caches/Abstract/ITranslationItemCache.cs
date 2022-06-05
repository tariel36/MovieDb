namespace MovieDbApi.Common.Data.Caches.Abstract
{
    public interface ITranslationItemCache
        : IDisposable
    {
        void Update();

        string GetTranslation(string language, string key);
        
        void Set(string to, string value, string translation);
    }
}
