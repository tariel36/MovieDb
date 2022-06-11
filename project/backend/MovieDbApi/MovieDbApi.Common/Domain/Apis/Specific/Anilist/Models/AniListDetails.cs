using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.Anilist.Models
{
    public class AniListDetails
    {
        [JsonProperty("data")]
        public DetailsData Data { get; set; }
    }
}
