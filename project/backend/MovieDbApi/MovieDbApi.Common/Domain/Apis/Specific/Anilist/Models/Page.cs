using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.Anilist.Models
{
    public class Page
    {
        [JsonProperty("media")]
        public List<PageMediaItem> Media { get; set; }
    }


}
