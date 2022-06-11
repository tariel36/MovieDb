using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.Anilist.Models
{
    public class Title
    {
        [JsonProperty("romaji")]
        public string Romaji { get; set; }

        [JsonProperty("english")]
        public string English { get; set; }

        [JsonProperty("native")]
        public string Native { get; set; }

        [JsonProperty("userPreferred")]
        public string UserPreferred { get; set; }
    }
}
