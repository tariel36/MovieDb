using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.Anilist.Models
{
    public class CoverImage
    {
        [JsonProperty("extraLarge")]
        public string ExtraLarge { get; set; }

        [JsonProperty("large")]
        public string Large { get; set; }

        [JsonProperty("medium")]
        public string Medium { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }
    }

}
