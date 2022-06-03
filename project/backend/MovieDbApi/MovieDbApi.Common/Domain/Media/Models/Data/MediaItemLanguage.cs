using System.ComponentModel.DataAnnotations;

namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    public class MediaItemLanguage
    {
        public MediaItemLanguage()
        {

        }

        public MediaItemLanguage(MediaLanguageType type, string language)
        {
            Type = type;
            Language = language;
        }

        [Key]
        public int Id { get; set; }

        public string Language { get; set; }

        public MediaLanguageType Type { get; set; }
    }
}
