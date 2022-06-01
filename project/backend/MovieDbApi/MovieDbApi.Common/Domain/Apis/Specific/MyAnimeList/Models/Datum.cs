using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList.Models
{
    public class Datum
    {
        [JsonProperty("node")]
        public Node Node { get; set; }
    }
}
