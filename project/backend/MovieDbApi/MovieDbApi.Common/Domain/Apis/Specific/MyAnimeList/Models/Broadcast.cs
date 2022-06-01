using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList.Models
{
    public class Broadcast
    {
        [JsonProperty("day_of_the_week")]
        public string DayOfTheWeek { get; set; }

        [JsonProperty("start_time")]
        public string StartTime { get; set; }
    }
}
