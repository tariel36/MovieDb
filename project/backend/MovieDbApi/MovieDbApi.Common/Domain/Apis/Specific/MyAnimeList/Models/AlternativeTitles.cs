using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList.Models
{
    public class AlternativeTitles
    {
        [JsonProperty("synonyms")]
        public List<object> Synonyms { get; set; }

        [JsonProperty("en")]
        public string En { get; set; }

        [JsonProperty("ja")]
        public string Ja { get; set; }
    }
}
