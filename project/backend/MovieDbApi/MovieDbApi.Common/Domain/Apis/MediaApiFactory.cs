using System.Linq;
using Microsoft.Extensions.Configuration;
using MovieDbApi.Common.Domain.Apis.Abstract;
using MovieDbApi.Common.Domain.Apis.Specific.Anilist;
using MovieDbApi.Common.Domain.Apis.Specific.DefaultFallback;
using MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList;
using MovieDbApi.Common.Domain.Apis.Specific.OpenMovieDb;
using MovieDbApi.Common.Maintenance;
using MovieDbApi.Common.Maintenance.Logging.Abstract;

namespace MovieDbApi.Common.Domain.Apis
{
    public class MediaApiFactory
    {
        public List<IMediaDataProvider> GetAllApis(ILoggerService logger,
            IConfiguration configuration)
        {
            return configuration.GetSection(ConfigurationKeys.ApiKeys)
                ?.AsEnumerable()
                .Select<KeyValuePair<string, string>, IMediaDataProvider>(x => x.Key switch
                {
                    ConfigurationKeys.ApiKeysAnilist => new AniListDataProvider(logger, x.Value),
                    ConfigurationKeys.Anilist => new AniListDataProvider(logger, x.Value),
                    ConfigurationKeys.ApiKeysMyAnimeList => new MyAnimeListDataProvider(logger, x.Value),
                    ConfigurationKeys.MyAnimeList => new MyAnimeListDataProvider(logger, x.Value),
                    ConfigurationKeys.ApiKeysOpenMovieDb => new OpenMovieDbDataProvider(logger, x.Value),
                    ConfigurationKeys.OpenMovieDb => new OpenMovieDbDataProvider(logger, x.Value),
                    _ => null
                })
                .Where(x => x != null)
                .Concat(new IMediaDataProvider[] { new DefaultFallbackDataProvider(logger) })
                .OrderBy(x => x.Order)
                .ToList();
        }
    }
} 
