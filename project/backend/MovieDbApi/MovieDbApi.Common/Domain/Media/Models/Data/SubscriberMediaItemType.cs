using System.ComponentModel.DataAnnotations;

namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    public class SubscriberMediaItemType
    {
        [Key]
        public int Id { get; set; }

        public MediaItemType Type { get; set; }
    }
}
