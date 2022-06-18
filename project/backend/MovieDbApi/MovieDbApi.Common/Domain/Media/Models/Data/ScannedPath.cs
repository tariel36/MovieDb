using System.ComponentModel.DataAnnotations;

namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    public class ScannedPath
    {
        [Key]
        public int Id { get; set; }

        public string Path { get; set; }
        
        public string? DisplayPath { get; set; }
    }
}
