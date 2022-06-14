using MovieDbApi.Common.Domain.Crawling.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;

namespace MovieDbApi.Common.Domain.Media.Models.Monitoring
{
    public class MediaMonitorIntermediateMediaItem
    {
        public string ApiSource { get; set; }
        
        public string ChapterTitle { get; set; }
        
        public string Directory { get; set; }

        public string Duration { get; set; }

        public string DurationPerEpisode { get; set; }

        public string ExternalId { get; set; }

        public string Genre { get; set; }

        public string GroupCount { get; set; }

        public string FilePath { get; set; }
        
        public string Group { get; set; }
        public string GroupCustomCover { get; set; }

        public List<string> Images { get; set; }

        public string MainImage { get; set; }

        public string Plot { get; set; }
        
        public string Rated { get; set; }

        public string Rating { get; set; }

        public string ReleaseDate { get; set; }

        public string SelectedPoster { get; set; }

        public string Staff { get; set; }

        public string Title { get; set; }

        public List<string> Titles { get; set; }
        
        public string Type { get; set; }
        
        public string WebPoster { get; set; }

        public string Url { get; set; }

        public string Year { get; set; }
        
        public string DirectoryOrder { get; set; }

        public MediaItemType MediaType { get; set; }

        public MediaType FileType { get; set; }

        public List<MediaItemLanguage> MediaLanguages { get; set; }
        
        public List<string> Links { get; set; }
    }
}
