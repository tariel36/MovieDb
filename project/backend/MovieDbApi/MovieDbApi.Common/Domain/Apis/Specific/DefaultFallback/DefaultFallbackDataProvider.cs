using MovieDbApi.Common.Domain.Apis.Abstract;
using MovieDbApi.Common.Domain.Apis.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Maintenance.Logging.Abstract;

namespace MovieDbApi.Common.Domain.Apis.Specific.DefaultFallback
{
    public class DefaultFallbackDataProvider
        : BaseMediaDataProvider
    {
        public DefaultFallbackDataProvider(ILoggerService logger)
            : base(logger, 999999, Enum.GetValues<MediaItemType>())
        {

        }

        public override ApiMediaItemDetails GetByUrl(string url)
        {
            return null;
        }

        public override SearchResult SearchByTitle(string title)
        {
            return new SearchResult
            {
                Search = new List<ApiMediaItem>()
                {
                    new ApiMediaItem
                    {
                        ApiSource = nameof(DefaultFallbackDataProvider),
                        Title = title
                    }
                }
            };
        }

        public override ApiMediaItemDetails SearchDetailsByTitle(string title)
        {
            return new ApiMediaItemDetails
            {
                ApiSource = nameof(DefaultFallbackDataProvider),
                Title = title
            };
        }
    }
}
