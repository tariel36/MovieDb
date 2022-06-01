using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList.Models
{
    public class Genre
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
