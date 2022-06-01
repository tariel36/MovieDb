using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList.Models
{
    public class StartSeason
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("season")]
        public string Season { get; set; }
    }
}
