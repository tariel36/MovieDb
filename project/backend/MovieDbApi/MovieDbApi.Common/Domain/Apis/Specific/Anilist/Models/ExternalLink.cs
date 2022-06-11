using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.Anilist.Models
{
    public class ExternalLink
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
