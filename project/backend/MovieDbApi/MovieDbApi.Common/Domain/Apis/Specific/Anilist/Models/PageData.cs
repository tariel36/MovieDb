using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.Anilist.Models
{
    public class PageData
    {
        [JsonProperty("Page")]
        public Page Page { get; set; }
    }


}
