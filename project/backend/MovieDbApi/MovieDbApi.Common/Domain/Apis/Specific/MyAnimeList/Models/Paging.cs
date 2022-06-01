using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList.Models
{
    public class Paging
    {
        [JsonProperty("next")]
        public string Next { get; set; }
    }
}
