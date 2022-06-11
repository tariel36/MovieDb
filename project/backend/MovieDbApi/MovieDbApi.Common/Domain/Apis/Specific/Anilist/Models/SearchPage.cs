using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.Anilist.Models
{
    public class SearchPage
    {
        [JsonProperty("data")]
        public PageData Data { get; set; }
    }
}
