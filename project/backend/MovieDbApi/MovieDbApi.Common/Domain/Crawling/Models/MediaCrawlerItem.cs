using System.Diagnostics;

namespace MovieDbApi.Common.Domain.Crawling.Models
{
    [DebuggerDisplay("{Directory}")]
    public class MediaCrawlerItem
    {
        public MediaCrawlerItem()
        {
            Videos = new List<string>();
            Images = new List<string>();
            Groups = new List<List<MediaCrawlerItem>>();
        }

        public string Directory { get; set; }
        public string? MainImage { get; set; }
        public List<string> Videos { get; set; }
        public List<string> Images { get; set; }
        public List<List<MediaCrawlerItem>> Groups { get; set; }
        public bool IsGrouping { get; set; }
        public MediaType Type { get; set; }
    }
}
