using Microsoft.Extensions.DependencyInjection;
using MovieDbApi.Common.Data.Caches.Abstract;
using MovieDbApi.Common.Data.Specific;
using MovieDbApi.Common.Domain.Utility;

namespace MovieDbApi.Common.Data.Caches.Specific
{
    public class TranslationItemCache
        : ITranslationItemCache
    {
        private readonly object _updateLock = new object();

        private readonly MediaContext _mediaContext;
        private readonly IServiceScope _scope;

        private Dictionary<string, Dictionary<string, string>> _translations;

        public TranslationItemCache(IServiceScopeFactory serviceScopeFactory)
        {
            _scope = serviceScopeFactory.CreateScope();
            _mediaContext = _scope.ServiceProvider.GetService<MediaContext>();
        }

        public void Dispose()
        {
            Extensions.SafeDispose(_mediaContext);
        }

        public string GetTranslation(string language, string key)
        {
            return _translations.TryGetValue(language, out Dictionary<string, string> dict)
                && dict.TryGetValue(key, out string value)
                ? dict[key]
                : String.Empty;
        }

        public void Set(string to, string value, string translation)
        {
            KeyValuePair<string, Dictionary<string, string>> newKv = _translations.EnsureKey(to, () => new Dictionary<string, string>());
            newKv.Value[value] = translation;
        }

        public void Update()
        {
            lock (_updateLock)
            {
                _translations = _mediaContext.TranslationCache
                    .AsEnumerable()
                    .GroupBy(x => x.Language, StringComparer.InvariantCultureIgnoreCase)
                    .ToDictionary(k => k.Key,
                        v => v.ToDictionary(kk => kk.Source, vv => vv.Target,
                        StringComparer.InvariantCultureIgnoreCase)
                    );
            }
        }
    }
}
