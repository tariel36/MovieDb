using System.Diagnostics;

namespace MovieDbApi.Common.Domain.Apis.Models
{
    [DebuggerDisplay("{Title}")]
    public class ApiMediaItem
    {
        public string Title { get; set; }

        public string Year { get; set; }

        public string ExternalId { get; set; }

        public string Type { get; set; }

        public string Poster { get; set; }

        public string ApiSource { get; set; }
    }
}
