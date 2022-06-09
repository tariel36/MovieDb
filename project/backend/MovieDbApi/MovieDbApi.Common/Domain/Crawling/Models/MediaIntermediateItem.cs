using System.Diagnostics;

namespace MovieDbApi.Common.Domain.Crawling.Models
{
    [DebuggerDisplay("{FilePath}")]
    public class MediaIntermediateItem
    {
        public string Directory { get; internal set; }
        public string FilePath { get; internal set; }
        public string MainImage { get; internal set; }
        public List<string> Images { get; internal set; }
        public string Group { get; internal set; }
        public MediaType Type { get; internal set; }
    }
}
