using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.Anilist.Models
{
    public class Media
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("idMal")]
        public int IdMal { get; set; }

        [JsonProperty("startDate")]
        public StartDate StartDate { get; set; }

        [JsonProperty("endDate")]
        public EndDate EndDate { get; set; }

        [JsonProperty("season")]
        public string Season { get; set; }

        [JsonProperty("seasonYear")]
        public int SeasonYear { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("episodes")]
        public int Episodes { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("chapters")]
        public object Chapters { get; set; }

        [JsonProperty("volumes")]
        public object Volumes { get; set; }

        [JsonProperty("isAdult")]
        public bool IsAdult { get; set; }

        [JsonProperty("averageScore")]
        public int AverageScore { get; set; }

        [JsonProperty("popularity")]
        public int Popularity { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("countryOfOrigin")]
        public string CountryOfOrigin { get; set; }

        [JsonProperty("synonyms")]
        public List<string> Synonyms { get; set; }

        [JsonProperty("coverImage")]
        public CoverImage CoverImage { get; set; }

        [JsonProperty("externalLinks")]
        public List<ExternalLink> ExternalLinks { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("title")]
        public Title Title { get; set; }

        [JsonProperty("isLicensed")]
        public bool IsLicensed { get; set; }

        [JsonProperty("genres")]
        public List<string> Genres { get; set; }

        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; }
    }
}
