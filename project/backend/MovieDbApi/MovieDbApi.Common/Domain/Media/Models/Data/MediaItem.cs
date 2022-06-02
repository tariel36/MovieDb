using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    public class MediaItem
    {
        public MediaItem()
        {
            DateAdded = DateTime.UtcNow;
        }

        [Key]
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? ChapterTitle { get; set; }

        public string? Image { get; set; }

        public string? Path { get; set; }

        [Column(TypeName = "LONGTEXT")]
        public string? Description { get; set; }
        
        public string? Group { get; set; }
        
        public int? GroupId { get; set; }

        public bool IsGrouping { get; set; }

        public DateTime DateAdded { get; set; }

        public string? Instructions { get; set; }

        public string? ExternalId { get; set; }

        public MediaItemType Type { get; set; }

        public virtual ICollection<MediaItemAttribute> Attributes { get; set; }

        public virtual ICollection<MediaItemImage> Images { get; set; }

        public virtual ICollection<MediaItemLanguage> Languages { get; set; }

        public virtual ICollection<MediaItemLink> Links { get; set; }

        //public virtual ICollection<MediaItemRelation> Relations { get; set; }

        public virtual ICollection<MediaItemTitle> Titles { get; set; }
    }
}
