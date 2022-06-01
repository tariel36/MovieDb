using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList.Models
{
    public class MalSearchResult
    {
        [JsonProperty("data")]
        public List<Datum> Data { get; set; }

        [JsonProperty("paging")]
        public Paging Paging { get; set; }
    }
}
