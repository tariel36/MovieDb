using MovieDbApi.Common.Domain.Utility;
using MovieDbApi.Common.Domain.Apis.Abstract;
using MovieDbApi.Common.Domain.Apis.Models;
using MovieDbApi.Common.Domain.Apis.Specific.OpenMovieDb.Models;
using MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList;
using MovieDbApi.Common.Domain.Crawling.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;

namespace MovieDbApi.Common.Domain.Apis.Specific.OpenMovieDb
{
    public class OpenMovieDbDataProvider
        : BaseMediaDataProvider
    {
        public OpenMovieDbDataProvider(string apiKey)
            : base(99, new[] { MediaItemType.Cartoon, MediaItemType.Movie, MediaItemType.Series })
        {
            ApiKey = apiKey;
        }

        private string ApiKey { get; }

        public override ApiMediaItemDetails GetByUrl(string url)
        {
            return null;
        }

        public override ApiMediaItemDetails SearchDetailsByTitle(string title)
        {
            string url = $"http://www.omdbapi.com/?apikey={ApiKey}&s={title}";

            OpenMovieSearchResult searchResult = Get<OpenMovieSearchResult>(url);

            OpenMediaItem searchItem = searchResult == null || !bool.TryParse(searchResult.Response, out bool response) || !response
                ? null
                : searchResult.Search.FirstOrDefault(x => string.Equals(title, x.Title))
                ;

            if (searchItem == null)
            {
                return null;
            }

            url = $"http://www.omdbapi.com/?apikey={ApiKey}&i={searchItem.ImdbID}";

            OpenMediaDetailsItem details = Get<OpenMediaDetailsItem>(url);

            MediaItemType mediaType = MediaItemType.Unknown;

            if ((details.Genre ?? string.Empty).Split(',').Select(x => x.ToLower()).Contains("animation"))
            {
                mediaType = MediaItemType.Cartoon;
            }
            else if (string.Equals(details.Type, "movie"))
            {
                mediaType = MediaItemType.Movie;
            }
            else if (string.Equals(details.Type, "series"))
            {
                mediaType = MediaItemType.Series;
            }

            return new ApiMediaItemDetails
            {
                Duration = details.Runtime.Split(' ').FirstOrDefault() ?? string.Empty,
                DurationPerEpisode = details.Runtime.Split(' ').FirstOrDefault() ?? string.Empty,
                ExternalId = details.ImdbID,
                Genre = details.Genre,
                Plot = details.Plot,
                Poster = details.Poster,
                Rated = details.Rated,
                Rating = details.ImdbRating,
                ReleaseDate = details.Released,
                Staff = new [] { details.Director, details.Writer }.Concat(details.Actors.Split(',')).Select(x => x.Trim()).Join(", "),
                Title = details.Title,
                Type = details.Type,
                Url = $"https://www.imdb.com/title/{details.ImdbID}/",
                Year = details.Year,
                ApiSource = nameof(OpenMovieDbDataProvider),
                MediaType = mediaType
            };
        }

        public override SearchResult SearchByTitle(string title)
        {
            string url = $"http://www.omdbapi.com/?apikey={ApiKey}&s={title}";

            OpenMovieSearchResult result = Get<OpenMovieSearchResult>(url);

            return new SearchResult()
            {
                Search = result.Search.Select(x => new ApiMediaItem()
                {
                    ExternalId = x.ImdbID,
                    Title = x.Title,
                    Poster = x.Poster,
                    Type = x.Type,
                    Year = x.Year,
                    ApiSource = nameof(OpenMovieDbDataProvider)
                })
                .ToList()
            };
        }
    }
}
