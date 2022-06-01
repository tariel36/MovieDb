using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList.Models
{
    public class MainPicture
    {
        [JsonProperty("medium")]
        public string Medium { get; set; }

        [JsonProperty("large")]
        public string Large { get; set; }
    }
}
