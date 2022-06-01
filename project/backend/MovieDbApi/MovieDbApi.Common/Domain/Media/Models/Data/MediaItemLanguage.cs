using System.ComponentModel.DataAnnotations;

namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    public class MediaItemLanguage
    {
        [Key]
        public int Id { get; set; }

        //public int IdItem { get; set; }

        public string Language { get; set; }

        public MediaLanguageType Type { get; set; }
    }
}
