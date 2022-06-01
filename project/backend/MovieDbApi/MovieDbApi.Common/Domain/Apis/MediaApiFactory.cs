using Microsoft.Extensions.Configuration;
using MovieDbApi.Common.Domain.Apis.Abstract;
using MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList;
using MovieDbApi.Common.Domain.Apis.Specific.OpenMovieDb;
using MovieDbApi.Common.Maintenance;

namespace MovieDbApi.Common.Domain.Apis
{
    public class MediaApiFactory
    {
        public List<IMediaDataProvider> GetAllApis(IConfiguration configuration)
        {
            return configuration.GetSection(ConfigurationKeys.ApiKeys)
                ?.AsEnumerable()
                .Select<KeyValuePair<string, string>, IMediaDataProvider>(x => x.Key switch
                {
                    ConfigurationKeys.ApiKeysMyAnimeList => new MyAnimeListDataProvider(x.Value),
                    ConfigurationKeys.MyAnimeList => new MyAnimeListDataProvider(x.Value),
                    ConfigurationKeys.ApiKeysOpenMovieDb => new OpenMovieDbDataProvider(x.Value),
                    ConfigurationKeys.OpenMovieDb => new OpenMovieDbDataProvider(x.Value),
                    _ => null
                })
                .Where(x => x != null)
                .OrderBy(x => x.Order)
                .ToList();
        }
    }
} 
