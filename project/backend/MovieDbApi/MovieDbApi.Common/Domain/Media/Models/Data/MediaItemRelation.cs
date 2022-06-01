using System.ComponentModel.DataAnnotations;

namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    public class MediaItemRelation
    {
        [Key]
        public int Id { get; set; }

        //public int IdItem { get; set; }

        //public int IdItemRelatedTo { get; set; }

        public MedaItemRelationType Type { get; set; }

        public MediaItem Item { get; set; }

        public MediaItem ItemRelatedTo { get; set; }
    }
}
