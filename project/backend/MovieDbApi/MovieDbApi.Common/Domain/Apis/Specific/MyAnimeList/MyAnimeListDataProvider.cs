using System.Linq;
using MovieDbApi.Common.Domain.Apis.Abstract;
using MovieDbApi.Common.Domain.Apis.Models;
using MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Utility;

namespace MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList
{
    public class MyAnimeListDataProvider
        : BaseMediaDataProvider
    {
        public MyAnimeListDataProvider(string apiKey)
            : base(1)
        {
            ApiKey = apiKey;
        }

        private string ApiKey { get; }

        public override ApiMediaItemDetails SearchDetailsByTitle(string title)
        {
            string url = $"https://api.myanimelist.net/v2/anime?q={title}";

            MalSearchResult searchResult = Get<MalSearchResult>(url, $"X-MAL-CLIENT-ID: {ApiKey}");
            Node searchItem = searchResult.Data.FirstOrDefault(x => string.Equals(title, CommonRegex.InvalidPathChars.Replace(x.Node.Title, string.Empty)))?.Node;

            if (searchItem == null)
            {
                return null;
            }

            url = $"https://api.myanimelist.net/v2/anime/{searchItem.Id}?fields='id,title,main_picture,alternative_titles,start_date,end_date,synopsis,mean,rank,popularity,nsfw,created_at,updated_at,media_type,status,genres,num_episodes,start_season,broadcast,source,average_episode_duration,rating,pictures,background,related_animestudios,statistics'";
            MalDetailsItem details = Get<MalDetailsItem>(url, $"X-MAL-CLIENT-ID: {ApiKey}");

            return new ApiMediaItemDetails
            {
                Duration = TimeSpan.FromSeconds(details.NumEpisodes * details.AverageEpisodeDuration).TotalMinutes.ToString(),
                DurationPerEpisode = TimeSpan.FromSeconds(details.AverageEpisodeDuration).TotalMinutes.ToString("#.##").Replace(',', '.'),
                ExternalId = details.Id.ToString(),
                Genre = details.Genres.Select(x => x.Name).Join(", "),
                Plot = details.Synopsis,
                Poster = details.MainPicture?.Large,
                Rated = details.Rating,
                Rating = details.Mean.ToString("#.##").Replace(',', '.'),
                ReleaseDate = details.StartDate,
                Staff = string.Empty,
                Title = details.Title,
                Titles = details.AlternativeTitles == null
                    ? null
                    : new [] { details.AlternativeTitles.En, details.AlternativeTitles.Ja }
                        .Concat(details.AlternativeTitles.Synonyms?.Select(x => x.ToString()).ToArray() ?? new string[0])
                        .Distinct()
                        .ToList()
                    ,
                Type = details.MediaType,
                Url = $"https://myanimelist.net/anime/{details.Id}/",
                Year = details.StartDate?.Split('-').FirstOrDefault(),
                ApiSource = nameof(MyAnimeListDataProvider),
                MediaType = MediaItemType.Anime
            };
        }

        public override SearchResult SearchByTitle(string title)
        {
            string url = $"https://api.myanimelist.net/v2/anime?q={title}";

            MalSearchResult result = Get<MalSearchResult>(url, $"X-MAL-CLIENT-ID: {ApiKey}");

            return new SearchResult()
            {
                Search = result.Data.Select(x => x.Node).Select(x => new ApiMediaItem()
                {
                    Title = x.Title,
                    ExternalId = x.Id.ToString(),
                    Poster = x.MainPicture?.Large ?? x.MainPicture?.Medium,
                    Type = "Anime",
                    ApiSource = nameof(MyAnimeListDataProvider)
                })
                .ToList()
            };
        }
    }
}
