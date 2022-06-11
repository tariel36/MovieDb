using MovieDbApi.Common.Domain.Media.Models.Data;

namespace MovieDbApi.Common.Domain.Apis.Models
{
    public class ApiMediaItemDetails
    {
        public string ApiSource { get; set; }

        public string Duration { get; set; }

        public string DurationPerEpisode { get; set; }

        public string ExternalId { get; set; }

        public string Genre { get; set; }

        public string Plot { get; set; }

        public string Poster { get; set; }

        public string Rated { get; set; }

        public string Rating { get; set; }

        public string ReleaseDate { get; set; }

        public string Staff { get; set; }

        public string Title { get; set; }

        public List<string> Titles { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }

        public string Year { get; set; }

        public List<string> Links { get; set; }

        public MediaItemType MediaType { get; set; }
    }
}
