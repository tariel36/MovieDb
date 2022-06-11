using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.Anilist.Models
{
    public class DetailsData
    {
        [JsonProperty("Media")]
        public Media Media { get; set; }
    }
}
