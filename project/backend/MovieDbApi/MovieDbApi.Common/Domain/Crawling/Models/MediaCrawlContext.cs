namespace MovieDbApi.Common.Domain.Crawling.Models
{
    public class MediaCrawlContext
    {
        public MediaCrawlContext()
        {
            Items = new List<MediaCrawlerItem>();
        }

        public string Path { get; set; }

        public List<MediaCrawlerItem> Items { get; set; }

    }
}
