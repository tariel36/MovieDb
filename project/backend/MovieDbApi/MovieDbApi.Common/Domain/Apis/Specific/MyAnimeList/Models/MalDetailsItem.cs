using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Specific.MyAnimeList.Models
{
    public class MalDetailsItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("main_picture")]
        public MainPicture MainPicture { get; set; }

        [JsonProperty("alternative_titles")]
        public AlternativeTitles AlternativeTitles { get; set; }

        [JsonProperty("start_date")]
        public string StartDate { get; set; }

        [JsonProperty("end_date")]
        public string EndDate { get; set; }

        [JsonProperty("synopsis")]
        public string Synopsis { get; set; }

        [JsonProperty("mean")]
        public double Mean { get; set; }

        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("popularity")]
        public int Popularity { get; set; }

        [JsonProperty("nsfw")]
        public string Nsfw { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("media_type")]
        public string MediaType { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("genres")]
        public List<Genre> Genres { get; set; }

        [JsonProperty("num_episodes")]
        public int NumEpisodes { get; set; }

        [JsonProperty("start_season")]
        public StartSeason StartSeason { get; set; }

        [JsonProperty("broadcast")]
        public Broadcast Broadcast { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("average_episode_duration")]
        public int AverageEpisodeDuration { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("pictures")]
        public List<Picture> Pictures { get; set; }

        [JsonProperty("background")]
        public string Background { get; set; }

        [JsonProperty("statistics")]
        public Statistics Statistics { get; set; }
    }
}
