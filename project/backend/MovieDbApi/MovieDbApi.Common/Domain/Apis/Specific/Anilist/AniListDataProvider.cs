using MovieDbApi.Common.Domain.Apis.Abstract;
using MovieDbApi.Common.Domain.Apis.Models;
using MovieDbApi.Common.Domain.Apis.Specific.Anilist.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Utility;
using MovieDbApi.Common.Maintenance.Logging.Abstract;

namespace MovieDbApi.Common.Domain.Apis.Specific.Anilist
{
    public class AniListDataProvider
        : BaseMediaDataProvider
    {
        public AniListDataProvider(ILoggerService logger,
            string apiKey)
            : base(logger, 2, new[] { MediaItemType.Anime })
        {
            ApiKey = apiKey;
        }

        private string ApiKey { get; }

        public override ApiMediaItemDetails GetByUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            string[] split = url.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            string id = split.LastOrDefault();

            if (!int.TryParse(id, out _))
            {
                id = split[^2];
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            return GetDetails(id);
        }

        public override ApiMediaItemDetails SearchDetailsByTitle(string title)
        {
            ApiMediaItem item = SearchByTitle(title).Search.FirstOrDefault(x => string.Equals(x.Title, title, StringComparison.InvariantCultureIgnoreCase));

            return item == null
                ? null
                : GetDetails(item.ExternalId);
        }

        public override SearchResult SearchByTitle(string title)
        {
            GqlTuple gql = new GqlTuple
            {
                Query = @"
                query($title: String)
                {
                  Page(page: 1, perPage: 50) {
                    media(search: $title, type: ANIME) {
                      id title { romaji english native }
                    }
                  }
                }
                ",
                Variables = new Dictionary<string, string>
                {
                    { "title", title }
                }
            };

            string url = $"https://graphql.anilist.co";
            List<PageMediaItem> items = Post<SearchPage>(url, gql, "Content-Type: application/json", "Accept: application/json")
                ?.Data
                ?.Page
                ?.Media ?? new List<PageMediaItem>();

            return new SearchResult()
            {
                Search = items.Select(x => new ApiMediaItem
                {
                    Title = x.Title.Romaji,
                    ExternalId = x.Id.ToString(),
                    Poster = string.Empty,
                    Type = "Anime",
                    ApiSource = nameof(AniListDataProvider)
                })
                .ToList()
            };
        }

        private ApiMediaItemDetails GetDetails(string id)
        {
            GqlTuple gql = new GqlTuple
            {
                Query = @"
                query ($id: Int)
                {
                  Media(id: $id, type: ANIME) {
                    id idMal season seasonYear
                    type format status episodes duration
                    chapters volumes isAdult averageScore
                    popularity source countryOfOrigin synonyms genres
                    startDate { year month day }
                    endDate { year month day }
                    coverImage { extraLarge large medium }
                    externalLinks { url }
                    description(asHtml: false)
                    title { romaji english native }
                    tags { name }
                  }
                }
                ",
                Variables = new Dictionary<string, string>
                {
                    { "id", id.ToString() }
                }
            };

            string url = $"https://graphql.anilist.co";
            Models.Media details = Post<AniListDetails>(url, gql, "Content-Type: application/json", "Accept: application/json")?.Data?.Media;

            if (details == null)
            {
                return null;
            }

            return new ApiMediaItemDetails
            {
                Duration = TimeSpan.FromMinutes(details.Episodes * details.Duration).TotalMinutes.ToString(),
                DurationPerEpisode = TimeSpan.FromMinutes(details.Duration).TotalMinutes.ToString("#.##").Replace(',', '.'),
                ExternalId = details.Id.ToString(),
                Genre = (details.Genres ?? new List<string>()).Concat((details.Tags ?? new List<Tag>()).Select(x => x.Name)).Join(", ") ?? string.Empty,
                Plot = details.Description,
                Poster = details.CoverImage.ExtraLarge,
                Rated = details.IsAdult ? "R+" : string.Empty,
                Rating = (details.AverageScore / 10.0f).ToString("#.##").Replace(',', '.'),
                ReleaseDate = new DateTime(details.StartDate.Year, details.StartDate.Month, details.StartDate.Day).ToString("yyyy-MM-dd"),
                Staff = string.Empty,
                Title = details.Title.Romaji,
                Titles = new []
                {
                    details.Title.English,
                    details.Title.Native
                }.Concat(details.Synonyms ?? new List<string>()).ToList(),
                Type = details.Format,
                Url = $"https://anilist.co/anime/{details.Id}/",
                Year = details.StartDate.Year.ToString(),
                ApiSource = nameof(AniListDataProvider),
                MediaType = MediaItemType.Anime,
                Links = (details.ExternalLinks ?? new List<ExternalLink>()).Select(x => x.Url).Concat(new [] { $"https://myanimelist.net/anime/{details.IdMal}/" }).ToList()
            };
        }
    }
}
