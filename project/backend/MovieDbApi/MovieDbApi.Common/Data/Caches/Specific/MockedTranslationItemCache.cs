using MovieDbApi.Common.Data.Caches.Abstract;

namespace MovieDbApi.Common.Data.Caches.Specific
{
    public class MockedTranslationItemCache
        : ITranslationItemCache
    {
        public void Dispose()
        {
            
        }

        public string GetTranslation(string language, string key)
        {
            return key;
        }

        public void Initialize()
        {
            
        }

        public void Set(string to, string value, string translation)
        {
            
        }

        public void Update()
        {
            
        }
    }
}
