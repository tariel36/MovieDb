using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Models
{
    public sealed class GqlTuple
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("variables")]
        public Dictionary<string, string> Variables { get; set; }
    }
}
