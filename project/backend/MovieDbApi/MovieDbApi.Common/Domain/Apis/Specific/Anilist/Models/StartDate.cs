using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.Anilist.Models
{
    public class StartDate
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("day")]
        public int Day { get; set; }
    }


}
