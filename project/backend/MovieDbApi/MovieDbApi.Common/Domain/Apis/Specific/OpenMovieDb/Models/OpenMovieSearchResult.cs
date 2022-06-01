using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.OpenMovieDb.Models
{
    public class OpenMovieSearchResult
    {
        [JsonProperty("Search")]
        public List<OpenMediaItem> Search { get; set; }

        [JsonProperty("totalResults")]
        public string TotalResults { get; set; }

        [JsonProperty("Response")]
        public string Response { get; set; }
    }
}
