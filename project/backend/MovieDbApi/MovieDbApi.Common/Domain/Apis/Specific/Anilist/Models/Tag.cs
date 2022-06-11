using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.Anilist.Models
{
    public class Tag
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }


}
