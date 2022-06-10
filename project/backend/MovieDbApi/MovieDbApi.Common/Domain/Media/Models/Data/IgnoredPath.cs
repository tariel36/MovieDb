using System.ComponentModel.DataAnnotations;

namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    public class IgnoredPath
    {
        [Key]
        public int Id { get; set; }

        public string Path { get; set; }
    }
}
