using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList.Models
{
    public class Status
    {
        [JsonProperty("watching")]
        public string Watching { get; set; }

        [JsonProperty("completed")]
        public string Completed { get; set; }

        [JsonProperty("on_hold")]
        public string OnHold { get; set; }

        [JsonProperty("dropped")]
        public string Dropped { get; set; }

        [JsonProperty("plan_to_watch")]
        public string PlanToWatch { get; set; }
    }
}
